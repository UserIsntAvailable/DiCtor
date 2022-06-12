namespace Dictor.Unit.Tests.TestsData;

public abstract class BaseCase
{
    protected const string TEST_NAMESPACE_NAME = "DictorGeneratorTests";
    protected const string ATTRIBUTE_NAME = nameof(DiCtorAttribute);

    public virtual string TestClassName => nameof(BaseCase);

    public abstract string SourceCode { get; }
}
