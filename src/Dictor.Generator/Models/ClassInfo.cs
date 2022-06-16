using Dictor.Generator.Extensions;

namespace Dictor.Generator.Models;

internal record ClassInfo(
    string Namespace,
    string Name,
    ImmutableArray<string> TypeParameters,
    ImmutableArray<FieldInfo> Fields)
{
    public class Comparer : Comparer<ClassInfo, Comparer>
    {
        protected override void AddToHashCode(ref HashCode hashCode, ClassInfo obj)
        {
            hashCode.Add(obj.Namespace);
            hashCode.Add(obj.Name);
            hashCode.AddRange(obj.TypeParameters);
            hashCode.AddRange(obj.Fields, FieldInfo.Comparer.Default);
        }

        protected override bool AreEqual(ClassInfo x, ClassInfo y) =>
            string.Equals(x.Namespace, y.Namespace, StringComparison.Ordinal) &&
            string.Equals(x.Name, y.Name, StringComparison.Ordinal) &&
            x.TypeParameters.SequenceEqual(y.TypeParameters) &&
            x.Fields.SequenceEqual(y.Fields, FieldInfo.Comparer.Default);
    }
}
