// <copyright file="FoundWordGroup.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.ViewModels;

using Baffleword.Core.Models;

/// <summary>
/// Groups found words by word length for the in-game word list.
/// </summary>
public sealed class FoundWordGroup
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FoundWordGroup"/> class.
    /// </summary>
    /// <param name="length">The word length.</param>
    /// <param name="words">The words in the group.</param>
    public FoundWordGroup(int length, IReadOnlyList<WordResult> words)
    {
        Length = length;
        Words = words ?? throw new ArgumentNullException(nameof(words));
    }

    /// <summary>
    /// Gets the word length.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets the display header.
    /// </summary>
    public string HeaderText => $"{Length} Letters({ItemCount})";

    /// <summary>
    /// Gets the number of words in the group.
    /// </summary>
    public int ItemCount => Words.Count;

    /// <summary>
    /// Gets the words in the group.
    /// </summary>
    public IReadOnlyList<WordResult> Words { get; }
}
