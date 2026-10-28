namespace SuperGood.Shared;

/// <summary>Mirrors document-specs/docs/contracts/catalog-item-schema.v1.json (SIE-25).</summary>
public sealed record CatalogItemV1
{
    public int SchemaVersion { get; init; } = 1;
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Unit { get; init; }
    public required decimal Price { get; init; }

    /// <summary>On-hand count (ADR 0005). Read-only via the API: changes only
    /// through the order-confirmed event, never a direct write.</summary>
    public required decimal StockOnHand { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}
