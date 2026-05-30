// <copyright file="StatisticsRepositoryTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Data.Tests;

using Baffleword.Data;
using Baffleword.Data.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

/// <summary>
/// Integration tests for the <see cref="StatisticsRepository"/> class.
/// </summary>
public sealed class StatisticsRepositoryTests : IDisposable
{
    private readonly SqliteConnection _keepAlive;
    private readonly BafflewordDatabase _db;
    private readonly StatisticsRepository _sut;

    public StatisticsRepositoryTests()
    {
        string connectionString = $"Data Source=StatsTest{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
        _keepAlive = new SqliteConnection(connectionString);
        _keepAlive.Open();
        _db = new BafflewordDatabase(connectionString, NullLogger<BafflewordDatabase>.Instance);
        _db.InitializeAsync().GetAwaiter().GetResult();
        _sut = new StatisticsRepository(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
        _keepAlive.Dispose();
    }

    [Fact]
    public async Task GetAsync_WhenNotSet_ReturnsNull()
    {
        string? value = await _sut.GetAsync("missing_stat");

        value.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_StoresValue()
    {
        await _sut.SetAsync("total_games", "42");

        string? value = await _sut.GetAsync("total_games");
        value.Should().Be("42");
    }

    [Fact]
    public async Task SetAsync_OverwritesExistingValue()
    {
        await _sut.SetAsync("total_games", "10");
        await _sut.SetAsync("total_games", "20");

        string? value = await _sut.GetAsync("total_games");
        value.Should().Be("20");
    }
}
