namespace SuperGood.Shared;

/// <summary>Mirrors document-specs/docs/contracts/contact-schema.v1.json (SIE-25).</summary>
public sealed record ContactV1
{
    public int SchemaVersion { get; init; } = 1;
    public required Guid Id { get; init; }
    public required ContactType Type { get; init; }
    public required string Name { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}

public enum ContactType
{
    Customer,
    Supplier,
}
