namespace Dictor.Generator.Diagnostics;

internal static class DiagnosticExtensions
{
    public static Diagnostic CreateDiagnostic(
        this DiagnosticDescriptor descriptor,
        ISymbol symbol,
        params object[] args)
    {
        return Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), args);
    }
}
