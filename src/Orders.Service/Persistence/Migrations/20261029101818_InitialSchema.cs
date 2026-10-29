using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orders.Service.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "orders");

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Direction = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ContactId = table.Column<Guid>(type: "uuid", nullable: true),
                    ContactName = table.Column<string>(type: "text", nullable: false),
                    ContactPhone = table.Column<string>(type: "text", nullable: true),
                    ContactAddress = table.Column<string>(type: "text", nullable: true),
                    Currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    SourceLanguage = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ConfirmedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Confidence = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                    table.CheckConstraint("ck_orders_confirmed_at", "(\"Status\" = 'Confirmed') = (\"ConfirmedAt\" IS NOT NULL)");
                    table.CheckConstraint("ck_orders_contact_name", "\"ContactName\" <> ''");
                    table.CheckConstraint("ck_orders_currency", "\"Currency\" ~ '^[A-Z]{3}$'");
                    table.CheckConstraint("ck_orders_direction", "\"Direction\" IN ('Sale', 'Purchase')");
                    table.CheckConstraint("ck_orders_money", "\"Subtotal\" >= 0 AND \"TaxTotal\" >= 0 AND \"Total\" >= 0");
                    table.CheckConstraint("ck_orders_source_language", "\"SourceLanguage\" IN ('Vi', 'En', 'Mixed')");
                    table.CheckConstraint("ck_orders_status", "\"Status\" IN ('Pending', 'Confirmed')");
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                schema: "orders",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    CatalogItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Qty = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxRate = table.Column<decimal>(type: "numeric(7,4)", precision: 7, scale: 4, nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    LineTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Confidence = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => new { x.OrderId, x.Position });
                    table.CheckConstraint("ck_order_items_money", "\"Price\" >= 0 AND \"TaxRate\" >= 0 AND \"TaxAmount\" >= 0 AND \"LineTotal\" >= 0");
                    table.CheckConstraint("ck_order_items_name_unit", "\"Name\" <> '' AND \"Unit\" <> ''");
                    table.CheckConstraint("ck_order_items_qty", "\"Qty\" > 0");
                    table.ForeignKey(
                        name: "FK_order_items_orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "orders",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_OwnerId_CreatedAt",
                schema: "orders",
                table: "orders",
                columns: new[] { "OwnerId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_orders_OwnerId_Status_CreatedAt",
                schema: "orders",
                table: "orders",
                columns: new[] { "OwnerId", "Status", "CreatedAt" });

            // The default privileges in the bootstrap script give the runtime role DML on
            // every table; it must not touch the migration history (ADR 0005).
            migrationBuilder.Sql(
                """
                DO $$ BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'orders_svc') THEN
                        REVOKE ALL ON TABLE orders."__EFMigrationsHistory" FROM orders_svc;
                    END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_items",
                schema: "orders");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "orders");
        }
    }
}
