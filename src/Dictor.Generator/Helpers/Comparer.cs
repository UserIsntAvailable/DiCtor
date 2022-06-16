// .NET Community Toolkit
// Copyright © .NET Foundation and Contributors
// All rights reserved.

// MIT License (MIT)

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files  (the “Software”), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge,  publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

// Namespace modified to match this project
namespace Dictor.Generator.Helpers;

/// <summary>
/// A base <see cref="IEqualityComparer{T}"/> implementation for <typeparamref name="T"/> instances.
/// </summary>
/// <typeparam name="T">The type of items to compare.</typeparam>
/// <typeparam name="TSelf">The concrete comparer type.</typeparam>
internal abstract class Comparer<T, TSelf> : IEqualityComparer<T>
    where TSelf : Comparer<T, TSelf>, new()
{
    /// <summary>
    /// The singleton <typeparamref name="TSelf"/> instance.
    /// </summary>
    public static TSelf Default { get; } = new();

    /// <inheritdoc/>
    public bool Equals(T? x, T? y)
    {
        if (x is null && y is null)
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        if (ReferenceEquals(x, y))
        {
            return true;
        }

        return this.AreEqual(x, y);
    }

    /// <inheritdoc/>
    public int GetHashCode(T obj)
    {
        HashCode hashCode = default;

        this.AddToHashCode(ref hashCode, obj);

        return hashCode.ToHashCode();
    }

    /// <summary>
    /// Adds the current instance to an incremental <see cref="HashCode"/> value.
    /// </summary>
    /// <param name="hashCode">The target <see cref="HashCode"/> value.</param>
    /// <param name="obj">The <typeparamref name="T"/> instance being inspected.</param>
    protected abstract void AddToHashCode(ref HashCode hashCode, T obj);

    /// <summary>
    /// Compares two <typeparamref name="T"/> instances for equality.
    /// </summary>
    /// <param name="x">The first <typeparamref name="T"/> instance to compare.</param>
    /// <param name="y">The second <typeparamref name="T"/> instance to compare.</param>
    /// <returns>Whether or not <paramref name="x"/> and <paramref name="y"/> are equal.</returns>
    protected abstract bool AreEqual(T x, T y);
}
