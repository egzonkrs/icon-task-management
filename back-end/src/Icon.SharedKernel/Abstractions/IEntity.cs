namespace Icon.SharedKernel.Abstractions;

public interface IEntity<TPrimaryKey>
{
    TPrimaryKey Id { get; }
}
