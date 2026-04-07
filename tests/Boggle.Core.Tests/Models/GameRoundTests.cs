// <copyright file="GameRoundTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Tests.Models;

using Boggle.Core.Models;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the <see cref="GameRound"/> class.
/// </summary>
public sealed class GameRoundTests
{
    [Fact]
    public void Constructor_InitializesCorrectly()
    {
        GameBoard board = CreateTestBoard();
        var round = new GameRound(board, TimeSpan.FromMinutes(3), 3);

        round.Board.Should().BeSameAs(board);
        round.TimerDuration.Should().Be(TimeSpan.FromMinutes(3));
        round.MinimumWordLength.Should().Be(3);
        round.State.Should().Be(GameRoundState.Playing);
        round.Score.Should().Be(0);
        round.ValidWordCount.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithMinWordLengthBelow3_ThrowsArgumentOutOfRangeException()
    {
        GameBoard board = CreateTestBoard();

        Action act = () => new GameRound(board, TimeSpan.FromMinutes(3), 2);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AddWordResult_ValidWord_IncrementsScore()
    {
        GameBoard board = CreateTestBoard();
        var round = new GameRound(board, TimeSpan.FromMinutes(3), 3);

        round.AddWordResult(new WordResult("CAT", WordStatus.Valid, 1));

        round.Score.Should().Be(1);
        round.ValidWordCount.Should().Be(1);
    }

    [Fact]
    public void AddWordResult_InvalidWord_DoesNotIncrementScore()
    {
        GameBoard board = CreateTestBoard();
        var round = new GameRound(board, TimeSpan.FromMinutes(3), 3);

        round.AddWordResult(new WordResult("XYZ", WordStatus.NotInDictionary, 0));

        round.Score.Should().Be(0);
        round.InvalidSubmissionCount.Should().Be(1);
    }

    [Fact]
    public void HasBeenSubmitted_ReturnsTrueForPreviouslySubmittedValidWord()
    {
        GameBoard board = CreateTestBoard();
        var round = new GameRound(board, TimeSpan.FromMinutes(3), 3);
        round.AddWordResult(new WordResult("CAT", WordStatus.Valid, 1));

        round.HasBeenSubmitted("CAT").Should().BeTrue();
        round.HasBeenSubmitted("cat").Should().BeTrue();
    }

    [Fact]
    public void HasBeenSubmitted_ReturnsFalseForUnsubmittedWord()
    {
        GameBoard board = CreateTestBoard();
        var round = new GameRound(board, TimeSpan.FromMinutes(3), 3);

        round.HasBeenSubmitted("DOG").Should().BeFalse();
    }

    [Fact]
    public void CompletionPercentage_CalculatesCorrectly()
    {
        GameBoard board = CreateTestBoard();
        var round = new GameRound(board, TimeSpan.FromMinutes(3), 3);
        round.TotalPossibleWords = 100;
        round.AddWordResult(new WordResult("CAT", WordStatus.Valid, 1));
        round.AddWordResult(new WordResult("DOG", WordStatus.Valid, 1));

        round.CompletionPercentage.Should().BeApproximately(2.0, 0.001);
    }

    private static GameBoard CreateTestBoard()
    {
        string[] letters = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P"];
        BoardCell[][] cells = new BoardCell[4][];
        for (int row = 0; row < 4; row++)
        {
            cells[row] = new BoardCell[4];
            for (int col = 0; col < 4; col++)
            {
                int i = (row * 4) + col;
                cells[row][col] = new BoardCell(letters[i], row, col);
            }
        }

        return new GameBoard(cells);
    }
}
