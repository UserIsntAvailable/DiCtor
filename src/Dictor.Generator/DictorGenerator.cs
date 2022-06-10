using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace Dictor.Generator;

[Generator]
public class DictorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var decoratedClasses = context.SyntaxProvider.CreateSyntaxProvider(
            SyntaxChecker.IsDecoratedClass,
            static (context, cancellationToken) =>
            {
                var classNode = (ClassDeclarationSyntax)context.Node;

                return(Node: classNode,
                       Symbol: (ITypeSymbol)context.SemanticModel.GetDeclaredSymbol(classNode, cancellationToken)!);
            }
        );

        var (partialClasses, nonPartialClasses) = decoratedClasses.WhereSplit(
            static decoratedClass => decoratedClass.Node.Modifiers.Any(SyntaxKind.PartialKeyword)
        );

        /*
         * Features:
         *  Include/Exclude Attribute
         *  Properties
         */

        var generatedClassesInfo = partialClasses.Select(
            static (partialClass, _) =>
            {
                var (_, classSymbol) = partialClass;

                var namespaceSymbol = classSymbol.ContainingNamespace;
                var @namespace = namespaceSymbol.IsGlobalNamespace ? "" : namespaceSymbol.ToDisplayString();

                return new GeneratedClassInfo(@namespace, classSymbol.Name, GetFieldsInfo(classSymbol));
            }
        ).Collect();

        var nonPartialDiagnostics = nonPartialClasses.Select(
            static (nonPartialClass, _) =>
            {
                var (classNode, classSymbol) = nonPartialClass;

                return DiagnosticInfo.ClassHasToBePartial(
                    classNode.Identifier.GetLocation(),
                    classSymbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                );
            }
        ).Collect();

        var sourceOutputInfo = nonPartialDiagnostics.Combine(generatedClassesInfo);

        context.RegisterSourceOutput(
            sourceOutputInfo,
            static (context, sourceOutputInfo) =>
            {
                var (diagnostics, generatedClassesInfo) = sourceOutputInfo;

                foreach(var diagnostic in diagnostics)
                {
                    context.ReportDiagnostic(diagnostic);
                }

                foreach(var generatedClassInfo in generatedClassesInfo)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();

                    var (@namespace, name, fields) = generatedClassInfo;

                    // TODO: Show diagnostic if constructor already exists
                    // TODO: Be able to change constructor visibility
                    // TODO: Format name of constructor parameters to cameCase ( instead of using _fieldName, use fieldName )

                    // TODO: Need to check in Linux if this works
                    var newLine = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "\r\n" : "\n";
                    const string TAB = "    ";

                    var source = $@"namespace {@namespace};

[global::System.CodeDom.Compiler.GeneratedCode(""{nameof(DictorGenerator)}"", ""{RuntimeInformation.FrameworkDescription}"")]
partial class {name}
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
            }
        );

        context.RegisterPostInitializationOutput(
            static context => context.AddSource("Attributes.cs", Utils.GetAttributesFileContent())
        );
    }

    // TODO: Need to revisit this.
    private static ImmutableArray<GeneratedFieldInfo> GetFieldsInfo(ITypeSymbol typeSymbol)
    {
        var fields = new List<GeneratedFieldInfo>();

        foreach(var member in typeSymbol.GetMembers())
        {
            if(member is IFieldSymbol
               {
                   IsReadOnly: true, IsStatic: false, IsImplicitlyDeclared: false,
               } fieldSymbol)
            {
                fields.Add(
                    new GeneratedFieldInfo(
                        fieldSymbol.Name,
                        fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)
                    )
                );
            }
        }

        return ImmutableArray.CreateRange(fields);
    }
}
