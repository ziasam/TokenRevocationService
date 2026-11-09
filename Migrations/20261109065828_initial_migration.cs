using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TokenRevocationService.Migrations
{
    /// <inheritdoc />
    public partial class initial_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "revoked_tokens",
                columns: table => new
                {
                    token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    revoked_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reason = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_revoked_tokens", x => x.token);
                });

            migrationBuilder.CreateIndex(
                name: "ix_revoked_tokens_expires_at_utc",
                table: "revoked_tokens",
                column: "expires_at_utc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "revoked_tokens");
        }
    }
}
