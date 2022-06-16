using System.Runtime.InteropServices;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dictor.Generator.Helpers;

internal static class SyntaxBuilder
{
    public static CompilationUnitSyntax From(ClassInfo classInfo)
    {
        // TODO: Format the code
        
        var classAttributeLists = AttributeList(SingletonSeparatedList(
            Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                .AddArgumentListArguments(
                    AttributeArgument(
                        LiteralExpression(SyntaxKind.StringLiteralExpression,
                        Literal(nameof(DictorGenerator)))),
                    AttributeArgument(
                        LiteralExpression(SyntaxKind.StringLiteralExpression,
                        Literal(RuntimeInformation.FrameworkDescription))))));
        
        var classTypeParameterList = classInfo.TypeParameters.IsEmpty ? null :
            TypeParameterList(SeparatedList(
                classInfo.TypeParameters.Select(
                    typeParameter => TypeParameter(Identifier(typeParameter)))));

        var constructorDeclaration = ConstructorDeclaration(classInfo.Name)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .WithParameterList(ParameterList(SeparatedList(
                classInfo.Fields.Select(static field => 
                    Parameter(Identifier(field.Name))
                        .WithType(IdentifierName($"global::{field.Type}"))))))
            .WithBody(Block(
                classInfo.Fields.Select(static field => 
                    ExpressionStatement(AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ThisExpression(),
                            IdentifierName(field.Name)),
                        IdentifierName(field.Name))))));
        
        var classDeclaration = ClassDeclaration(classInfo.Name)
            .AddAttributeLists(classAttributeLists)
            .WithTypeParameterList(classTypeParameterList)
            .AddModifiers(Token(SyntaxKind.PartialKeyword))
            .AddMembers(constructorDeclaration);

        if(classInfo.Namespace is "")
        {
            return CompilationUnit()
                .AddMembers(classDeclaration)
                .NormalizeWhitespace();
        }

        // TODO: Should I support lower csharp versions, and use NamespaceDeclaration?
        var namespaceSyntax = FileScopedNamespaceDeclaration(IdentifierName(classInfo.Namespace))
            .AddMembers(classDeclaration);

        return CompilationUnit()
            .AddMembers(namespaceSyntax)
            .NormalizeWhitespace();
    }
}
