namespace Dictor.Generator.Models;

internal record ClassInfo(
    string Namespace,
    string Name,
    ImmutableArray<string> TypeParameters,
    ImmutableArray<FieldInfo> Fields);
