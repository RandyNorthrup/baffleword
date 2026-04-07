// <copyright file="IDictionaryProvider.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Dictionary;

/// <summary>
/// Provides word lookup and prefix checking for the Boggle word list.
/// </summary>
public interface IDictionaryProvider
{
    /// <summary>
    /// Gets the total number of words in the dictionary.
    /// </summary>
    int WordCount { get; }

    /// <summary>
    /// Checks whether a word exists in the dictionary.
    /// </summary>
    /// <param name="word">The word to look up (uppercase).</param>
    /// <returns><see langword="true"/> if the word is valid; otherwise, <see langword="false"/>.</returns>
    bool IsValidWord(string word);

    /// <summary>
    /// Checks whether any word in the dictionary starts with the given prefix.
    /// </summary>
    /// <param name="prefix">The prefix to check (uppercase).</param>
    /// <returns><see langword="true"/> if any word starts with this prefix; otherwise, <see langword="false"/>.</returns>
    bool HasPrefix(string prefix);
}
