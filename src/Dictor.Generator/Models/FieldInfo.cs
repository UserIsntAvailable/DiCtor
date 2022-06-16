namespace Dictor.Generator.Models;

/// <summary>
/// x
/// </summary>
/// <param name="Name"></param>
/// <param name="Type"></param>
internal record FieldInfo(string Name, string Type)
{
    public class Comparer : Comparer<FieldInfo, Comparer>
    {
        protected override void AddToHashCode(ref HashCode hashCode, FieldInfo obj)
        {
            hashCode.Add(obj.Name);
            hashCode.Add(obj.Type);
        }

        protected override bool AreEqual(FieldInfo x, FieldInfo y) =>
            string.Equals(x.Name, y.Name, StringComparison.Ordinal) &&
            string.Equals(x.Type, y.Type, StringComparison.Ordinal);
    }
}

