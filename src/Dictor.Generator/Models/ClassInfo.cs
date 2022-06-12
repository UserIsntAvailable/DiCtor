using System.Collections.Immutable;

namespace Dictor.Generator.Models;

internal record struct ClassInfo(
    string Namespace,
    string Name,
    ImmutableArray<string> TypeParameters,
    ImmutableArray<FieldInfo> Fields);
