// MIT License

// Copyright (c) 2021 Sergio Pedri

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// This file is ported and adapted from ComputeSharp (Sergio0694/ComputeSharp),
// more info in ThirdPartyNotices.txt in the root of the project.

namespace Dictor.Generator.Extensions;

/// <summary>
/// Extension methods for <see cref="HashCode"/>.
/// </summary>
internal static class HashCodeExtensions
{
    /// <summary>
    /// Adds all items from a given <see cref="ImmutableArray{T}"/> instance to an hashcode.
    /// </summary>
    /// <typeparam name="T">The type of items to hash.</typeparam>
    /// <param name="hashCode">The target <see cref="HashCode"/> instance.</param>
    /// <param name="items">The input items to hash.</param>
    public static void AddRange<T>(this ref HashCode hashCode, ImmutableArray<T> items)
    {
        foreach(var item in items)
        {
            hashCode.Add(item);
        }
    }

    /// <summary>
    /// Adds all items from a given <see cref="ImmutableArray{T}"/> instance to an hashcode.
    /// </summary>
    /// <typeparam name="T">The type of items to hash.</typeparam>
    /// <param name="hashCode">The target <see cref="HashCode"/> instance.</param>
    /// <param name="comparer">A comparer to get hashcodes for <typeparamref name="T"/> items.</param>
    /// <param name="items">The input items to hash.</param>
    public static void AddRange<T>(this ref HashCode hashCode, ImmutableArray<T> items, IEqualityComparer<T> comparer)
    {
        foreach(var item in items)
        {
            hashCode.Add(item, comparer);
        }
    }
}
