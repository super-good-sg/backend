using SuperGood.Shared;

namespace SuperGood.Catalog.Persistence;

/// <summary>Persistence model for contact-schema.v1.json (SIE-25). Contacts live in
/// the catalog schema (ADR 0005).</summary>
public sealed class Contact
{
    public Guid Id { get; set; }

    /// <summary>Keycloak <c>sub</c>; every query is scoped to it.</summary>
    public Guid OwnerId { get; set; }

    public ContactType Type { get; set; }
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

/// <summary>Persistence model for catalog-item-schema.v1.json (SIE-25).</summary>
public sealed class CatalogItem
{
    public Guid Id { get; set; }

    /// <summary>Keycloak <c>sub</c>; every query is scoped to it.</summary>
    public Guid OwnerId { get; set; }

    public string Name { get; set; } = null!;
    public string Unit { get; set; } = null!;
    public decimal Price { get; set; }

    /// <summary>On-hand count (ADR 0005). Written only by the order-confirmed event
    /// handler, never by an API write. Not constrained to be non-negative: the
    /// contract sets no minimum.</summary>
    public decimal StockOnHand { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<PriceHistoryEntry> PriceHistory { get; set; } = [];
}

/// <summary>One entry per price change, appended when <see cref="CatalogItem.Price"/> changes.</summary>
public sealed class PriceHistoryEntry
{
    public Guid Id { get; set; }
    public Guid CatalogItemId { get; set; }
    public decimal Price { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
}
