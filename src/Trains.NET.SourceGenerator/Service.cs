using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Trains.NET.SourceGenerator
{
    public class Service
    {
        public Service(INamedTypeSymbol typeToCreate)
        {
            this.Type = typeToCreate;
        }

        public INamedTypeSymbol Type { get; set; }
        public INamedTypeSymbol ImplementationType { get; internal set; } = null!;
        public List<Service> ConstructorArguments { get; internal set; } = new List<Service>();
        public bool IsTransient { get; internal set; }
        public bool UseCollectionInitializer { get; internal set; }
        public string? VariableName { get; internal set; }
    }
}
