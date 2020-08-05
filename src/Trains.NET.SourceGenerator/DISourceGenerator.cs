using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
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

            var sourceBuilder = new StringBuilder();

            foreach (var tree in compilation.SyntaxTrees)
            {
                var root = tree.GetRoot();
                var attributes = root.DescendantNodesAndSelf().OfType<AttributeSyntax>();

                foreach (var att in attributes)
                {
                    if (att.Name.ToString() == "EntryPoint")
                    {
                        var model = compilation.GetSemanticModel(tree);

                        ClassDeclarationSyntax? classSyntax = att.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                        if (classSyntax == null)
                        {
                            continue;
                        }

                        if (!(model.GetDeclaredSymbol(classSyntax) is INamedTypeSymbol classDecl))
                        {
                            continue;
                        }

                        sourceBuilder.AppendLine($@"
namespace {classDecl.ContainingNamespace}
{{
    public static class Services
    {{
        public static {classDecl.Name} GetEntryPoint()
        {{
            return null;
        }}
    }}
}}
");

                    }
                }
            }

            //File.WriteAllText(@"C:\Users\dawengie\Desktop\SourceGenerator.txt", sourceBuilder.ToString(), Encoding.UTF8);

            context.AddSource("digenerator.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }
    }
}
