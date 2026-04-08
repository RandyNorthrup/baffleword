// <copyright file="HighScoreRepositoryTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Data.Tests;

using Boggle.Core.Models;
using Boggle.Data;
using Boggle.Data.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

/// <summary>
/// Integration tests for the <see cref="HighScoreRepository"/> class.
/// </summary>
public sealed class HighScoreRepositoryTests : IDisposable
{
    private const string GameModeFilter = "Standard";
    private readonly SqliteConnection _keepAlive;
    private readonly BoggleDatabase _db;
    private readonly HighScoreRepository _sut;

    public HighScoreRepositoryTests()
    {
        string connectionString = $"Data Source=HighScoreTest{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
        _keepAlive = new SqliteConnection(connectionString);
        _keepAlive.Open();
        _db = new BoggleDatabase(connectionString, NullLogger<BoggleDatabase>.Instance);
        _db.InitializeAsync().GetAwaiter().GetResult();
        _sut = new HighScoreRepository(_db, NullLogger<HighScoreRepository>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _keepAlive.Dispose();
    }

    [Fact]
    public async Task AddAsync_StoresHighScore()
    {
        var entry = CreateEntry(100);

        await _sut.AddAsync(entry);

        IReadOnlyList<HighScoreEntry> scores = await _sut.GetTopAsync(GameModeFilter, 10);
        scores.Should().ContainSingle();
    }

    [Fact]
    public async Task GetTopAsync_ReturnsInDescendingScoreOrder()
    {
        await _sut.AddAsync(CreateEntry(50));
        await _sut.AddAsync(CreateEntry(100));
        await _sut.AddAsync(CreateEntry(75));

        IReadOnlyList<HighScoreEntry> scores = await _sut.GetTopAsync(GameModeFilter, 10);

        scores.Select(s => s.Score).Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task GetTopAsync_RespectsLimit()
    {
        for (int i = 0; i < 20; i++)
        {
            await _sut.AddAsync(CreateEntry(i * 10));
        }

        IReadOnlyList<HighScoreEntry> scores = await _sut.GetTopAsync(GameModeFilter, 5);

        scores.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetMinimumTopScoreAsync_ReturnsLowestOfTopN()
    {
        await _sut.AddAsync(CreateEntry(100));
        await _sut.AddAsync(CreateEntry(50));
        await _sut.AddAsync(CreateEntry(75));

        int min = await _sut.GetMinimumTopScoreAsync(GameModeFilter, 3);

        min.Should().Be(50);
    }

    [Fact]
    public async Task GetMinimumTopScoreAsync_WhenEmpty_ReturnsZero()
    {
        int min = await _sut.GetMinimumTopScoreAsync(GameModeFilter, 10);

        min.Should().Be(0);
    }

    [Fact]
    public async Task AddAsync_PrunesEntriesBeyond50PerGameMode()
    {
        for (int i = 1; i <= 51; i++)
        {
            await _sut.AddAsync(CreateEntry(i * 10));
        }

        IReadOnlyList<HighScoreEntry> scores = await _sut.GetTopAsync(GameModeFilter, 100);

        scores.Should().HaveCount(50);
        scores.Should().NotContain(s => s.Score == 10, "the lowest score should have been pruned");
    }

    private static HighScoreEntry CreateEntry(int score) => new()
    {
        Score = score,
        WordsFound = score / 10,
        CompletionPercentage = score / 100.0,
        LongestWord = "TEST",
        TimerDuration = TimeSpan.FromSeconds(180),
        GameMode = GameMode.Standard,
        AchievedAt = DateTime.UtcNow,
    };
}
