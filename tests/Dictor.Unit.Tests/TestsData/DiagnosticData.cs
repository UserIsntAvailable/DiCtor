using System.Collections;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Testing;
using static Dictor.Generator.Diagnostics.DiagnosticDescriptors;

namespace Dictor.Unit.Tests.TestsData;

internal class DiagnosticData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new ClassHasToBePartialCase(), };
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public abstract class DiagnosticCase : BaseCase
{
    public abstract ImmutableArray<DiagnosticResult> ExpectedDiagnostics { get; }
}

public class ClassHasToBePartialCase : DiagnosticCase
{
    public override string SourceCode => $@"using System;
using Dictor;

namespace {TEST_NAMESPACE_NAME};
                                            
[{ATTRIBUTE_NAME}]
public class {this.TestClassName}
{{
    public readonly IComparable _comparable;
    public readonly IFormattable _formattable;
}}";

    public override ImmutableArray<DiagnosticResult> ExpectedDiagnostics
    {
        get
        {
            var diagnosticBuilder = ImmutableArray.CreateBuilder<DiagnosticResult>();
            diagnosticBuilder.Add(
                // Fix: Hardcoded for now. I need to switch to SyntaxFactory to make this easier to write and reuse.
                new DiagnosticResult(ClassHasToBePartial).WithSpan(7, 14, 7, 22)
                                                         .WithArguments("DictorGeneratorTests.BaseCase")
            );

            return diagnosticBuilder.ToImmutable();
        }
    }
}
