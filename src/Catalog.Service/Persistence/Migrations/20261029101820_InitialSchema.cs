using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");

            migrationBuilder.CreateTable(
                name: "catalog_items",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StockOnHand = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false, defaultValue: 0m),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalog_items", x => x.Id);
                    table.CheckConstraint("ck_catalog_items_name_unit", "\"Name\" <> '' AND \"Unit\" <> ''");
                    table.CheckConstraint("ck_catalog_items_price", "\"Price\" >= 0");
                });

            migrationBuilder.CreateTable(
                name: "contacts",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacts", x => x.Id);
                    table.CheckConstraint("ck_contacts_name", "\"Name\" <> ''");
                    table.CheckConstraint("ck_contacts_type", "\"Type\" IN ('Customer', 'Supplier')");
                });

            migrationBuilder.CreateTable(
                name: "price_history",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CatalogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ChangedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_price_history", x => x.Id);
                    table.CheckConstraint("ck_price_history_price", "\"Price\" >= 0");
                    table.ForeignKey(
                        name: "FK_price_history_catalog_items_CatalogItemId",
                        column: x => x.CatalogItemId,
                        principalSchema: "catalog",
                        principalTable: "catalog_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_catalog_items_OwnerId_Name",
                schema: "catalog",
                table: "catalog_items",
                columns: new[] { "OwnerId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_contacts_OwnerId_Type_Name",
                schema: "catalog",
                table: "contacts",
                columns: new[] { "OwnerId", "Type", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_price_history_CatalogItemId_ChangedAt",
                schema: "catalog",
                table: "price_history",
                columns: new[] { "CatalogItemId", "ChangedAt" });

            // The default privileges in the bootstrap script give the runtime role DML on
            // every table; it must not touch the migration history (ADR 0005).
            migrationBuilder.Sql(
                """
                DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'catalog_svc') THEN
                        REVOKE ALL ON TABLE catalog."__EFMigrationsHistory" FROM catalog_svc;
                    END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contacts",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "price_history",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "catalog_items",
                schema: "catalog");
        }
    }
}
