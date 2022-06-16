namespace Dictor.Generator.Extensions;

internal static class SymbolExtensions
{
    public static bool HasKnownAttribute(this ISymbol symbol, string qualifiedName)
    {
        return symbol.GetAttributes().Select(
            attribute => string.Equals(
                attribute.AttributeClass?.ToDisplayString(),
                qualifiedName,
                StringComparison.Ordinal
            )
        ).FirstOrDefault();
    }
}
