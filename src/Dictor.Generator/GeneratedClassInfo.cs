using System.Collections.Immutable;

namespace Dictor.Generator;

internal record struct GeneratedClassInfo(
    string Namespace,
    string Name,
    ImmutableArray<string> GenericTypes,
    ImmutableArray<GeneratedFieldInfo> Fields);

#pragma warning disable MA0048
internal record struct GeneratedFieldInfo(string Name, string Type);
#pragma warning restore MA0048
