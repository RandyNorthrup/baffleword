// <copyright file="HighScoreServiceTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Tests.Services;

using Boggle.Core.Models;
using Boggle.Core.Repositories;
using Boggle.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public sealed class HighScoreServiceTests
{
    private readonly Mock<IHighScoreRepository> _repository = new();
    private readonly HighScoreService _sut;

    public HighScoreServiceTests()
    {
        _sut = new HighScoreService(_repository.Object, NullLogger<HighScoreService>.Instance);
    }

    [Fact]
    public async Task TryRecordScoreAsync_QualifyingScore_RecordsAndReturnsEntry()
    {
        GameRound round = CreateRound(100, "TESTING");
        _repository.Setup(r => r.GetMinimumTopScoreAsync(180, 50))
            .ReturnsAsync(50);

        HighScoreEntry? result = await _sut.TryRecordScoreAsync(round);

        result.Should().NotBeNull();
        result!.Score.Should().Be(100);
        result.LongestWord.Should().Be("TESTING");
        _repository.Verify(r => r.AddAsync(It.IsAny<HighScoreEntry>()), Times.Once);
    }

    [Fact]
    public async Task TryRecordScoreAsync_NonQualifyingScore_ReturnsNull()
    {
        GameRound round = CreateRound(10, "CAT");
        _repository.Setup(r => r.GetMinimumTopScoreAsync(180, 50))
            .ReturnsAsync(50);

        HighScoreEntry? result = await _sut.TryRecordScoreAsync(round);

        result.Should().BeNull();
        _repository.Verify(r => r.AddAsync(It.IsAny<HighScoreEntry>()), Times.Never);
    }

    [Fact]
    public async Task TryRecordScoreAsync_EmptyLeaderboard_RecordsScore()
    {
        GameRound round = CreateRound(1, "CAT");
        _repository.Setup(r => r.GetMinimumTopScoreAsync(180, 50))
            .ReturnsAsync(0);

        HighScoreEntry? result = await _sut.TryRecordScoreAsync(round);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task TryRecordScoreAsync_NoValidWords_LongestWordIsEmpty()
    {
        GameRound round = new(CreateTestBoard(), TimeSpan.FromMinutes(3), 3);
        round.AddWordResult(new WordResult("XY", WordStatus.TooShort, 0));
        _repository.Setup(r => r.GetMinimumTopScoreAsync(180, 50))
            .ReturnsAsync(0);

        HighScoreEntry? result = await _sut.TryRecordScoreAsync(round);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTopScoresAsync_DelegatesToRepository()
    {
        var expected = new List<HighScoreEntry> { new() { Score = 100 } };
        _repository.Setup(r => r.GetTopAsync(180, 10)).ReturnsAsync(expected);

        IReadOnlyList<HighScoreEntry> result = await _sut.GetTopScoresAsync(TimeSpan.FromMinutes(3), 10);

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task TryRecordScoreAsync_SetsCompletionPercentage()
    {
        GameRound round = CreateRound(100, "TESTING");
        round.TotalPossibleWords = 10;
        _repository.Setup(r => r.GetMinimumTopScoreAsync(180, 50))
            .ReturnsAsync(0);

        HighScoreEntry? result = await _sut.TryRecordScoreAsync(round);

        result.Should().NotBeNull();
        result!.CompletionPercentage.Should().Be(round.CompletionPercentage);
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

    private static GameRound CreateRound(int score, string longestWord)
    {
        GameRound round = new(CreateTestBoard(), TimeSpan.FromMinutes(3), 3);
        round.AddWordResult(new WordResult(longestWord, WordStatus.Valid, score));
        return round;
    }
}
