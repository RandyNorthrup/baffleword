// <copyright file="AchievementRepositoryTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
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
/// Integration tests for the <see cref="AchievementRepository"/> class.
/// </summary>
public sealed class AchievementRepositoryTests : IDisposable
{
    private readonly SqliteConnection _keepAlive;
    private readonly BoggleDatabase _db;
    private readonly AchievementRepository _sut;

    public AchievementRepositoryTests()
    {
        string connectionString = $"Data Source=AchievementTest{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
        _keepAlive = new SqliteConnection(connectionString);
        _keepAlive.Open();
        _db = new BoggleDatabase(connectionString, NullLogger<BoggleDatabase>.Instance);
        _db.InitializeAsync().GetAwaiter().GetResult();
        _sut = new AchievementRepository(_db, NullLogger<AchievementRepository>.Instance);
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

    [Fact]
    public async Task ClearAllAsync_RemovesAllAchievements()
    {
        await _sut.SaveAsync(CreateAchievement(1, "A1", true));
        await _sut.SaveAsync(CreateAchievement(2, "A2", false));

        await _sut.ClearAllAsync();

        IReadOnlyList<Achievement> achievements = await _sut.GetAllAsync();
        achievements.Should().BeEmpty();
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
