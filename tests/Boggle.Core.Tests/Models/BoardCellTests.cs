// <copyright file="BoardCellTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Tests.Models;

using Boggle.Core.Models;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the <see cref="BoardCell"/> class.
/// </summary>
public sealed class BoardCellTests
{
    [Fact]
    public void Constructor_WithValidArgs_CreatesCell()
    {
        var cell = new BoardCell("A", 0, 0);

        cell.Letter.Should().Be("A");
        cell.Row.Should().Be(0);
        cell.Column.Should().Be(0);
    }

    [Fact]
    public void Constructor_ConvertsLetterToUpperCase()
    {
        var cell = new BoardCell("a", 0, 0);

        cell.Letter.Should().Be("A");
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(4, 0)]
    [InlineData(0, 4)]
    public void Constructor_WithInvalidPosition_ThrowsArgumentOutOfRangeException(int row, int col)
    {
        Action act = () => new BoardCell("A", row, col);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Constructor_WithNullLetter_ThrowsArgumentException()
    {
        Action act = () => new BoardCell(null!, 0, 0);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IsQu_ReturnsTrueForQuCell()
    {
        var cell = new BoardCell("QU", 0, 0);

        cell.IsQu.Should().BeTrue();
    }

    [Fact]
    public void IsQu_ReturnsFalseForNormalCell()
    {
        var cell = new BoardCell("Q", 0, 0);

        cell.IsQu.Should().BeFalse();
    }

    [Theory]
    [InlineData(0, 0, 0, 1, true)] // horizontal neighbor
    [InlineData(0, 0, 1, 0, true)] // vertical neighbor
    [InlineData(0, 0, 1, 1, true)] // diagonal neighbor
    [InlineData(0, 0, 0, 2, false)] // too far
    [InlineData(0, 0, 2, 2, false)] // too far diagonal
    [InlineData(0, 0, 0, 0, false)] // same cell
    public void IsAdjacentTo_ReturnsExpectedResult(int r1, int c1, int r2, int c2, bool expected)
    {
        var cell1 = new BoardCell("A", r1, c1);
        var cell2 = new BoardCell("B", r2, c2);

        cell1.IsAdjacentTo(cell2).Should().Be(expected);
    }

    [Fact]
    public void IsAdjacentTo_WithNull_ThrowsArgumentNullException()
    {
        var cell = new BoardCell("A", 0, 0);

        Action act = () => cell.IsAdjacentTo(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
