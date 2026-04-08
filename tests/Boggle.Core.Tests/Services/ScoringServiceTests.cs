// <copyright file="ScoringServiceTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Tests.Services;

using Boggle.Core.Models;
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

    [Theory]
    [InlineData("CAT", 0)] // 3 letters = 0 in Super Big (min 4)
    [InlineData("CATS", 1)] // 4 letters = 1 point
    [InlineData("CATCH", 2)] // 5 letters = 2 points
    [InlineData("CATHER", 3)] // 6 letters = 3 points
    [InlineData("CATCHES", 5)] // 7 letters = 5 points
    [InlineData("CATCHING", 11)] // 8 letters = 11 points
    [InlineData("CATCHINGS", 18)] // 9 letters = 9 * 2 = 18 points
    [InlineData("OUTMATCHING", 22)] // 11 letters = 11 * 2 = 22 points
    public void CalculateWordScore_SuperBigBoggle_ReturnsCorrectPoints(string word, int expectedPoints)
    {
        _sut.CalculateWordScore(word, GameMode.SuperBigBoggle).Should().Be(expectedPoints);
    }

    [Theory]
    [InlineData("CAT", 1)] // Standard scoring unchanged
    [InlineData("CATCH", 2)]
    public void CalculateWordScore_BigBoggle_UsesSameAsStandard(string word, int expectedPoints)
    {
        _sut.CalculateWordScore(word, GameMode.BigBoggle).Should().Be(expectedPoints);
    }
}
