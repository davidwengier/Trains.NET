using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Trains.NET.SourceGenerator;

[Generator]
public sealed class SvgSourceGenerator : IIncrementalGenerator
{
    private const string GenerateSkiaPictureMetadata = "build_metadata.AdditionalFiles.GenerateSkiaPicture";
    private const string NamespaceNameMetadata = "build_metadata.AdditionalFiles.NamespaceName";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var svgFiles = context.AdditionalTextsProvider
            .Where(static file => string.Equals(Path.GetExtension(file.Path), ".svg", StringComparison.OrdinalIgnoreCase))
            .Combine(context.AnalyzerConfigOptionsProvider);

        context.RegisterSourceOutput(svgFiles, static (sourceContext, item) =>
        {
            var file = item.Left;
            var options = item.Right.GetOptions(file);
            Generate(sourceContext, file, options);
        });
    }

    private static void Generate(SourceProductionContext context, AdditionalText file, AnalyzerConfigOptions options)
    {
        if (!options.TryGetValue(GenerateSkiaPictureMetadata, out var generate) ||
            !string.Equals(generate, bool.TrueString, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var text = file.GetText(context.CancellationToken);
        if (text is null)
        {
            ReportError(context, file, "the file could not be read");
            return;
        }

        if (!options.TryGetValue(NamespaceNameMetadata, out var namespaceName) ||
            string.IsNullOrWhiteSpace(namespaceName))
        {
            ReportError(context, file, "NamespaceName metadata is required");
            return;
        }

        namespaceName = namespaceName.Trim();
        if (!IsValidNamespace(namespaceName))
        {
            ReportError(context, file, $"'{namespaceName}' is not a valid namespace");
            return;
        }

        var className = $"Svg_{Path.GetFileNameWithoutExtension(file.Path).Replace('-', '_')}";
        if (!SyntaxFacts.IsValidIdentifier(className))
        {
            ReportError(context, file, $"'{className}' is not a valid class name");
            return;
        }

        XDocument document;
        try
        {
            using var stringReader = new StringReader(text.ToString());
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
            };
            using var reader = XmlReader.Create(stringReader, settings);
            document = XDocument.Load(reader, LoadOptions.None);
        }
        catch (XmlException exception)
        {
            ReportError(context, file, exception.Message);
            return;
        }

        var root = document.Root;
        if (root is null || root.Name.LocalName != "svg")
        {
            ReportError(context, file, "the root element must be <svg>");
            return;
        }

        var unsupportedRootAttribute = root.Attributes()
            .FirstOrDefault(static attribute => !attribute.IsNamespaceDeclaration && attribute.Name.LocalName != "viewBox");
        if (unsupportedRootAttribute is not null)
        {
            ReportError(context, file, $"the <svg> attribute '{unsupportedRootAttribute.Name.LocalName}' is not supported");
            return;
        }

        var unsupportedElement = root.Descendants()
            .FirstOrDefault(static element => element.Name.LocalName != "path");
        if (unsupportedElement is not null)
        {
            ReportError(context, file, $"the <{unsupportedElement.Name.LocalName}> element is not supported");
            return;
        }

        var paths = root.Descendants()
            .Where(static element => element.Name.LocalName == "path")
            .ToArray();
        if (paths.Length != 1)
        {
            ReportError(context, file, "exactly one <path> element is required");
            return;
        }

        var path = paths[0];
        var unsupportedPathAttribute = path.Attributes()
            .FirstOrDefault(static attribute => attribute.Name.LocalName != "d");
        if (unsupportedPathAttribute is not null)
        {
            ReportError(context, file, $"the <path> attribute '{unsupportedPathAttribute.Name.LocalName}' is not supported");
            return;
        }

        var pathDataAttribute = path.Attribute("d");
        if (pathDataAttribute is null || string.IsNullOrWhiteSpace(pathDataAttribute.Value))
        {
            ReportError(context, file, "the <path> element must have non-empty path data");
            return;
        }
        var pathData = pathDataAttribute.Value;

        if (!TryParseViewBox(root.Attribute("viewBox")?.Value, out var left, out var top, out var right, out var bottom))
        {
            ReportError(context, file, "viewBox must contain four finite numbers with positive width and height");
            return;
        }

        var source = GenerateSource(namespaceName, className, pathData, left, top, right, bottom);
        context.AddSource($"{className}.svg.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static bool IsValidNamespace(string namespaceName)
        => namespaceName.Split('.').All(SyntaxFacts.IsValidIdentifier);

    private static bool TryParseViewBox(
        string? value,
        out float left,
        out float top,
        out float right,
        out float bottom)
    {
        left = 0;
        top = 0;
        right = 0;
        bottom = 0;

        if (value is null)
        {
            return false;
        }

        var values = value.Split(new[] { ' ', '\t', '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (values.Length != 4 ||
            !TryParseFinite(values[0], out left) ||
            !TryParseFinite(values[1], out top) ||
            !TryParseFinite(values[2], out var width) ||
            !TryParseFinite(values[3], out var height) ||
            width <= 0 ||
            height <= 0)
        {
            return false;
        }

        right = left + width;
        bottom = top + height;
        return IsFinite(right) && IsFinite(bottom);
    }

    private static bool TryParseFinite(string value, out float result)
        => float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result) && IsFinite(result);

    private static bool IsFinite(float value)
        => !float.IsNaN(value) && !float.IsInfinity(value);

    private static string GenerateSource(
        string namespaceName,
        string className,
        string pathData,
        float left,
        float top,
        float right,
        float bottom)
    {
        var pathDataLiteral = Microsoft.CodeAnalysis.CSharp.SymbolDisplay.FormatLiteral(pathData, quote: true);
        return $$"""
            // <auto-generated />
            namespace {{namespaceName}};

            public static class {{className}}
            {
                public static global::SkiaSharp.SKPicture Picture { get; } = CreatePicture();

                private static global::SkiaSharp.SKPicture CreatePicture()
                {
                    using var recorder = new global::SkiaSharp.SKPictureRecorder();
                    using global::SkiaSharp.SKCanvas canvas = recorder.BeginRecording(new global::SkiaSharp.SKRect({{FormatFloat(left)}}, {{FormatFloat(top)}}, {{FormatFloat(right)}}, {{FormatFloat(bottom)}}));
                    using global::SkiaSharp.SKPath path = global::SkiaSharp.SKPath.ParseSvgPathData({{pathDataLiteral}});
                    using var paint = new global::SkiaSharp.SKPaint { IsAntialias = true };
                    canvas.DrawPath(path, paint);
                    return recorder.EndRecording();
                }
            }
            """;
    }

    private static string FormatFloat(float value)
        => value == 0 ? "0f" : value.ToString("R", CultureInfo.InvariantCulture) + "f";

    private static void ReportError(SourceProductionContext context, AdditionalText file, string message)
    {
        var position = new LinePosition(0, 0);
        var location = Location.Create(
            file.Path,
            new TextSpan(0, 0),
            new LinePositionSpan(position, position));
        context.ReportDiagnostic(Diagnostic.Create(
            "TRAINS4",
            "SVG",
            $"Cannot generate '{Path.GetFileName(file.Path)}': {message}",
            DiagnosticSeverity.Error,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            warningLevel: 0,
            isSuppressed: false,
            location: location));
    }
}
