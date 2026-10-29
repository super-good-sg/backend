using Microsoft.EntityFrameworkCore;
using SuperGood.Shared;

namespace SuperGood.Catalog.Persistence;

/// <summary>Everything lives in the <c>catalog</c> schema (ADR 0005). The runtime
/// database user has row-level rights there and nowhere else.</summary>
public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public const string SchemaName = "catalog";

    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<PriceHistoryEntry> PriceHistory => Set<PriceHistoryEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.Entity<Contact>(e =>
        {
            e.ToTable("contacts", t =>
            {
                t.HasCheckConstraint("ck_contacts_type", "\"Type\" IN ('Customer', 'Supplier')");
                t.HasCheckConstraint("ck_contacts_name", "\"Name\" <> ''");
            });
            e.HasKey(c => c.Id);
            e.Property(c => c.Type).HasConversion<string>().HasMaxLength(16);
            e.Property(c => c.Name).IsRequired();
            e.HasIndex(c => new { c.OwnerId, c.Type, c.Name });
        });

        modelBuilder.Entity<CatalogItem>(e =>
        {
            e.ToTable("catalog_items", t =>
            {
                t.HasCheckConstraint("ck_catalog_items_name_unit", "\"Name\" <> '' AND \"Unit\" <> ''");
                t.HasCheckConstraint("ck_catalog_items_price", "\"Price\" >= 0");
            });
            e.HasKey(i => i.Id);
            e.Property(i => i.Name).IsRequired();
            e.Property(i => i.Unit).IsRequired();
            e.Property(i => i.Price).HasPrecision(18, 2);
            e.Property(i => i.StockOnHand).HasPrecision(18, 4).HasDefaultValue(0m);
            e.HasMany(i => i.PriceHistory).WithOne().HasForeignKey(h => h.CatalogItemId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(i => new { i.OwnerId, i.Name });
        });

        modelBuilder.Entity<PriceHistoryEntry>(e =>
        {
            e.ToTable("price_history", t =>
                t.HasCheckConstraint("ck_price_history_price", "\"Price\" >= 0"));
            e.HasKey(h => h.Id);
            e.Property(h => h.Price).HasPrecision(18, 2);
            e.HasIndex(h => new { h.CatalogItemId, h.ChangedAt });
        });
    }
}
