using System.Collections;
using System.Runtime.InteropServices;

namespace Dictor.Unit.Tests.TestsData;

internal class GeneratedCodeData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new NormalCase(), };
        yield return new object[] { new GlobalNamespaceCase(), };
        yield return new object[] { new GenericCase(), };
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public abstract class GeneratedCodeCase: BaseCase
{
    public abstract string GeneratedCode { get; }
}

// TODO: Use C#11 raw string literals.
internal class NormalCase : GeneratedCodeCase
{
    public override string TestClassName => nameof(NormalCase);

    public override string SourceCode => $@"using System;
using Dictor;

namespace {TEST_NAMESPACE_NAME};
                                            
[{ATTRIBUTE_NAME}]
public partial class {this.TestClassName}
{{
    public readonly IComparable _comparable;
    public readonly IFormattable _formattable;
}}";

    public override string GeneratedCode => $@"namespace {TEST_NAMESPACE_NAME};

[global::System.CodeDom.Compiler.GeneratedCode(""{nameof(DictorGenerator)}"", ""{RuntimeInformation.FrameworkDescription}"")]
partial class {this.TestClassName}
{{
    public {this.TestClassName}(
        global::System.IComparable _comparable,
        global::System.IFormattable _formattable)
    {{
        this._comparable = _comparable;
        this._formattable = _formattable;
    }}
}}
";
}

internal class GlobalNamespaceCase : GeneratedCodeCase
{
    public override string TestClassName => nameof(GlobalNamespaceCase);

    public override string SourceCode => $@"using System;
using Dictor;

[{ATTRIBUTE_NAME}]
public partial class {this.TestClassName}
{{
    public readonly IComparable _comparable;
    public readonly IFormattable _formattable;
}}";

    public override string GeneratedCode => $@"

[global::System.CodeDom.Compiler.GeneratedCode(""{nameof(DictorGenerator)}"", ""{RuntimeInformation.FrameworkDescription}"")]
partial class {this.TestClassName}
{{
    public {this.TestClassName}(
        global::System.IComparable _comparable,
        global::System.IFormattable _formattable)
    {{
        this._comparable = _comparable;
        this._formattable = _formattable;
    }}
}}
";
}

internal class GenericCase : GeneratedCodeCase
{
    public override string TestClassName => nameof(GenericCase);

    public override string SourceCode => $@"using System;
using Dictor;

namespace {TEST_NAMESPACE_NAME};
                                            
[{ATTRIBUTE_NAME}]
public partial class {this.TestClassName}<T, U>
{{
    public readonly IComparable _comparable;
    public readonly IFormattable _formattable;
}}";

    public override string GeneratedCode => $@"namespace {TEST_NAMESPACE_NAME};

[global::System.CodeDom.Compiler.GeneratedCode(""{nameof(DictorGenerator)}"", ""{RuntimeInformation.FrameworkDescription}"")]
partial class {this.TestClassName}<T, U>
{{
    public {this.TestClassName}(
        global::System.IComparable _comparable,
        global::System.IFormattable _formattable)
    {{
        this._comparable = _comparable;
        this._formattable = _formattable;
    }}
}}
";
}
