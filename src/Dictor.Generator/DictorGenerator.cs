using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Dictor.Generator.Diagnostics;
using Dictor.Generator.Models;
using static Dictor.Generator.Diagnostics.DiagnosticDescriptors;

namespace Dictor.Generator;

[Generator]
public class DictorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // TODO: Change equality comparer of GeneratedClassInfo

        var decoratedClasses = context.SyntaxProvider.CreateSyntaxProvider(
            // TODO: Use ForAttributeWithMetadataName whenever it is available
            SyntaxChecker.IsDecoratedClass,
            static (context, cancellationToken) =>
            {
                var classNode = (ClassDeclarationSyntax)context.Node;

                return(IsPartial: classNode.Modifiers.Any(SyntaxKind.PartialKeyword),
                       Symbol: context.SemanticModel.GetDeclaredSymbol(classNode, cancellationToken)!);
            }
        );

        var partialClasses = decoratedClasses.Where(static x => x.IsPartial).Select(static (x, _) => x.Symbol);
        var nonPartialClasses = decoratedClasses.Where(static x => !x.IsPartial).Select(static (x, _) => x.Symbol);

        context.RegisterSourceOutput(
            nonPartialClasses.Select(
                static (classSymbol, _) => ClassHasToBePartial.CreateDiagnostic(
                    classSymbol,
                    classSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                )
            ),
            static (context, diagnostic) => context.ReportDiagnostic(diagnostic)
        );

        var generatedClassesInfo = partialClasses.Select(
            static (classSymbol, _) =>
            {
                var namespaceSymbol = classSymbol.ContainingNamespace;
                var @namespace = namespaceSymbol.IsGlobalNamespace ? "" : namespaceSymbol.ToDisplayString();

                return new ClassInfo(
                    @namespace,
                    classSymbol.Name,
                    GetTypeParametersInfo(classSymbol),
                    GetFieldsInfo(classSymbol)
                );
            }
        );

        context.RegisterSourceOutput(
            generatedClassesInfo,
            static (context, generatedClassInfo) =>
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var (@namespace, name, genericTypes, fields) = generatedClassInfo;

                // TODO: Show diagnostic if constructor already exists
                // TODO: Be able to change constructor visibility
                // TODO: Format name of constructor parameters to cameCase ( instead of using _fieldName, use fieldName )

                var newLine = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\r\n" : "\n";
                const string TAB = "    ";

                // TODO: I want to probably refactor this to use a CompilationUnit
                var source = $@"{(@namespace != "" ? $"namespace {@namespace};" : "")}

[global::System.CodeDom.Compiler.GeneratedCode(""{nameof(DictorGenerator)}"", ""{RuntimeInformation.FrameworkDescription}"")]
partial class {name}{(genericTypes.Length > 0 ? $"<{string.Join(", ", genericTypes)}>" : "")}
{{
    public {name}(
        {string.Join($",{newLine}{TAB}{TAB}", fields.Select(x => $"global::{x.Type} {x.Name}"))})
    {{
        {string.Join($"{newLine}{TAB}{TAB}", fields.Select(x => $"this.{x.Name} = {x.Name};"))}
    }}
}}
";

                context.AddSource($"{name}.g.cs", source);
            }
        );

        context.RegisterPostInitializationOutput(
            static context => context.AddSource("Attributes.cs", Utils.GetAttributesFileContent())
        );
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
            if(member is IFieldSymbol
               {
                   IsReadOnly: true, IsStatic: false, IsImplicitlyDeclared: false,
               } fieldSymbol)
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
