// <copyright file="BafflewordDatabaseTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Data.Tests;

using Baffleword.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

/// <summary>
/// Integration tests for the <see cref="BafflewordDatabase"/> class.
/// </summary>
public sealed class BafflewordDatabaseTests : IDisposable
{
    private readonly BafflewordDatabase _sut;

    public BafflewordDatabaseTests()
    {
        _sut = new BafflewordDatabase("Data Source=BafflewordDatabaseTest;Mode=Memory;Cache=Shared", NullLogger<BafflewordDatabase>.Instance);
    }

    public void Dispose()
    {
        _sut.Dispose();
    }

    [Fact]
    public async Task InitializeAsync_CreatesDatabase()
    {
        await _sut.InitializeAsync();

        // If we get here without exception, the database initialized successfully
        _sut.Should().NotBeNull();
    }

    [Fact]
    public async Task InitializeAsync_CanBeCalledMultipleTimes()
    {
        await _sut.InitializeAsync();
        await _sut.InitializeAsync();

        // Should not throw
        _sut.Should().NotBeNull();
    }
}
