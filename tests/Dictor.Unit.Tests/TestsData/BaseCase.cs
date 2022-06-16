using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Dictor.Unit.Tests.TestsData;

public abstract class BaseCase
{
    protected const string TEST_NAMESPACE_NAME = "DictorGeneratorTests";

    public virtual string TestClassName => nameof(BaseCase);
    
    protected virtual string AttributeName => nameof(DiCtorAttribute);

    public SourceText SourceCode => this.SourceCompilationUnit.GetText(Encoding.UTF8);
    
#pragma warning disable CS0649
    private CompilationUnitSyntax? _sourceCompilationUnit;
#pragma warning restore CS0649
    protected virtual CompilationUnitSyntax SourceCompilationUnit
    {
        get
        {
            if(_sourceCompilationUnit is not null) return _sourceCompilationUnit;
            
            var compilationUnit = CompilationUnit().WithUsings(this.Usings);
            var classSyntax = this.SourceClass.WithMembers(this.FieldsMembers);

            return this.Namespace == null
                ? compilationUnit.AddMembers(classSyntax).NormalizeWhitespace()
                : compilationUnit.AddMembers(this.Namespace!.AddMembers(classSyntax)).NormalizeWhitespace();
        }
    }

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
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.PartialKeyword));
    
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
