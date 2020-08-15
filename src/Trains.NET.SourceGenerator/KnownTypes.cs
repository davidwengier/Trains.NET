using Microsoft.CodeAnalysis;

namespace Trains.NET.SourceGenerator
{
    internal class KnownTypes
    {
        public INamedTypeSymbol TransientAttribute;
        public INamedTypeSymbol OrderAttribute;
        public INamedTypeSymbol ILayoutOfT;
        public INamedTypeSymbol FilteredLayout;
        public INamedTypeSymbol IEnumerableOfT;
        public INamedTypeSymbol ListOfT;

        public KnownTypes(INamedTypeSymbol transientAttribute, INamedTypeSymbol orderAttribute, INamedTypeSymbol layoutOfT, INamedTypeSymbol filteredLayout, INamedTypeSymbol iEnumerableOfT, INamedTypeSymbol listOfT)
        {
            TransientAttribute = transientAttribute;
            OrderAttribute = orderAttribute;
            ILayoutOfT = layoutOfT;
            FilteredLayout = filteredLayout;
            IEnumerableOfT = iEnumerableOfT;
            ListOfT = listOfT;
        }
    }
}
