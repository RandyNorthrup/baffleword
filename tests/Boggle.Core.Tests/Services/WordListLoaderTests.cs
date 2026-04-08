// <copyright file="WordListLoaderTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Tests.Services;

using System.Text;
using Boggle.Core.Dictionary;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public sealed class WordListLoaderTests
{
    private readonly WordListLoader _sut = new(NullLogger<WordListLoader>.Instance);

    [Fact]
    public void LoadFromStream_ReturnsWordsWithThreeOrMoreLetters()
    {
        using MemoryStream stream = CreateStream("cat\ndog\nhi\nbat\n");

        IEnumerable<string> words = _sut.LoadFromStream(stream);

        words.Should().BeEquivalentTo("cat", "dog", "bat");
    }

    [Fact]
    public void LoadFromStream_FiltersNonAlphabeticWords()
    {
        using MemoryStream stream = CreateStream("cat\ndog123\nba-t\nhello\n");

        IEnumerable<string> words = _sut.LoadFromStream(stream);

        words.Should().BeEquivalentTo("cat", "hello");
    }

    [Fact]
    public void LoadFromStream_TrimsWhitespace()
    {
        using MemoryStream stream = CreateStream("  cat  \n  dog  \n");

        IEnumerable<string> words = _sut.LoadFromStream(stream);

        words.Should().BeEquivalentTo("cat", "dog");
    }

    [Fact]
    public void LoadFromStream_SkipsEmptyLines()
    {
        using MemoryStream stream = CreateStream("cat\n\n\ndog\n");

        IEnumerable<string> words = _sut.LoadFromStream(stream);

        words.Should().BeEquivalentTo("cat", "dog");
    }

    [Fact]
    public void LoadFromStream_EmptyStream_ReturnsEmpty()
    {
        using MemoryStream stream = CreateStream(string.Empty);

        IEnumerable<string> words = _sut.LoadFromStream(stream);

        words.Should().BeEmpty();
    }

    [Fact]
    public void LoadFromStream_WithNullStream_ThrowsArgumentNullException()
    {
        Action act = () => _sut.LoadFromStream(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void LoadFromStream_PreservesCase()
    {
        using MemoryStream stream = CreateStream("Cat\nDOG\nhello\n");

        IEnumerable<string> words = _sut.LoadFromStream(stream);

        words.Should().BeEquivalentTo("Cat", "DOG", "hello");
    }

    private static MemoryStream CreateStream(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }
}
