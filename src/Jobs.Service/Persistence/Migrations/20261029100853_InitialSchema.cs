using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jobs.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "jobs");

            migrationBuilder.CreateTable(
                name: "jobs",
                schema: "jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Result = table.Column<string>(type: "jsonb", nullable: true),
                    Error = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jobs", x => x.Id);
                    table.CheckConstraint("ck_jobs_error", "(\"Status\" = 'Failed') = (\"Error\" IS NOT NULL)");
                    table.CheckConstraint("ck_jobs_result", "(\"Status\" = 'Succeeded') = (\"Result\" IS NOT NULL)");
                    table.CheckConstraint("ck_jobs_status", "\"Status\" IN ('Pending', 'Processing', 'Succeeded', 'Failed')");
                    table.CheckConstraint("ck_jobs_type", "\"Type\" IN ('OrderExtraction', 'ProductExtraction')");
                });

            migrationBuilder.CreateIndex(
                name: "IX_jobs_OwnerId_CreatedAt",
                schema: "jobs",
                table: "jobs",
                columns: new[] { "OwnerId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_jobs_Status",
                schema: "jobs",
                table: "jobs",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jobs",
                schema: "jobs");
        }
    }
}
