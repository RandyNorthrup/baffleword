// <copyright file="BoggleDatabaseTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Data.Tests;

using Boggle.Data;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

/// <summary>
/// Integration tests for the <see cref="BoggleDatabase"/> class.
/// </summary>
public sealed class BoggleDatabaseTests : IDisposable
{
    private readonly BoggleDatabase _sut;

    public BoggleDatabaseTests()
    {
        _sut = new BoggleDatabase("Data Source=BoggleDatabaseTest;Mode=Memory;Cache=Shared", NullLogger<BoggleDatabase>.Instance);
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
