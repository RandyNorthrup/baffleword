// <copyright file="AchievementRepositoryTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Data.Tests;

using Baffleword.Core.Models;
using Baffleword.Data;
using Baffleword.Data.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

/// <summary>
/// Integration tests for the <see cref="AchievementRepository"/> class.
/// </summary>
public sealed class AchievementRepositoryTests : IDisposable
{
    private readonly SqliteConnection _keepAlive;
    private readonly BafflewordDatabase _db;
    private readonly AchievementRepository _sut;

    public AchievementRepositoryTests()
    {
        string connectionString = $"Data Source=AchievementTest{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
        _keepAlive = new SqliteConnection(connectionString);
        _keepAlive.Open();
        _db = new BafflewordDatabase(connectionString, NullLogger<BafflewordDatabase>.Instance);
        _db.InitializeAsync().GetAwaiter().GetResult();
        _sut = new AchievementRepository(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
        _keepAlive.Dispose();
    }

    [Fact]
    public async Task GetAllAsync_WhenEmpty_ReturnsEmptyList()
    {
        IReadOnlyList<Achievement> achievements = await _sut.GetAllAsync();

        achievements.Should().BeEmpty();
    }

    [Fact]
    public async Task SaveAsync_StoresAchievement()
    {
        var achievement = CreateAchievement(1, "First Word", true);

        await _sut.SaveAsync(achievement);

        IReadOnlyList<Achievement> achievements = await _sut.GetAllAsync();
        achievements.Should().ContainSingle();
        achievements[0].Name.Should().Be("First Word");
    }

    [Fact]
    public async Task SaveAsync_UpdatesExistingAchievement()
    {
        var achievement = CreateAchievement(1, "First Word", false);
        await _sut.SaveAsync(achievement);

        achievement.IsUnlocked = true;
        achievement.UnlockedAt = DateTime.UtcNow;
        await _sut.SaveAsync(achievement);

        IReadOnlyList<Achievement> achievements = await _sut.GetAllAsync();
        achievements.Should().ContainSingle();
        achievements[0].IsUnlocked.Should().BeTrue();
    }

    private static Achievement CreateAchievement(int id, string name, bool unlocked) => new()
    {
        Id = id,
        Name = name,
        Description = $"Test achievement: {name}",
        IsUnlocked = unlocked,
        UnlockedAt = unlocked ? DateTime.UtcNow : null,
    };
}
