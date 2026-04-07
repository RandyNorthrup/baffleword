// <copyright file="SettingsRepositoryTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Data.Tests;

using Boggle.Data;
using Boggle.Data.Repositories;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

/// <summary>
/// Integration tests for the <see cref="SettingsRepository"/> class.
/// </summary>
public sealed class SettingsRepositoryTests : IDisposable
{
    private readonly SqliteConnection _keepAlive;
    private readonly BoggleDatabase _db;
    private readonly SettingsRepository _sut;

    public SettingsRepositoryTests()
    {
        string connectionString = $"Data Source=SettingsTest{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
        _keepAlive = new SqliteConnection(connectionString);
        _keepAlive.Open();
        _db = new BoggleDatabase(connectionString, NullLogger<BoggleDatabase>.Instance);
        _db.InitializeAsync().GetAwaiter().GetResult();
        _sut = new SettingsRepository(_db, NullLogger<SettingsRepository>.Instance);
    }

    public void Dispose()
    {
        _db.Dispose();
        _keepAlive.Dispose();
    }

    [Fact]
    public async Task GetAsync_WhenNotSet_ReturnsNull()
    {
        string? value = await _sut.GetAsync("missing_key");

        value.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_StoresValue()
    {
        await _sut.SetAsync("theme", "dark");

        string? value = await _sut.GetAsync("theme");
        value.Should().Be("dark");
    }

    [Fact]
    public async Task SetAsync_OverwritesExistingValue()
    {
        await _sut.SetAsync("volume", "50");
        await _sut.SetAsync("volume", "80");

        string? value = await _sut.GetAsync("volume");
        value.Should().Be("80");
    }

    [Fact]
    public async Task ClearAllAsync_RemovesAllSettings()
    {
        await _sut.SetAsync("key1", "value1");
        await _sut.SetAsync("key2", "value2");

        await _sut.ClearAllAsync();

        string? val1 = await _sut.GetAsync("key1");
        string? val2 = await _sut.GetAsync("key2");
        val1.Should().BeNull();
        val2.Should().BeNull();
    }
}
