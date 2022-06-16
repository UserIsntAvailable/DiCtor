﻿using System.Collections;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dictor.Unit.Tests.TestsData;

internal class GeneratedCodeData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new NormalCase(), };
        yield return new object[] { new ShortAttributeName(), };
        yield return new object[] { new GlobalNamespaceCase(), };
        yield return new object[] { new GenericCase(), };
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public abstract class GeneratedCodeCase : BaseCase
{
    public SourceText GeneratedCode => BuildCompilationUnit(
        this.GeneratedCompilationUnit,
        this.Namespace?.WithLeadingTrivia(this.Trivia),
        this.Namespace != null ? this.GeneratedClass : this.GeneratedClass.WithLeadingTrivia(this.Trivia)
    ).GetText(Encoding.UTF8);

    private CompilationUnitSyntax? _generatedCompilationUnit;
    protected virtual CompilationUnitSyntax GeneratedCompilationUnit => _generatedCompilationUnit ??= CompilationUnit();
    
    private readonly AssemblyName _assemblyName = typeof(DictorGenerator).Assembly.GetName();

    private ClassDeclarationSyntax? _class;
    protected virtual ClassDeclarationSyntax GeneratedClass => _class ??=
        ClassDeclaration(this.TestClassName)
            .AddAttributeLists(AttributeList(SingletonSeparatedList(
                Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                .AddArgumentListArguments(
                    AttributeArgument(
                        LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(_assemblyName.Name!))),
                    AttributeArgument(
                        LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            Literal(_assemblyName.Version!.ToString())))))))
            .AddModifiers(Token(SyntaxKind.PartialKeyword))
            .AddMembers(this.Constructor);
    
    private ConstructorDeclarationSyntax? _constructor;
    protected virtual ConstructorDeclarationSyntax Constructor => _constructor ??=
        ConstructorDeclaration(this.TestClassName)
            .AddAttributeLists(AttributeList(SingletonSeparatedList(
                Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .WithParameterList(ParameterList(SeparatedList(
                this.Fields?.Select(static field => 
                    Parameter(Identifier(field.Name))
                        .WithType(IdentifierName($"global::{field.Using}.{field.Type}"))))))
            .WithBody(Block(
                this.Fields?.Select(static field => 
                    ExpressionStatement(AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ThisExpression(),
                            IdentifierName(field.Name)),
                        IdentifierName(field.Name)))) ?? Array.Empty<ExpressionStatementSyntax>()));

    private SyntaxTriviaList? _trivia;
    private SyntaxTriviaList Trivia => _trivia ??= TriviaList(
        Comment("// <auto-generated/>"),
        Trivia(NullableDirectiveTrivia(Token(SyntaxKind.EnableKeyword), isActive: true)));
}

internal class NormalCase : GeneratedCodeCase
{
    public override string TestClassName => nameof(NormalCase);
}

internal class ShortAttributeName : GeneratedCodeCase
{
    public override string TestClassName => nameof(ShortAttributeName);

    protected override string AttributeName => base.AttributeName.Replace("Attribute", "");
}

internal class GlobalNamespaceCase : GeneratedCodeCase
{
    public override string TestClassName => nameof(GlobalNamespaceCase);
    
    protected override FileScopedNamespaceDeclarationSyntax? Namespace => null;
}

internal class GenericCase : GeneratedCodeCase
{
    public override string TestClassName => nameof(GenericCase);

    private readonly TypeParameterListSyntax _classParameterSyntax = TypeParameterList(
        SeparatedList(new[] { TypeParameter(Identifier("T")), TypeParameter(Identifier("U")), })
    );

    protected override ClassDeclarationSyntax SourceClass =>
        base.SourceClass.WithTypeParameterList(_classParameterSyntax);

    protected override ClassDeclarationSyntax GeneratedClass =>
        base.GeneratedClass.WithTypeParameterList(_classParameterSyntax);
}
