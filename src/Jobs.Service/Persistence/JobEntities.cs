namespace SuperGood.Jobs.Persistence;

/// <summary>Persistence model for the OpenAPI v1 <c>Job</c>. SIE-27 extends it with
/// retry and dead-letter detail.</summary>
public sealed class Job
{
    /// <summary>Also the idempotency key when the worker creates the pending order in
    /// the Orders service.</summary>
    public Guid Id { get; set; }

    /// <summary>Keycloak <c>sub</c>; every query is scoped to it.</summary>
    public Guid OwnerId { get; set; }

    public JobType Type { get; set; }
    public JobStatus Status { get; set; }

    /// <summary>JSON matching order-schema.v1.json or product-schema.v1.json by
    /// <see cref="Type"/>. Set only when <see cref="Status"/> is Succeeded. The Orders
    /// service owns the order itself; this is the extraction output.</summary>
    public string? Result { get; set; }

    /// <summary>JSON <c>Problem</c>. Set only when <see cref="Status"/> is Failed.</summary>
    public string? Error { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public enum JobType
{
    OrderExtraction,
    ProductExtraction,
}

public enum JobStatus
{
    Pending,
    Processing,
    Succeeded,
    Failed,
}
