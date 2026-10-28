namespace SuperGood.Shared;

/// <summary>Mirrors document-specs/docs/contracts/order-schema.v1.json (SIE-24).</summary>
public sealed record OrderV1
{
    public int SchemaVersion { get; init; } = 1;
    public required Guid Id { get; init; }
    public required OrderDirection Direction { get; init; }
    public required OrderStatus Status { get; init; }
    public required OrderContactV1 Contact { get; init; }
    public required IReadOnlyList<OrderItemV1> Items { get; init; }
    public required string Currency { get; init; }
    public required decimal Subtotal { get; init; }
    public required decimal TaxTotal { get; init; }
    public required decimal Total { get; init; }
    public required SourceLanguage SourceLanguage { get; init; }
    public OrderConfidenceV1? Confidence { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public DateTimeOffset? ConfirmedAt { get; init; }
}

public enum OrderDirection
{
    Sale,
    Purchase,
}

public enum OrderStatus
{
    Pending,
    Confirmed,
}

public enum SourceLanguage
{
    Vi,
    En,
    Mixed,
}

public sealed record OrderContactV1
{
    public Guid? ContactId { get; init; }
    public required string Name { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
}

public sealed record OrderItemV1
{
    public Guid? CatalogItemId { get; init; }
    public required string Name { get; init; }
    public required decimal Qty { get; init; }
    public required string Unit { get; init; }
    public required decimal Price { get; init; }
    public required decimal TaxRate { get; init; }
    public required decimal TaxAmount { get; init; }
    public required decimal LineTotal { get; init; }
    public OrderItemConfidenceV1? Confidence { get; init; }
}

public sealed record OrderItemConfidenceV1
{
    public double? Name { get; init; }
    public double? Qty { get; init; }
    public double? Unit { get; init; }
    public double? Price { get; init; }
    public double? TaxRate { get; init; }
    public double? TaxAmount { get; init; }
}

public sealed record OrderConfidenceV1
{
    public double? Contact { get; init; }
}
