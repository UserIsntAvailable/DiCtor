using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dictor.Unit.Tests.TestsData;

public abstract class BaseCase
{
    protected const string TEST_NAMESPACE_NAME = "DictorGeneratorTests";

    public SourceText SourceCode =>
        BuildCompilationUnit(this.SourceCompilationUnit, this.Namespace, this.SourceClass).GetText(Encoding.UTF8);

    public virtual string TestClassName => nameof(BaseCase);

    protected static CompilationUnitSyntax BuildCompilationUnit(
        CompilationUnitSyntax compilationSyntax,
        BaseNamespaceDeclarationSyntax? namespaceSyntax,
        ClassDeclarationSyntax classSyntax)
    {
        return namespaceSyntax == null
            ? compilationSyntax.AddMembers(classSyntax).NormalizeWhitespace()
            : compilationSyntax.AddMembers(namespaceSyntax.AddMembers(classSyntax)).NormalizeWhitespace();
    }
    
    protected virtual string AttributeName => nameof(DiCtorAttribute);

    private CompilationUnitSyntax? _sourceCompilationUnit;
    protected virtual CompilationUnitSyntax SourceCompilationUnit =>
        _sourceCompilationUnit ??= CompilationUnit().WithUsings(this.Usings);

    private IEnumerable<Type>? _fieldTypes;
    protected virtual IEnumerable<Type>? FieldTypes => _fieldTypes ??=
        new[] { typeof(IComparable), typeof(IFormattable), };

    private IEnumerable<(string, string, string)>? _fieldInfo;
    protected virtual IEnumerable<(string Name, string Type, string Using)>? Fields => _fieldInfo ??=
        this.FieldTypes?.Select(
            field =>
            {
                var fieldName = $"_{field.Name.ToLowerInvariant()}";

                var fieldFullName = field.FullName!;
                var lastPoint = fieldFullName.LastIndexOf(".", StringComparison.Ordinal);

                var fieldUsing = fieldFullName[..lastPoint];
                var fieldType = fieldFullName[(lastPoint + 1)..];

                return(fieldName, fieldType, fieldUsing);
            }
        );

    private FileScopedNamespaceDeclarationSyntax? _namespace;
    protected virtual FileScopedNamespaceDeclarationSyntax? Namespace => _namespace ??=
        FileScopedNamespaceDeclaration(IdentifierName(TEST_NAMESPACE_NAME));

    private ClassDeclarationSyntax? _class;
    protected virtual ClassDeclarationSyntax SourceClass => _class ??=
        ClassDeclaration(this.TestClassName)
            .AddAttributeLists(AttributeList(SingletonSeparatedList(
                Attribute(IdentifierName(this.AttributeName)))))
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword))
            .WithMembers(this.FieldsMembers);
    
    private SyntaxList<UsingDirectiveSyntax>? _usings;
    private SyntaxList<UsingDirectiveSyntax> Usings => _usings ??= new SyntaxList<UsingDirectiveSyntax>()
        .Add(UsingDirective(IdentifierName(typeof(DiCtorAttribute).Namespace!)))
        .AddRange(this.Fields?.DistinctBy(field => field.Using)
            .Select(field => UsingDirective(IdentifierName(field.Using))) ?? Array.Empty<UsingDirectiveSyntax>()
        );

    private SyntaxList<MemberDeclarationSyntax>? _fields;
    private SyntaxList<MemberDeclarationSyntax> FieldsMembers => _fields ??= List<MemberDeclarationSyntax>(
        this.Fields?.Select(field =>
            FieldDeclaration(VariableDeclaration(IdentifierName(field.Type))
                .WithVariables(SingletonSeparatedList(VariableDeclarator(field.Name))))
                .WithModifiers(TokenList(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.ReadOnlyKeyword)))) ?? Array.Empty<FieldDeclarationSyntax>()
    );
}
