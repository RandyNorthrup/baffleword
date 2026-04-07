// <copyright file="BoardGeneratorTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Tests.Services;

using Boggle.Core.Models;
using Boggle.Core.Services;
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

        for (int r = 0; r < GameBoard.Rows; r++)
        {
            for (int c = 0; c < GameBoard.Columns; c++)
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
        for (int r = 0; r < GameBoard.Rows; r++)
        {
            for (int c = 0; c < GameBoard.Columns; c++)
            {
                if (board1[r, c].Letter != board2[r, c].Letter)
                {
                    anyDifferent = true;
                }
            }
        }

        anyDifferent.Should().BeTrue();
    }
}
