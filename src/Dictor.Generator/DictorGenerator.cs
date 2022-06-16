using System.Text;
using Dictor.Generator.Extensions;
using static Dictor.Generator.DiagnosticDescriptors;

namespace Dictor.Generator;

[Generator]
public class DictorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(
            static context => context.AddSource("Attributes.cs", Resources.GetAttributesFileContent())
        );

        // TODO: Add '#nullable enable' and '#pragma warning disable' to generated source
        // TODO: Be able to change constructor visibility via attribute
        // TODO: Change equality comparer of ClassInfo and FieldInfo
        // TODO: Create proper SymbolDisplayFormat to display FullyQualifiedName
        // TODO: Format name of constructor parameters to cameCase ( instead of using _fieldName, use fieldName )
        // TODO: Include type parameters on class name ( Change SymbolDisplayFormat )
        // TODO: Show diagnostic if constructor already exists

        // TODO: Use ForAttributeWithMetadataName whenever it is available
        var candidateClasses = context.SyntaxProvider.CreateSyntaxProvider(
            static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0, },
            static (context, cancellationToken) =>
            {
                var classNode = (ClassDeclarationSyntax)context.Node;
                
                return(Node: classNode, Symbol: context.SemanticModel.GetDeclaredSymbol(classNode, cancellationToken)!);
            }
        );

        var decoratedClasses = candidateClasses
            .Where(@class => @class.Symbol.HasKnownAttribute(typeof(DiCtorAttribute).FullName))
            .Select((@class, _) => (IsPartial: @class.Node.Modifiers.Any(SyntaxKind.PartialKeyword), @class.Symbol));

        var classInfoWithDiagnostics = decoratedClasses.Select(
            static (@class, _) => (ClassInfo: GetClassInfo(@class, out var diagnostics), Diagnostics: diagnostics)
        );

        context.RegisterSourceOutput(
            classInfoWithDiagnostics.Select(static (x, _) => x.Diagnostics),
            static (context, diagnostics) =>
            {
                foreach(var diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }
            }
        );

        context.RegisterSourceOutput(
            classInfoWithDiagnostics.Where(static x => x.ClassInfo is not null).Select(static (x, _) => x.ClassInfo),
            static (context, classInfo) =>
            {
                context.AddSource($"{classInfo!.Name}.g.cs", SyntaxBuilder.From(classInfo).GetText(Encoding.UTF8));
            }
        );
    }

    private static ClassInfo? GetClassInfo(
        (bool IsPartial, INamedTypeSymbol Symbol) decoratedClass,
        out ImmutableArray<Diagnostic> diagnostics)
    {
        var diagnosticBuilder = ImmutableArray.CreateBuilder<Diagnostic>();
        var (isPartialClass, classSymbol) = decoratedClass;

        if(!isPartialClass)
        {
            diagnosticBuilder.Add(
                ClassHasToBePartial.CreateDiagnostic(
                    classSymbol,
                    classSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                )
            );

            goto FoundDiagnostics;
        }

        var namespaceSymbol = classSymbol.ContainingNamespace;
        var @namespace = namespaceSymbol.IsGlobalNamespace ? "" : namespaceSymbol.ToDisplayString();

        diagnostics = diagnosticBuilder.ToImmutable();

        return new ClassInfo(
            @namespace,
            classSymbol.Name,
            GetTypeParametersInfo(classSymbol),
            GetFieldsInfo(classSymbol)
        );

        FoundDiagnostics:
        diagnostics = diagnosticBuilder.ToImmutable();

        return null;
    }

    private static ImmutableArray<string> GetTypeParametersInfo(INamedTypeSymbol classSymbol)
    {
        var typeParameters = ImmutableArray.CreateBuilder<string>();

        foreach(var typeParameterSymbol in classSymbol.TypeParameters)
        {
            typeParameters.Add(typeParameterSymbol.Name);
        }

        return typeParameters.ToImmutable();
    }

    private static ImmutableArray<FieldInfo> GetFieldsInfo(INamedTypeSymbol typeSymbol)
    {
        var fields = ImmutableArray.CreateBuilder<FieldInfo>();

        foreach(var member in typeSymbol.GetMembers())
        {
            if(member is IFieldSymbol { IsReadOnly: true, IsStatic: false, IsImplicitlyDeclared: false, } fieldSymbol)
            {
                fields.Add(
                    new FieldInfo(
                        fieldSymbol.Name,
                        fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                    )
                );
            }
        }

        return fields.ToImmutable();
    }
}
