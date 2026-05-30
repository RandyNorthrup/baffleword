// <copyright file="BoardGeneratorTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Tests.Services;

using Baffleword.Core.Models;
using Baffleword.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

/// <summary>
/// Tests for the <see cref="BoardGenerator"/> class.
/// </summary>
public sealed class BoardGeneratorTests
{
    private readonly BoardGenerator _sut = new(NullLogger<BoardGenerator>.Instance);

    [Fact]
    public void Generate_ReturnsValidBoard()
    {
        GameBoard board = _sut.Generate();

        board.AllCells.Should().HaveCount(16);
    }

    [Fact]
    public void Generate_AllCellsHaveLetters()
    {
        GameBoard board = _sut.Generate();

        foreach (BoardCell cell in board.AllCells)
        {
            cell.Letter.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public void Generate_AllLettersAreFromDiceFaces()
    {
        GameBoard board = _sut.Generate();
        HashSet<string> allValidFaces = DiceSet.StandardDice
            .SelectMany(d => d.Faces)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (BoardCell cell in board.AllCells)
        {
            allValidFaces.Should().Contain(cell.Letter);
        }
    }

    [Fact]
    public void Generate_WithSeedProducesDeterministicResults()
    {
        var gen1 = new BoardGenerator(NullLogger<BoardGenerator>.Instance, new Random(123));
        var gen2 = new BoardGenerator(NullLogger<BoardGenerator>.Instance, new Random(123));

        GameBoard board1 = gen1.Generate();
        GameBoard board2 = gen2.Generate();

        for (int r = 0; r < board1.Rows; r++)
        {
            for (int c = 0; c < board1.Columns; c++)
            {
                board1[r, c].Letter.Should().Be(board2[r, c].Letter);
            }
        }
    }

    [Fact]
    public void Generate_DifferentSeedsProduceDifferentBoards()
    {
        var gen1 = new BoardGenerator(NullLogger<BoardGenerator>.Instance, new Random(1));
        var gen2 = new BoardGenerator(NullLogger<BoardGenerator>.Instance, new Random(9999));

        GameBoard board1 = gen1.Generate();
        GameBoard board2 = gen2.Generate();

        // At least one cell should differ with very high probability
        bool anyDifferent = false;
        for (int r = 0; r < board1.Rows; r++)
        {
            for (int c = 0; c < board1.Columns; c++)
            {
                if (board1[r, c].Letter != board2[r, c].Letter)
                {
                    anyDifferent = true;
                }
            }
        }

        anyDifferent.Should().BeTrue();
    }

    [Fact]
    public void Generate_BigBoard_Returns5x5Board()
    {
        GameBoard board = _sut.Generate(GameMode.BigBoard);

        board.Rows.Should().Be(5);
        board.Columns.Should().Be(5);
        board.AllCells.Should().HaveCount(25);
    }

    [Fact]
    public void Generate_BigBoard_AllCellsHaveLetters()
    {
        GameBoard board = _sut.Generate(GameMode.BigBoard);

        foreach (BoardCell cell in board.AllCells)
        {
            cell.Letter.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public void Generate_SuperBoard_Returns6x6Board()
    {
        GameBoard board = _sut.Generate(GameMode.SuperBoard);

        board.Rows.Should().Be(6);
        board.Columns.Should().Be(6);
        board.AllCells.Should().HaveCount(36);
    }

    [Fact]
    public void Generate_SuperBoard_AllCellsHaveLettersOrAreBlocked()
    {
        GameBoard board = _sut.Generate(GameMode.SuperBoard);

        foreach (BoardCell cell in board.AllCells)
        {
            if (cell.IsBlocked)
            {
                cell.Letter.Should().BeEmpty();
            }
            else
            {
                cell.Letter.Should().NotBeNullOrEmpty();
            }
        }
    }

    [Fact]
    public void Generate_SuperBoard_MayContainBlockedCells()
    {
        // Generate many boards; at least one should have a blocked cell
        // (die #27 has 3 blocked faces out of 6, so probability is very high)
        bool foundBlocked = false;
        for (int i = 0; i < 20; i++)
        {
            var gen = new BoardGenerator(NullLogger<BoardGenerator>.Instance, new Random(i));
            GameBoard board = gen.Generate(GameMode.SuperBoard);
            if (board.AllCells.Any(c => c.IsBlocked))
            {
                foundBlocked = true;
                break;
            }
        }

        foundBlocked.Should().BeTrue();
    }

    [Fact]
    public void Generate_SuperBoard_MayContainDigraphs()
    {
        bool foundDigraph = false;
        for (int i = 0; i < 20; i++)
        {
            var gen = new BoardGenerator(NullLogger<BoardGenerator>.Instance, new Random(i));
            GameBoard board = gen.Generate(GameMode.SuperBoard);
            if (board.AllCells.Any(c => c.IsDigraph))
            {
                foundDigraph = true;
                break;
            }
        }

        foundDigraph.Should().BeTrue();
    }
}
