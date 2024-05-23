using Microsoft.CodeAnalysis;

namespace Trains.NET.SourceGenerator;

public class Service(INamedTypeSymbol typeToCreate, Service? parent)
{
    public INamedTypeSymbol Type { get; set; } = typeToCreate;
    public INamedTypeSymbol ImplementationType { get; internal set; } = null!;
    public List<Service> ConstructorArguments { get; internal set; } = new List<Service>();
    public bool IsTransient { get; internal set; }
    public bool UseCollectionInitializer { get; internal set; }
    public string? VariableName { get; internal set; }
    public Service? Parent { get; internal set; } = parent;
    public ITypeSymbol? ElementType { get; internal set; }
}
