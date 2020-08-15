using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Reflection;
using Trains.NET.SourceGenerator;
using System.Linq;

namespace SourceGeneratorTestBed
{
    public class Program
    {
        private static void Main()
        {
            string source = @"
namespace Trains.NET
{
    public class Foo
    {
        public void Method()
        {
            var x = DI.ServiceLocator.GetService<Trains.NET.Comet.MainPage>();
        }
    }
}
";
            var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(kind: SourceCodeKind.Regular));

            var references = new List<MetadataReference>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic)
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            references.Add(MetadataReference.CreateFromFile(typeof(Trains.NET.Rendering.IGame).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(Trains.NET.Rendering.Skia.RenderingExtensions).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(Trains.NET.Engine.ContextAttribute).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(Trains.NET.Comet.ITrainController).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(Trains.App).Assembly.Location));

            var compilation = CSharpCompilation.Create("foo", new SyntaxTree[] { syntaxTree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var diagnostics = compilation.GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Where(d => d.Descriptor.Description.ToString() == "The name 'DI' does not exist in the current context");
            if (diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Errors:");
                Console.ResetColor();
                Console.WriteLine(string.Join("\n", diagnostics));

                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Output:");
            Console.ResetColor();

            source = DISourceGenerator.Generate(compilation);
            Console.WriteLine(source);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Compilation Results:");
            Console.ResetColor();

            syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(kind: SourceCodeKind.Regular));
            compilation = compilation.AddSyntaxTrees(syntaxTree);

            diagnostics = compilation.GetDiagnostics();
            Console.WriteLine(string.Join("\n", diagnostics));
        }
    }
}
