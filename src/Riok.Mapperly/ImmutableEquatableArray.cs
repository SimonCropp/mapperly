﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using Riok.Mapperly.Helpers;

namespace Riok.Mapperly;

/// <summary>
/// Provides an immutable list implementation which implements sequence equality.
/// </summary>
public sealed class ImmutableEquatableArray<T> : IEquatable<ImmutableEquatableArray<T>>, IReadOnlyList<T>
    where T : IEquatable<T>
{
    public static ImmutableEquatableArray<T> Empty { get; } = new(Array.Empty<T>());

    private readonly T[] _values;
    public T this[int index] => _values[index];
    public int Count => _values.Length;

    public ImmutableEquatableArray(IEnumerable<T> values) => _values = values.ToArray();

    public bool Equals(ImmutableEquatableArray<T>? other) => other != null && ((ReadOnlySpan<T>)_values).SequenceEqual(other._values);

    public override bool Equals(object? obj) => obj is ImmutableEquatableArray<T> other && Equals(other);

    public override int GetHashCode()
    {
        var hash = 0;
        foreach (T value in _values)
        {
            hash = HashHelper.Combine(hash, value.GetHashCode());
        }

        return hash;
    }

    public Enumerator GetEnumerator() => new Enumerator(_values);

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => ((IEnumerable<T>)_values).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _values.GetEnumerator();

    public struct Enumerator
    {
        private readonly T[] _values;
        private int _index;

        internal Enumerator(T[] values)
        {
            _values = values;
            _index = -1;
        }

        public bool MoveNext()
        {
            var newIndex = _index + 1;

            if ((uint)newIndex < (uint)_values.Length)
            {
                _index = newIndex;
                return true;
            }

            return false;
        }

        public readonly T Current => _values[_index];
    }
}

public static class ImmutableEquatableArray
{
    public static ImmutableEquatableArray<T> Empty<T>()
        where T : IEquatable<T> => ImmutableEquatableArray<T>.Empty;

    public static ImmutableEquatableArray<T> ToImmutableEquatableArray<T>(this IEnumerable<T> values)
        where T : IEquatable<T> => new(values);

    public static ImmutableEquatableArray<T> Create<T>(params T[] values)
        where T : IEquatable<T> => values is { Length: > 0 } ? new(values) : ImmutableEquatableArray<T>.Empty;
}
