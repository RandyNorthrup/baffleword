// <copyright file="TrieDictionaryProviderTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Tests.Dictionary;

using Baffleword.Core.Dictionary;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the <see cref="TrieDictionaryProvider"/> class.
/// </summary>
public sealed class TrieDictionaryProviderTests
{
    [Fact]
    public void LoadWords_SetsWordCount()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["cat", "car", "card"]);

        trie.WordCount.Should().Be(3);
    }

    [Fact]
    public void IsValidWord_ReturnsTrueForLoadedWord()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["cat"]);

        trie.IsValidWord("CAT").Should().BeTrue();
    }

    [Fact]
    public void IsValidWord_ReturnsFalseForMissingWord()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["cat"]);

        trie.IsValidWord("DOG").Should().BeFalse();
    }

    [Fact]
    public void IsValidWord_ReturnsFalseForPrefix()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["card"]);

        trie.IsValidWord("CAR").Should().BeFalse();
    }

    [Fact]
    public void HasPrefix_ReturnsTrueForValidPrefix()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["card"]);

        trie.HasPrefix("CAR").Should().BeTrue();
    }

    [Fact]
    public void HasPrefix_ReturnsFalseForInvalidPrefix()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["card"]);

        trie.HasPrefix("DOG").Should().BeFalse();
    }

    [Fact]
    public void HasPrefix_ReturnsTrueForEmptyPrefix()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["cat"]);

        trie.HasPrefix(string.Empty).Should().BeTrue();
    }

    [Fact]
    public void IsValidWord_EmptyString_ReturnsFalse()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["cat"]);

        trie.IsValidWord(string.Empty).Should().BeFalse();
    }

    [Fact]
    public void LoadWords_SkipsDuplicates()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["cat", "CAT", "Cat"]);

        trie.WordCount.Should().Be(1);
    }

    [Fact]
    public void LoadWords_SkipsNonAlphabeticCharacters()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["cat", "don't", "well-known"]);

        trie.WordCount.Should().Be(1);
        trie.IsValidWord("CAT").Should().BeTrue();
    }
}
