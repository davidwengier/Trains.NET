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

            //System.IO.File.WriteAllText(@"C:\Users\dawengie\Desktop\SourceGenerator.txt", sourceBuilder, Encoding.UTF8);

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

            var sourceText = SourceText.From(stub, Encoding.UTF8);
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceText, ((CSharpSyntaxTree)compilation.SyntaxTrees.First()).Options);
            compilation = compilation.AddSyntaxTrees(syntaxTree);

            var diags = compilation.GetDiagnostics();

            var sourceBuilder = new StringBuilder();

            foreach (var tree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);

                var invocations = tree.GetRoot().DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>();

                foreach (var methodCall in invocations)
                {
                    var symbol = semanticModel.GetSymbolInfo(methodCall).Symbol;

                    if (symbol?.ContainingType.Name == "ServiceLocator")
                    {
                        if (symbol is IMethodSymbol methodSymbol &&
                            methodSymbol.ReturnType.OriginalDefinition is INamedTypeSymbol typeToCreate)
                        {
                            Generate(sourceBuilder, typeToCreate, compilation);
                        }
                    }
                }

            }
            //return sourceBuilder.ToString();
            return stub;
        }

        private static void Generate(StringBuilder sourceBuilder, INamedTypeSymbol typeToCreate, Compilation compilation)
        {
            var realType = typeToCreate.IsAbstract ? FindImplementation(typeToCreate, compilation) : typeToCreate;

            sourceBuilder.AppendLine("// Generating: " + realType);

            var constructor = realType?.Constructors.FirstOrDefault();
            if (constructor != null)
            {
                foreach (var parametr in constructor.Parameters)
                {
                    if (parametr.Type is INamedTypeSymbol paramType)
                    {
                        Generate(sourceBuilder, paramType, compilation);
                    }
                }
            }
        }

        private static INamedTypeSymbol? FindImplementation(INamedTypeSymbol typeToCreate, Compilation compilation)
        {
            foreach (var x in GetAllTypes(compilation.GlobalNamespace.GetNamespaceMembers()))
            {
                if (x.Interfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, typeToCreate)))
                {
                    return x;
                }
            }
            return null;

            IEnumerable<INamedTypeSymbol> GetAllTypes(IEnumerable<INamespaceSymbol> namespaces)
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
}
