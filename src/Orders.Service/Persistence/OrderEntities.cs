using SuperGood.Shared;

namespace SuperGood.Orders.Persistence;

/// <summary>Persistence model for order-schema.v1.json. Contact and item fields are
/// point-in-time snapshots: <see cref="ContactId"/> and
/// <see cref="OrderItem.CatalogItemId"/> reference the Catalog service by id only,
/// with no foreign key (ADR 0005).</summary>
public sealed class Order
{
    public Guid Id { get; set; }

    /// <summary>Keycloak <c>sub</c>; every query is scoped to it.</summary>
    public Guid OwnerId { get; set; }

    public OrderDirection Direction { get; set; }
    public OrderStatus Status { get; set; }
    public Guid? ContactId { get; set; }
    public string ContactName { get; set; } = null!;
    public string? ContactPhone { get; set; }
    public string? ContactAddress { get; set; }
    public string Currency { get; set; } = null!;
    public decimal Subtotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal Total { get; set; }
    public SourceLanguage SourceLanguage { get; set; }
    public OrderConfidence? Confidence { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? ConfirmedAt { get; set; }
    public List<OrderItem> Items { get; set; } = [];
}

public sealed class OrderItem
{
    public Guid OrderId { get; set; }

    /// <summary>Zero-based line position; keeps item order stable.</summary>
    public int Position { get; set; }

    public Guid? CatalogItemId { get; set; }
    public string Name { get; set; } = null!;
    public decimal Qty { get; set; }
    public string Unit { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public OrderItemConfidence? Confidence { get; set; }
}

public sealed class OrderConfidence
{
    public double? Contact { get; set; }
}

public sealed class OrderItemConfidence
{
    public double? Name { get; set; }
    public double? Qty { get; set; }
    public double? Unit { get; set; }
    public double? Price { get; set; }
    public double? TaxRate { get; set; }
    public double? TaxAmount { get; set; }
}
