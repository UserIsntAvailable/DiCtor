using System.Collections.Immutable;

namespace Dictor.Generator;

internal static class IncrementalValueProviderExtensions
{
    // TODO: Can I optimize this?
    public static (IncrementalValuesProvider<TSource> Left, IncrementalValuesProvider<TSource> Right)
        WhereSplit<TSource>(this IncrementalValuesProvider<TSource> source, Func<TSource, bool> predicate)
    {
        var left = source.SelectMany(
            (item, _) => predicate(item) ? ImmutableArray.Create(item) : ImmutableArray<TSource>.Empty
        );

        var right = source.SelectMany(
            (item, _) => !predicate(item) ? ImmutableArray.Create(item) : ImmutableArray<TSource>.Empty
        );

        return(left, right);
    }
}
