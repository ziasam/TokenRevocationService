using TokenRevocationService.Application.Interfaces;

namespace TokenRevocationService.Infrastructure.Postgres;

public class TokenCleanupBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupBackgroundService> _logger;
    private readonly TimeSpan _interval;

    public TokenCleanupBackgroundService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<TokenCleanupBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        var minutes = configuration.GetValue<int>("CleanupSettings:IntervalMinutes", 10);
        _interval = TimeSpan.FromMinutes(minutes);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TokenCleanupBackgroundService started. Purge interval: {Interval}", _interval);

        using var timer = new PeriodicTimer(_interval);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var store = scope.ServiceProvider.GetRequiredService<ITokenRevocationStore>();
                await store.PurgeExpiredTokensAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during scheduled token expiration cleanup.");
            }
        }
    }
}
