// <copyright file="TrieDictionaryProvider.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Dictionary;

/// <summary>
/// A Trie-based dictionary provider for fast word lookup and prefix checking.
/// </summary>
public sealed class TrieDictionaryProvider : IDictionaryProvider
{
    private readonly TrieNode _root = new();

    /// <inheritdoc/>
    public int WordCount { get; private set; }

    /// <summary>
    /// Loads words into the trie.
    /// </summary>
    /// <param name="words">The words to load.</param>
    public void LoadWords(IEnumerable<string> words)
    {
        ArgumentNullException.ThrowIfNull(words);

        foreach (string word in words)
        {
            InsertWord(word.ToUpperInvariant());
        }
    }

    /// <inheritdoc/>
    public bool IsValidWord(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return false;
        }

        TrieNode? node = FindNode(word);
        return node is not null && node.IsEndOfWord;
    }

    /// <inheritdoc/>
    public bool HasPrefix(string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            return true;
        }

        return FindNode(prefix) is not null;
    }

    private void InsertWord(string word)
    {
        TrieNode current = _root;
        foreach (char c in word)
        {
            int index = c - 'A';
            if (index < 0 || index >= 26)
            {
                return; // Skip words with non-letter characters
            }

            current.Children[index] ??= new TrieNode();
            current = current.Children[index]!;
        }

        if (!current.IsEndOfWord)
        {
            current.IsEndOfWord = true;
            WordCount++;
        }
    }

    private TrieNode? FindNode(string word)
    {
        TrieNode current = _root;
        foreach (char c in word)
        {
            int index = c - 'A';
            if (index < 0 || index >= 26)
            {
                return null;
            }

            if (current.Children[index] is null)
            {
                return null;
            }

            current = current.Children[index]!;
        }

        return current;
    }

    private sealed class TrieNode
    {
        public TrieNode?[] Children { get; } = new TrieNode?[26];

        public bool IsEndOfWord { get; set; }
    }
}
