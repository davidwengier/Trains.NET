using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Trains.NET.SourceGenerator;

public class Service
{
    public Service(INamedTypeSymbol typeToCreate, Service? parent)
    {
        this.Type = typeToCreate;
        this.Parent = parent;
    }

    public INamedTypeSymbol Type { get; set; }
    public INamedTypeSymbol ImplementationType { get; internal set; } = null!;
    public List<Service> ConstructorArguments { get; internal set; } = new List<Service>();
    public bool IsTransient { get; internal set; }
    public bool UseCollectionInitializer { get; internal set; }
    public string? VariableName { get; internal set; }
    public Service? Parent { get; internal set; }
    public ITypeSymbol? ElementType { get; internal set; }
}
