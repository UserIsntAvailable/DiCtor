using VerifyCS = Dictor.Unit.Tests.CSharpGeneratorVerifier<Dictor.Generator.DictorGenerator>;

namespace Dictor.Unit.Tests;

public class DictorGeneratorTests
{
    public const string TEST_CLASS_NAME = "TestController";

    [Fact]
    public async void Generator_ShouldAddAttributesFile_AfterGeneratorPostInitialization()
    {
        await new VerifyCS.Test
        {
            TestState = { GeneratedSources = { Source("Attributes.cs", Utils.GetAttributesFileContent()), }, },
        }.RunAsync();
    }

    [Theory]
    [ClassData(typeof(TestData))]
    public async void Generator_ShouldCreateDIConstructor_WhenClassIsAnnotatedWithDiCtorAttribute(ICase data)
    {
        await new VerifyCS.Test
        {
            TestState =
            {
                Sources = { data.SourceCode, },
                GeneratedSources =
                {
                    Source("Attributes.cs", Utils.GetAttributesFileContent()),
                    Source($"{TEST_CLASS_NAME}.g.cs", data.GeneratedCode),
                },
            },
        }.RunAsync();
    }

    private static (Type, string, string) Source(string typeName, string source)
    {
        return(typeof(Adapter<DictorGenerator>), typeName, source);
    }
}
