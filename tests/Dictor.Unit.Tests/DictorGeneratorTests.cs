using Dictor.Unit.Tests.TestsData;
using Microsoft.CodeAnalysis.Testing;
using VerifyCS = Dictor.Unit.Tests.CSharpGeneratorVerifier<Dictor.Generator.DictorGenerator>;

namespace Dictor.Unit.Tests;

public class DictorGeneratorTests
{
    [Fact]
    public async void Generator_ShouldAddAttributesFile_AfterGeneratorPostInitialization()
    {
        await new VerifyCS.Test
        {
            TestState = { GeneratedSources = { Source("Attributes.cs", Utils.GetAttributesFileContent()), }, },
        }.RunAsync();
    }

    [Theory]
    [ClassData(typeof(GeneratedCodeData))]
    public async void Generator_ShouldCreateDIConstructor_WhenClassIsAnnotatedWithDiCtorAttribute(
        GeneratedCodeCase data)
    {
        await new VerifyCS.Test
        {
            TestState =
            {
                Sources = { data.SourceCode, },
                GeneratedSources =
                {
                    Source("Attributes.cs", Utils.GetAttributesFileContent()),
                    Source($"{data.TestClassName}.g.cs", data.GeneratedCode),
                },
            },
        }.RunAsync();
    }

    [Theory]
    [ClassData(typeof(DiagnosticData))]
    public async void Generator_ShouldSkipGeneratedCode_AndCheckForCodeDiagnostics(DiagnosticCase data)
    {
        var test = new VerifyCS.Test
        {
            TestState = { Sources = { data.SourceCode, }, },
            TestBehaviors = TestBehaviors.SkipGeneratedSourcesCheck,
        };

        test.ExpectedDiagnostics.AddRange(data.ExpectedDiagnostics);
        
        await test.RunAsync();
    }

    private static (Type, string, string) Source(string typeName, string source)
    {
        return(typeof(Adapter<DictorGenerator>), typeName, source);
    }
}
