// ReSharper disable MemberHidesStaticFromOuterClass

namespace Dictor.Generator;

internal static class DiagnosticInfo
{
    private static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor ClassHasToBePartial = new(
            "DCT0001",
            // TODO: Improve message?
            $"The type decorated with {nameof(DiCtor)} attribute should have a partial modifier",
            $"The type '{{0}}' decorated with {nameof(DiCtor)} attribute should have a partial modifier",
            "Usage",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );
    }

    public static Diagnostic ClassHasToBePartial(Location location, string className) => Diagnostic.Create(
        DiagnosticDescriptors.ClassHasToBePartial,
        location,
        className
    );
}
