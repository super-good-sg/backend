namespace SuperGood.Shared;

/// <summary>Mirrors document-specs/docs/contracts/product-schema.v1.json (SIE-24).
/// Extraction-only shape; price history and full catalog-item fields are a
/// catalog-service concern, not part of this contract.</summary>
public sealed record ProductV1
{
    public int SchemaVersion { get; init; } = 1;

    /// <summary>Required by the schema even while null (new item, no catalog match).</summary>
    public required Guid? CatalogItemId { get; init; }
    public required string Name { get; init; }
    public required string Unit { get; init; }
    public required decimal Price { get; init; }
    public ProductConfidenceV1? Confidence { get; init; }
    public required SourceLanguage SourceLanguage { get; init; }
}

public sealed record ProductConfidenceV1
{
    public double? Name { get; init; }
    public double? Unit { get; init; }
    public double? Price { get; init; }
}
