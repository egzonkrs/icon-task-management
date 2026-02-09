namespace Icon.SharedKernel.Abstractions;

public interface IDatabaseInitializer
{
    Task InitializeAsync();
}
