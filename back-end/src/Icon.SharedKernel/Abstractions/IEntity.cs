namespace Icon.SharedKernel.Abstractions;

/// <summary>
/// Any object stored in the database that has a unique identifier.
/// </summary>
public interface IEntity<TPrimaryKey>
{
    /// <summary>
    /// The unique identifier for this entity.
    /// </summary>
    TPrimaryKey Id { get; set; }
}
