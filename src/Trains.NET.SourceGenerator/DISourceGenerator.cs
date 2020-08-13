using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Trains.NET.SourceGenerator
{
    [Generator]
    public class DISourceGenerator : ISourceGenerator
    {
        public void Initialize(InitializationContext context)
        {
        }

        public void Execute(SourceGeneratorContext context)
        {
            var compilation = context.Compilation;

            string sourceBuilder = Generate(compilation);

            context.AddSource("digenerator.cs", SourceText.From(sourceBuilder, Encoding.UTF8));
        }

        public static string Generate(Compilation compilation)
        {
            string stub = @"
namespace DI
{ 
    public static class ServiceLocator
    {
        public static T GetService<T>()
        {
            return default;
        }
    }
}
";

            CSharpParseOptions? options = (compilation as CSharpCompilation)?.SyntaxTrees[0].Options as CSharpParseOptions;
            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(stub, Encoding.UTF8), options));

            var diags = compilation.GetDiagnostics();

            var sourceBuilder = new StringBuilder();

            List<Service> services = new List<Service>();

            var serviceLocatorClass = compilation.GetTypeByMetadataName("DI.ServiceLocator")!;
            var transientAttribute = compilation.GetTypeByMetadataName("Trains.NET.Engine.TransientAttribute")!;
            var orderAttribute = compilation.GetTypeByMetadataName("Trains.NET.Engine.OrderAttribute")!;
            var layoutOfT = compilation.GetTypeByMetadataName("Trains.NET.Engine.ILayout`1")!.ConstructUnboundGenericType();
            var filteredLayout = compilation.GetTypeByMetadataName("Trains.NET.Engine.Tracks.FilteredLayout`1")!;
            var iEnumerableOfT = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")!.ConstructUnboundGenericType();
            var listOfT = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")!;

            var knownTypes = new KnownTypes(transientAttribute, orderAttribute, layoutOfT, filteredLayout, iEnumerableOfT, listOfT);

            foreach (var tree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);
                var typesToCreate = from i in tree.GetRoot().DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>()
                                    let symbol = semanticModel.GetSymbolInfo(i).Symbol as IMethodSymbol
                                    where symbol != null
                                    where SymbolEqualityComparer.Default.Equals(symbol.ContainingType, serviceLocatorClass)
                                    select symbol.ReturnType.OriginalDefinition as INamedTypeSymbol;

                foreach (var typeToCreate in typesToCreate)
                {
                    Generate(sourceBuilder, typeToCreate, compilation, services, knownTypes);
                }
            }

            sourceBuilder.AppendLine(@"
namespace DI
{ 
    public static class ServiceLocator
    {");
            List<Service> fields = new List<Service>();
            GenerateFields(sourceBuilder, services, fields);

            sourceBuilder.AppendLine(@"
        public static T GetService<T>()
        {");

            foreach (var service in services)
            {
                sourceBuilder.AppendLine("if (typeof(T) == typeof(" + service.Type + "))");
                sourceBuilder.AppendLine("{");
                sourceBuilder.AppendLine($"    return (T)(object){GetTypeConstruction(service, service.IsTransient ? new List<Service>() : fields)};");
                sourceBuilder.AppendLine("}");
            }

            sourceBuilder.AppendLine("return default;");
            sourceBuilder.AppendLine(@"
        }
    }
}");

            return sourceBuilder.ToString();
        }

        private static void GenerateFields(StringBuilder sourceBuilder, List<Service> services, List<Service> fields)
        {
            foreach (var service in services)
            {
                GenerateFields(sourceBuilder, service.ConstructorArguments, fields);
                if (!service.IsTransient)
                {
                    if (fields.Any(f => SymbolEqualityComparer.Default.Equals(f.ImplementationType, service.ImplementationType)))
                    {
                        continue;
                    }
                    sourceBuilder.AppendLine($"private static {service.Type} {GetVariableName(service)} = {GetTypeConstruction(service, fields)};");
                    fields.Add(service);
                }
            }
        }

        private static string GetTypeConstruction(Service service, List<Service> fields)
        {
            StringBuilder sb = new StringBuilder();

            var field = fields.FirstOrDefault(f => SymbolEqualityComparer.Default.Equals(f.ImplementationType, service.ImplementationType));
            if (field != null)
            {
                sb.Append(GetVariableName(field));
            }
            else
            {
                sb.Append("new ");
                sb.Append(service.ImplementationType);
                sb.Append('(');
                if (service.UseCollectionInitializer)
                {
                    sb.Append(')');
                    sb.Append('{');
                }
                bool first = true;
                foreach (var arg in service.ConstructorArguments)
                {
                    if (!first)
                    {
                        sb.Append(',');
                    }
                    sb.Append(GetTypeConstruction(arg, fields));
                    first = false;
                }
                if (service.UseCollectionInitializer)
                {
                    sb.Append('}');
                }
                else
                {
                    sb.Append(')');
                }
            }
            return sb.ToString();
        }

        private static string GetVariableName(Service service)
        {
            string typeName = service.ImplementationType.ToString();
            return "_" + typeName.Replace(".", "_").Replace("<", "").Replace(">", "").Replace("?", "");
        }

        private static void Generate(StringBuilder sourceBuilder, INamedTypeSymbol typeToCreate, Compilation compilation, List<Service> services, KnownTypes knownTypes)
        {
            typeToCreate = (INamedTypeSymbol)typeToCreate.WithNullableAnnotation(default);

            if (typeToCreate.IsGenericType && SymbolEqualityComparer.Default.Equals(typeToCreate.ConstructUnboundGenericType(), knownTypes.IEnumerableOfT))
            {
                var typeToFind = typeToCreate.TypeArguments[0];
                var types = FindImplementations(typeToFind, compilation).OrderBy(t => (int)(t.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, knownTypes.OrderAttribute))?.ConstructorArguments[0].Value ?? 0));

                var list = knownTypes.ListOfT.Construct(typeToFind);
                var listService = new Service(typeToCreate);
                services.Add(listService);
                listService.ImplementationType = list;
                listService.UseCollectionInitializer = true;

                foreach (var thingy in types)
                {
                    Generate(sourceBuilder, thingy, compilation, listService.ConstructorArguments, knownTypes);
                }
            }
            else if (typeToCreate.IsGenericType && SymbolEqualityComparer.Default.Equals(typeToCreate.ConstructUnboundGenericType(), knownTypes.ILayoutOfT))
            {
                var entityType = typeToCreate.TypeArguments[0];

                var layout = knownTypes.FilteredLayout.Construct(entityType);

                var layoutService = new Service(typeToCreate);
                services.Add(layoutService);
                layoutService.ImplementationType = layout;
                Generate(sourceBuilder, layout, compilation, layoutService.ConstructorArguments, knownTypes);
            }
            else
            {
                var realType = typeToCreate.IsAbstract ? FindImplementation(typeToCreate, compilation) : typeToCreate;

                if (realType != null)
                {
                    var service = new Service(typeToCreate);
                    services.Add(service);
                    service.ImplementationType = realType;
                    service.IsTransient = typeToCreate.GetAttributes().Any(c => SymbolEqualityComparer.Default.Equals(c.AttributeClass, knownTypes.TransientAttribute));

                    sourceBuilder.AppendLine("//     Found: " + realType);

                    var constructor = realType?.Constructors.FirstOrDefault();
                    if (constructor != null)
                    {
                        foreach (var parametr in constructor.Parameters)
                        {
                            if (parametr.Type is INamedTypeSymbol paramType)
                            {
                                Generate(sourceBuilder, paramType, compilation, service.ConstructorArguments, knownTypes);
                            }
                        }
                    }
                }
            }
        }

        private static INamedTypeSymbol? FindImplementation(ITypeSymbol typeToCreate, Compilation compilation)
        {
            return FindImplementations(typeToCreate, compilation).FirstOrDefault();
        }

        private static IEnumerable<INamedTypeSymbol> FindImplementations(ITypeSymbol typeToFind, Compilation compilation)
        {
            foreach (var x in GetAllTypes(compilation.GlobalNamespace.GetNamespaceMembers()))
            {
                if (!x.IsAbstract && x.Interfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, typeToFind)))
                {
                    yield return x;
                }
            }
        }

        private static IEnumerable<INamedTypeSymbol> GetAllTypes(IEnumerable<INamespaceSymbol> namespaces)
        {
            foreach (var ns in namespaces)
            {
                foreach (var t in ns.GetTypeMembers())
                {
                    yield return t;
                }

                foreach (var subType in GetAllTypes(ns.GetNamespaceMembers()))
                {
                    yield return subType;
                }
            }
        }
    }
}
