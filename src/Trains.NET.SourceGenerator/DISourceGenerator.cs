using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Compilation? compilation = context.Compilation;

            string sourceBuilder = Generate(context, compilation);
            context.AddSource("ServiceLocator.cs", SourceText.From(sourceBuilder, Encoding.UTF8));
        }

        public static string Generate(GeneratorExecutionContext context, Compilation compilation)
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

            var options = (compilation as CSharpCompilation)?.SyntaxTrees[0].Options as CSharpParseOptions;
            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(stub, Encoding.UTF8), options));

            ImmutableArray<Diagnostic> diags = compilation.GetDiagnostics();

            var sourceBuilder = new StringBuilder();

            var services = new List<Service>();

            INamedTypeSymbol? serviceLocatorClass = compilation.GetTypeByMetadataName("DI.ServiceLocator")!;
            INamedTypeSymbol? transientAttribute = compilation.GetTypeByMetadataName("Trains.NET.Engine.TransientAttribute")!;
            INamedTypeSymbol? orderAttribute = compilation.GetTypeByMetadataName("Trains.NET.Engine.OrderAttribute")!;
            INamedTypeSymbol? layoutOfT = compilation.GetTypeByMetadataName("Trains.NET.Engine.ILayout`1")!.ConstructUnboundGenericType();
            INamedTypeSymbol? filteredLayout = compilation.GetTypeByMetadataName("Trains.NET.Engine.FilteredLayout`1")!;
            INamedTypeSymbol? iEnumerableOfT = compilation.GetTypeByMetadataName("System.Collections.Generic.IEnumerable`1")!.ConstructUnboundGenericType();
            INamedTypeSymbol? listOfT = compilation.GetTypeByMetadataName("System.Collections.Generic.List`1")!;

            var knownTypes = new KnownTypes(transientAttribute, orderAttribute, layoutOfT, filteredLayout, iEnumerableOfT, listOfT);

            foreach (SyntaxTree? tree in compilation.SyntaxTrees)
            {
                SemanticModel? semanticModel = compilation.GetSemanticModel(tree);
                IEnumerable<INamedTypeSymbol>? typesToCreate = from i in tree.GetRoot().DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>()
                                                               let symbol = semanticModel.GetSymbolInfo(i).Symbol as IMethodSymbol
                                                               where symbol != null
                                                               where SymbolEqualityComparer.Default.Equals(symbol.ContainingType, serviceLocatorClass)
                                                               select symbol.ReturnType as INamedTypeSymbol;

                foreach (INamedTypeSymbol? typeToCreate in typesToCreate)
                {
                    Generate(context, typeToCreate, compilation, services, null, knownTypes);
                }
            }

            sourceBuilder.AppendLine(@"
namespace DI
{ 
    public static class ServiceLocator
    {");
            var fields = new List<Service>();
            GenerateFields(sourceBuilder, services, fields);

            sourceBuilder.AppendLine(@"
        public static T GetService<T>()
        {");

            foreach (Service? service in services)
            {
                sourceBuilder.AppendLine("if (typeof(T) == typeof(" + service.Type + "))");
                sourceBuilder.AppendLine("{");
                sourceBuilder.AppendLine($"    return (T)(object){GetTypeConstruction(service, service.IsTransient ? new List<Service>() : fields)};");
                sourceBuilder.AppendLine("}");
            }

            sourceBuilder.AppendLine("throw new System.InvalidOperationException(\"Don't know how to initialize type: \" + typeof(T).Name);");
            sourceBuilder.AppendLine(@"
        }
    }
}");

            return sourceBuilder.ToString();
        }

        private static void GenerateFields(StringBuilder sourceBuilder, List<Service> services, List<Service> fields)
        {
            foreach (Service? service in services)
            {
                GenerateFields(sourceBuilder, service.ConstructorArguments, fields);
                if (!service.IsTransient)
                {
                    if (fields.Any(f => SymbolEqualityComparer.Default.Equals(f.ImplementationType, service.ImplementationType)))
                    {
                        continue;
                    }
                    service.VariableName = GetVariableName(service, fields);
                    sourceBuilder.AppendLine($"private static {service.Type} {service.VariableName} = {GetTypeConstruction(service, fields)};");
                    fields.Add(service);
                }
            }
        }

        private static string GetTypeConstruction(Service service, List<Service> fields)
        {
            var sb = new StringBuilder();

            Service? field = fields.FirstOrDefault(f => SymbolEqualityComparer.Default.Equals(f.ImplementationType, service.ImplementationType));
            if (field != null)
            {
                sb.Append(field.VariableName);
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
                foreach (Service? arg in service.ConstructorArguments)
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

        private static string GetVariableName(Service service, List<Service> fields)
        {
            string typeName = service.ImplementationType.ToString().Replace("<", "").Replace(">", "").Replace("?", "");

            string[] parts = typeName.Split('.');
            for (int i = parts.Length - 1; i >= 0; i--)
            {
                string? candidate = string.Join("", parts.Skip(i));
                candidate = "_" + char.ToLowerInvariant(candidate[0]) + candidate.Substring(1);
                if (!fields.Any(f => string.Equals(f.VariableName, candidate, StringComparison.Ordinal)))
                {
                    typeName = candidate;
                    break;
                }
            }
            return typeName;
        }

        private static void Generate(GeneratorExecutionContext context, INamedTypeSymbol typeToCreate, Compilation compilation, List<Service> services, Service? parent, KnownTypes knownTypes)
        {
            // System.Diagnostics.Debugger.Launch();
            typeToCreate = (INamedTypeSymbol)typeToCreate.WithNullableAnnotation(default);

            if (typeToCreate.IsGenericType && SymbolEqualityComparer.Default.Equals(typeToCreate.ConstructUnboundGenericType(), knownTypes.IEnumerableOfT))
            {
                ITypeSymbol? typeToFind = typeToCreate.TypeArguments[0];
                var types = FindImplementations(context, compilation, knownTypes, typeToFind);

                if (!types.Any())
                {
                    context.ReportDiagnostic(Diagnostic.Create("TRAINS2", "DI", $"Can't find an implemnentation for {typeToFind} to construct an IEnumerable", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, false));
                }

                INamedTypeSymbol? list = knownTypes.ListOfT.Construct(typeToFind);
                var listService = new Service(typeToCreate, parent)
                {
                    ImplementationType = list,
                    UseCollectionInitializer = true
                };

                if (CheckForCycle(context, services, list))
                {
                    return;
                }
                services.Add(listService);

                foreach (INamedTypeSymbol? thingy in types)
                {
                    Generate(context, thingy, compilation, listService.ConstructorArguments, listService, knownTypes);
                }
            }
            else if (typeToCreate.IsGenericType && SymbolEqualityComparer.Default.Equals(typeToCreate.ConstructUnboundGenericType(), knownTypes.ILayoutOfT))
            {
                ITypeSymbol? entityType = typeToCreate.TypeArguments[0];

                INamedTypeSymbol? layout = knownTypes.FilteredLayout.Construct(entityType);

                var layoutService = new Service(typeToCreate, parent);
                services.Add(layoutService);
                layoutService.ImplementationType = layout;
                Generate(context, layout, compilation, layoutService.ConstructorArguments, layoutService, knownTypes);
            }
            else
            {
                INamedTypeSymbol? realType = typeToCreate.IsAbstract ? FindImplementation(context, compilation, knownTypes, typeToCreate) : typeToCreate;

                if (realType == null)
                {
                    context.ReportDiagnostic(Diagnostic.Create("TRAINS1", "DI", $"Can't find an implemnentation for {typeToCreate}", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, false));
                }
                else
                {
                    var service = new Service(typeToCreate, parent)
                    {
                        ImplementationType = realType,
                        IsTransient = typeToCreate.GetAttributes().Any(c => SymbolEqualityComparer.Default.Equals(c.AttributeClass, knownTypes.TransientAttribute))
                    };

                    if (CheckForCycle(context, services, realType))
                    {
                        return;
                    }
                    services.Add(service);

                    IMethodSymbol? constructor = realType?.Constructors.FirstOrDefault();
                    if (constructor != null)
                    {
                        foreach (IParameterSymbol? parametr in constructor.Parameters)
                        {
                            if (parametr.Type is INamedTypeSymbol paramType)
                            {
                                Generate(context, paramType, compilation, service.ConstructorArguments, service, knownTypes);
                            }
                        }
                    }
                }
            }
        }

        private static bool CheckForCycle(GeneratorExecutionContext context, List<Service> services, INamedTypeSymbol typeToCreate)
        {
            foreach (var service in services)
            {
                var current = service;
                while (current != null)
                {
                    if (SymbolEqualityComparer.Default.Equals(current.ImplementationType, typeToCreate))
                    {
                        context.ReportDiagnostic(Diagnostic.Create("TRAINS3", "DI", $"Circular reference detected: {typeToCreate}", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, false));
                        return true;
                    }
                    current = current.Parent;
                }
            }
            return false;
        }

        private static INamedTypeSymbol? FindImplementation(GeneratorExecutionContext context, Compilation compilation, KnownTypes knownTypes, ITypeSymbol typeToFind)
        {
            return FindImplementations(context, compilation, knownTypes, typeToFind).FirstOrDefault();
        }

        private static IOrderedEnumerable<INamedTypeSymbol> FindImplementations(GeneratorExecutionContext context, Compilation compilation, KnownTypes knownTypes, ITypeSymbol typeToFind)
        {
            return FindImplementations(context, typeToFind, compilation).OrderBy(t => (int)(t.GetAttributes().FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, knownTypes.OrderAttribute))?.ConstructorArguments[0].Value ?? 0));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static IEnumerable<INamedTypeSymbol> FindImplementations(GeneratorExecutionContext context, ITypeSymbol typeToFind, Compilation compilation)
        {
            foreach (INamespaceSymbol? ns in compilation.GlobalNamespace.GetNamespaceMembers())
            {
                if (ns.Name == "System" || ns.Name == "Microsoft" || ns.Name == "SkiaSharp" || ns.Name == "OpenTK") continue;

                int count = 0;
                foreach (INamedTypeSymbol? x in GetAllTypes(new[] { ns }))
                {
                    count++;
                    if (!x.IsAbstract && x.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, typeToFind)))
                    {
                        yield return x;
                    }
                }
            }
        }

        private static IEnumerable<INamedTypeSymbol> GetAllTypes(IEnumerable<INamespaceSymbol> namespaces)
        {
            foreach (INamespaceSymbol? ns in namespaces)
            {
                foreach (INamedTypeSymbol? t in ns.GetTypeMembers())
                {
                    yield return t;
                }

                foreach (INamedTypeSymbol? subType in GetAllTypes(ns.GetNamespaceMembers()))
                {
                    yield return subType;
                }
            }
        }
    }
}
