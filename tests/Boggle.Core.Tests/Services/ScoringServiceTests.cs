// <copyright file="ScoringServiceTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Tests.Services;

using Boggle.Core.Services;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the <see cref="ScoringService"/> class.
/// </summary>
public sealed class ScoringServiceTests
{
    private readonly ScoringService _sut = new();

    [Theory]
    [InlineData("CAT", 1)] // 3 letters = 1 point
    [InlineData("CATS", 1)] // 4 letters = 1 point
    [InlineData("CATCH", 2)] // 5 letters = 2 points
    [InlineData("CATHER", 3)] // 6 letters = 3 points
    [InlineData("CATCHES", 5)] // 7 letters = 5 points
    [InlineData("CATCHING", 11)] // 8 letters = 11 points
    [InlineData("CATCHINGS", 11)] // 9 letters = 11 points
    public void CalculateWordScore_ReturnsCorrectPoints(string word, int expectedPoints)
    {
        _sut.CalculateWordScore(word).Should().Be(expectedPoints);
    }

    [Theory]
    [InlineData("", 0)]
    [InlineData("AB", 0)]
    public void CalculateWordScore_ShortWords_ReturnsZero(string word, int expected)
    {
        _sut.CalculateWordScore(word).Should().Be(expected);
    }

    [Fact]
    public void GetEffectiveLength_ReturnsWordLength()
    {
        _sut.GetEffectiveLength("HELLO").Should().Be(5);
    }

    [Fact]
    public void GetEffectiveLength_EmptyString_ReturnsZero()
    {
        _sut.GetEffectiveLength(string.Empty).Should().Be(0);
    }

    [Fact]
    public void GetEffectiveLength_Null_ReturnsZero()
    {
        _sut.GetEffectiveLength(null!).Should().Be(0);
    }
}
