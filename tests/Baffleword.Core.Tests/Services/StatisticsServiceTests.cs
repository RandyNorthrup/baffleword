// <copyright file="StatisticsServiceTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Tests.Services;

using Baffleword.Core.Models;
using Baffleword.Core.Repositories;
using Baffleword.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public sealed class StatisticsServiceTests
{
    private readonly Mock<IStatisticsRepository> _repository = new();
    private readonly StatisticsService _sut;

    public StatisticsServiceTests()
    {
        _repository.Setup(r => r.GetAsync(It.IsAny<string>())).ReturnsAsync((string?)null);
        _sut = new StatisticsService(_repository.Object, NullLogger<StatisticsService>.Instance);
    }

    [Fact]
    public async Task GetStatisticsAsync_WhenEmpty_ReturnsDefaults()
    {
        GameStatistics stats = await _sut.GetStatisticsAsync();

        stats.TotalRoundsPlayed.Should().Be(0);
        stats.TotalScore.Should().Be(0);
        stats.HighestRoundScore.Should().Be(0);
        stats.TotalWordsFound.Should().Be(0);
        stats.LongestWordEver.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStatisticsAsync_WithStoredValues_ReturnsCorrectValues()
    {
        _repository.Setup(r => r.GetAsync("TotalRoundsPlayed")).ReturnsAsync("10");
        _repository.Setup(r => r.GetAsync("TotalScore")).ReturnsAsync("500");
        _repository.Setup(r => r.GetAsync("HighestRoundScore")).ReturnsAsync("120");
        _repository.Setup(r => r.GetAsync("LongestWordEver")).ReturnsAsync("TESTING");

        GameStatistics stats = await _sut.GetStatisticsAsync();

        stats.TotalRoundsPlayed.Should().Be(10);
        stats.TotalScore.Should().Be(500);
        stats.HighestRoundScore.Should().Be(120);
        stats.LongestWordEver.Should().Be("TESTING");
    }

    [Fact]
    public async Task UpdateStatisticsAsync_IncrementsTotalRounds()
    {
        GameRound round = CreateRound(50, "CAT");

        await _sut.UpdateStatisticsAsync(round);

        _repository.Verify(r => r.SetAsync("TotalRoundsPlayed", "1"), Times.Once);
    }

    [Fact]
    public async Task UpdateStatisticsAsync_AccumulatesScore()
    {
        _repository.Setup(r => r.GetAsync("TotalScore")).ReturnsAsync("100");
        GameRound round = CreateRound(50, "CAT");

        await _sut.UpdateStatisticsAsync(round);

        _repository.Verify(r => r.SetAsync("TotalScore", "150"), Times.Once);
    }

    [Fact]
    public async Task UpdateStatisticsAsync_UpdatesHighestRoundScore_WhenNew()
    {
        _repository.Setup(r => r.GetAsync("HighestRoundScore")).ReturnsAsync("30");
        GameRound round = CreateRound(50, "CAT");

        await _sut.UpdateStatisticsAsync(round);

        _repository.Verify(r => r.SetAsync("HighestRoundScore", "50"), Times.Once);
    }

    [Fact]
    public async Task UpdateStatisticsAsync_DoesNotUpdateHighest_WhenLower()
    {
        _repository.Setup(r => r.GetAsync("HighestRoundScore")).ReturnsAsync("100");
        GameRound round = CreateRound(50, "CAT");

        await _sut.UpdateStatisticsAsync(round);

        _repository.Verify(r => r.SetAsync("HighestRoundScore", "100"), Times.Once);
    }

    [Fact]
    public async Task UpdateStatisticsAsync_UpdatesLongestWord_WhenLonger()
    {
        _repository.Setup(r => r.GetAsync("LongestWordEver")).ReturnsAsync("CAT");
        GameRound round = CreateRound(50, "TESTING");

        await _sut.UpdateStatisticsAsync(round);

        _repository.Verify(r => r.SetAsync("LongestWordEver", "TESTING"), Times.Once);
    }

    [Fact]
    public async Task UpdateStatisticsAsync_KeepsLongestWord_WhenShorter()
    {
        _repository.Setup(r => r.GetAsync("LongestWordEver")).ReturnsAsync("TESTING");
        GameRound round = CreateRound(50, "CAT");

        await _sut.UpdateStatisticsAsync(round);

        _repository.Verify(r => r.SetAsync("LongestWordEver", "TESTING"), Times.Once);
    }

    [Fact]
    public async Task UpdateStatisticsAsync_UpdatesBestCompletion_WhenHigher()
    {
        _repository.Setup(r => r.GetAsync("BestCompletionPercentage")).ReturnsAsync("25");
        GameRound round = CreateRound(50, "CAT");
        round.TotalPossibleWords = 2;

        await _sut.UpdateStatisticsAsync(round);

        _repository.Verify(r => r.SetAsync("BestCompletionPercentage", "50"), Times.Once);
    }

    [Fact]
    public async Task UpdateStatisticsAsync_AccumulatesPlayTime()
    {
        _repository.Setup(r => r.GetAsync("TotalPlayTimeSeconds")).ReturnsAsync("180");
        GameRound round = CreateRound(50, "CAT");

        await _sut.UpdateStatisticsAsync(round);

        _repository.Verify(r => r.SetAsync("TotalPlayTimeSeconds", "360"), Times.Once);
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
