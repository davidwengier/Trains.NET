using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Reflection;
using Trains.NET.SourceGenerator;

namespace SourceGeneratorTestBed
{
    internal class Program
    {
        private static void Main()
        {
            string source = @"
namespace Trains.NET
{
    public class Foo
    {
        public Foo()
        {
        }

        public void Method()
        {
            var x = DI.ServiceLocator.GetService<Foo>();
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

            var compilation = CSharpCompilation.Create("foo", new SyntaxTree[] { syntaxTree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            Console.WriteLine(DISourceGenerator.Generate(compilation));
        }
    }
}
