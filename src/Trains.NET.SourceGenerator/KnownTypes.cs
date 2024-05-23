using Microsoft.CodeAnalysis;

namespace Trains.NET.SourceGenerator;

internal class KnownTypes(
    INamedTypeSymbol transientAttribute,
    INamedTypeSymbol orderAttribute,
    INamedTypeSymbol layoutOfT,
    INamedTypeSymbol filteredLayout,
    INamedTypeSymbol iEnumerableOfT,
    INamedTypeSymbol listOfT)
{
    public INamedTypeSymbol TransientAttribute = transientAttribute;
    public INamedTypeSymbol OrderAttribute = orderAttribute;
    public INamedTypeSymbol ILayoutOfT = layoutOfT;
    public INamedTypeSymbol FilteredLayout = filteredLayout;
    public INamedTypeSymbol IEnumerableOfT = iEnumerableOfT;
    public INamedTypeSymbol ListOfT = listOfT;
}
