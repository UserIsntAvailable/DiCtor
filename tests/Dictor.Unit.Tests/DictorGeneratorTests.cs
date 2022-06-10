using System.Runtime.InteropServices;
using VerifyCS = Dictor.Unit.Tests.CSharpGeneratorVerifier<Dictor.Generator.DictorGenerator>;

namespace Dictor.Unit.Tests;

public class DictorGeneratorTests
{
    private const string TEST_NAMESPACE_NAME = "DictorGeneratorTests";
    private const string TEST_CLASS_NAME = "TestController";

    [Fact]
    public async void Generator_ShouldAddAttributesFile_AfterGeneratorPostInitialization()
    {
        await new VerifyCS.Test
        {
            TestState = { GeneratedSources = { Source("Attributes.cs", Utils.GetAttributesFileContent()), }, },
        }.RunAsync();
    }

    [Fact]
    public async void Generator_ShouldCreateDIConstructor_WhenClassIsAnnotatedWithDiCtorAttribute()
    {
        // TODO: Use C#11 raw string literals.
        const string CODE = $@"
using System;
using Dictor;

namespace {TEST_NAMESPACE_NAME};

[DiCtor]
public partial class {TEST_CLASS_NAME}
{{
    private readonly IComparable _comparable;
    private readonly IFormattable _formattable;
}}";

        var generated = $@"namespace {TEST_NAMESPACE_NAME};

[global::System.CodeDom.Compiler.GeneratedCode(""{nameof(DictorGenerator)}"", ""{RuntimeInformation.FrameworkDescription}"")]
partial class {TEST_CLASS_NAME}
{{
    public {TEST_CLASS_NAME}(
        global::System.IComparable _comparable,
        global::System.IFormattable _formattable)
    {{
        this._comparable = _comparable;
        this._formattable = _formattable;
    }}
}}
";

        await new VerifyCS.Test
        {
            TestState =
            {
                Sources = { CODE, },
                GeneratedSources =
                {
                    Source("Attributes.cs", Utils.GetAttributesFileContent()),
                    Source($"{TEST_CLASS_NAME}.g.cs", generated),
                },
                // ExpectedDiagnostics = {  },
            },
        }.RunAsync();
    }

    private static (Type, string, string) Source(string typeName, string source)
    {
        return(typeof(Adapter<DictorGenerator>), typeName, source);
    }
}
