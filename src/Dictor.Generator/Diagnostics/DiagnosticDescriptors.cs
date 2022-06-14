// ReSharper disable MemberHidesStaticFromOuterClass

namespace Dictor.Generator.Diagnostics;

internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor ClassHasToBePartial = new(
        "DCT0001",
        $"The type decorated with {nameof(DiCtorAttribute)} should have a partial modifier",
        $"The type '{{0}}' decorated with {nameof(DiCtorAttribute)} should have a partial modifier",
        "Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true
    );
}
