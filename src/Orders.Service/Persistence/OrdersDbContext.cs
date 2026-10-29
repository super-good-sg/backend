using Microsoft.EntityFrameworkCore;
using SuperGood.Shared;

namespace SuperGood.Orders.Persistence;

/// <summary>Everything lives in the <c>orders</c> schema (ADR 0005). The runtime
/// database user has row-level rights there and nowhere else.</summary>
public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public const string SchemaName = "orders";

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("orders", t =>
            {
                t.HasCheckConstraint("ck_orders_direction", "\"Direction\" IN ('Sale', 'Purchase')");
                t.HasCheckConstraint("ck_orders_status", "\"Status\" IN ('Pending', 'Confirmed')");
                t.HasCheckConstraint("ck_orders_source_language", "\"SourceLanguage\" IN ('Vi', 'En', 'Mixed')");
                t.HasCheckConstraint("ck_orders_confirmed_at", "(\"Status\" = 'Confirmed') = (\"ConfirmedAt\" IS NOT NULL)");
                t.HasCheckConstraint("ck_orders_contact_name", "\"ContactName\" <> ''");
                t.HasCheckConstraint("ck_orders_currency", "\"Currency\" ~ '^[A-Z]{3}$'");
                t.HasCheckConstraint("ck_orders_money", "\"Subtotal\" >= 0 AND \"TaxTotal\" >= 0 AND \"Total\" >= 0");
            });
            e.HasKey(o => o.Id);
            e.Property(o => o.Direction).HasConversion<string>().HasMaxLength(16);
            e.Property(o => o.Status).HasConversion<string>().HasMaxLength(16);
            e.Property(o => o.SourceLanguage).HasConversion<string>().HasMaxLength(16);
            e.Property(o => o.ContactName).IsRequired();
            e.Property(o => o.Currency).HasMaxLength(3).IsFixedLength().IsRequired();
            e.Property(o => o.Subtotal).HasPrecision(18, 2);
            e.Property(o => o.TaxTotal).HasPrecision(18, 2);
            e.Property(o => o.Total).HasPrecision(18, 2);
            e.OwnsOne(o => o.Confidence, b => b.ToJson());
            e.HasMany(o => o.Items).WithOne().HasForeignKey(i => i.OrderId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(o => new { o.OwnerId, o.CreatedAt });
            e.HasIndex(o => new { o.OwnerId, o.Status, o.CreatedAt });
        });

        modelBuilder.Entity<OrderItem>(e =>
        {
            e.ToTable("order_items", t =>
            {
                t.HasCheckConstraint("ck_order_items_name_unit", "\"Name\" <> '' AND \"Unit\" <> ''");
                t.HasCheckConstraint("ck_order_items_qty", "\"Qty\" > 0");
                t.HasCheckConstraint("ck_order_items_money", "\"Price\" >= 0 AND \"TaxRate\" >= 0 AND \"TaxAmount\" >= 0 AND \"LineTotal\" >= 0");
            });
            e.HasKey(i => new { i.OrderId, i.Position });
            e.Property(i => i.Name).IsRequired();
            e.Property(i => i.Unit).IsRequired();
            e.Property(i => i.Qty).HasPrecision(18, 4);
            e.Property(i => i.Price).HasPrecision(18, 2);
            e.Property(i => i.TaxRate).HasPrecision(7, 4);
            e.Property(i => i.TaxAmount).HasPrecision(18, 2);
            e.Property(i => i.LineTotal).HasPrecision(18, 2);
            e.OwnsOne(i => i.Confidence, b => b.ToJson());
        });
    }
}
