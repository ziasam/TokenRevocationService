using Microsoft.EntityFrameworkCore;
using TokenRevocationService.Domain.Entities;

namespace TokenRevocationService.Infrastructure;

public class TokenRevocationDbContext : DbContext
{
    public TokenRevocationDbContext(DbContextOptions<TokenRevocationDbContext> options) : base(options)
    {
    }

    public DbSet<RevokedTokenRecord> RevokedTokens => Set<RevokedTokenRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RevokedTokenRecord>(entity =>
        {
            entity.HasKey(e => e.Token);

            entity.HasIndex(e => e.ExpiresAtUtc)
                  .HasDatabaseName("ix_revoked_tokens_expires_at_utc");
        });
    }
}
