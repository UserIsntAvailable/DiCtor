using System.Collections;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Testing;
using static Dictor.Generator.DiagnosticDescriptors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

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
    public abstract DiagnosticResult[] ExpectedDiagnostics { get; }
}

public class ClassHasToBePartialCase : DiagnosticCase
{
    public override string TestClassName => nameof(ClassHasToBePartialCase);

    protected override ClassDeclarationSyntax SourceClass =>
        base.SourceClass.WithModifiers(new SyntaxTokenList(Token(SyntaxKind.PublicKeyword)));

    public override DiagnosticResult[] ExpectedDiagnostics => new[]
    {
        new DiagnosticResult(ClassHasToBePartial)
            .WithArguments($"{TEST_NAMESPACE_NAME}.{this.TestClassName}"),
    };
}
