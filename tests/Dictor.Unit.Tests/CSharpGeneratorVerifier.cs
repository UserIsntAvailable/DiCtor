using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Testing;

namespace Dictor.Unit.Tests;

public static class CSharpGeneratorVerifier<TIncrementalGenerator>
    where TIncrementalGenerator : IIncrementalGenerator, new()
{
    public class Test : CSharpSourceGeneratorTest<Adapter<TIncrementalGenerator>,
        IgnoreExpectedDiagnosticsLocationVerifier>
    {
        protected override CompilationOptions CreateCompilationOptions()
        {
            var compilationOptions = base.CreateCompilationOptions();

            return compilationOptions.WithSpecificDiagnosticOptions(
                compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler())
            );
        }

        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Preview;

        private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
        {
            string[] args = { "/warnaserror:nullable", };
            var commandLineArguments = CSharpCommandLineParser.Default.Parse(
                args,
                baseDirectory: Environment.CurrentDirectory,
                sdkDirectory: Environment.CurrentDirectory
            );

            var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

            return nullableWarnings;
        }

        protected override ParseOptions CreateParseOptions()
        {
            return((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(this.LanguageVersion);
        }
    }
}

/// <summary>
/// My diagnostics test cases are ( will be ) really simple, I don't need to check the location ( most of the time )
/// where there will be located. If expected == Location.None, just ignore the location check.
/// </summary>
public class IgnoreExpectedDiagnosticsLocationVerifier : XUnitVerifier
{
    public IgnoreExpectedDiagnosticsLocationVerifier()
    {
    }

    private IgnoreExpectedDiagnosticsLocationVerifier(ImmutableStack<string> content)
        : base(content)
    {
    }

    public override void Equal<T>(T expected, T actual, string? message = null)
    {
        if(this.Context.Peek() == "Diagnostics of test state")
        {
            if(expected is Location { Kind: LocationKind.None, })
            {
                return;
            }
        }

        base.Equal(expected, actual, message);
    }

    public override IVerifier PushContext(string context)
    {
        return new IgnoreExpectedDiagnosticsLocationVerifier(this.Context.Push(context));
    }
}
