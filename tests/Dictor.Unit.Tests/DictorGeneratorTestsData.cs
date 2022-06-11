using System.Collections;
using System.Runtime.InteropServices;

namespace Dictor.Unit.Tests;

public interface ICase
{
    string SourceCode { get; }

    string GeneratedCode { get; }
}

public class TestData : IEnumerable<object[]>
{
    private const string TEST_NAMESPACE_NAME = "DictorGeneratorTests";
    private const string TEST_CLASS_NAME = DictorGeneratorTests.TEST_CLASS_NAME;

    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new BaseCase(), };
        yield return new object[] { new GlobalNamespaceCase(), };
        yield return new object[] { new GenericCase(), };
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    // TODO: Use C#11 raw string literals.
    private readonly struct BaseCase : ICase
    {
        public string SourceCode => $@"using System;
using Dictor;

namespace {TEST_NAMESPACE_NAME};
                                            
[DiCtor]
public partial class {TEST_CLASS_NAME}
{{
    private readonly IComparable _comparable;
    private readonly IFormattable _formattable;
}}";

        public string GeneratedCode => $@"namespace {TEST_NAMESPACE_NAME};

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
    }

    private readonly struct GlobalNamespaceCase : ICase
    {
        public string SourceCode => $@"using System;
using Dictor;

[DiCtor]
public partial class {TEST_CLASS_NAME}
{{
    private readonly IComparable _comparable;
    private readonly IFormattable _formattable;
}}";

        public string GeneratedCode => $@"

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
    }

    private readonly struct GenericCase : ICase
    {
        public string SourceCode => $@"using System;
using Dictor;

namespace {TEST_NAMESPACE_NAME};
                                            
[DiCtor]
public partial class {TEST_CLASS_NAME}<T, U>
{{
    private readonly IComparable _comparable;
    private readonly IFormattable _formattable;
}}";

        public string GeneratedCode => $@"namespace {TEST_NAMESPACE_NAME};

[global::System.CodeDom.Compiler.GeneratedCode(""{nameof(DictorGenerator)}"", ""{RuntimeInformation.FrameworkDescription}"")]
partial class {TEST_CLASS_NAME}<T, U>
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
    }
}
