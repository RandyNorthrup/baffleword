// <copyright file="DieTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Tests.Models;

using Baffleword.Core.Models;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the <see cref="Die"/> class.
/// </summary>
public sealed class DieTests
{
    private static readonly string[] _expectedFaces = ["A", "B", "C", "D", "E", "F"];

    [Fact]
    public void Constructor_WithValidFaces_CreatesInstance()
    {
        var die = new Die("A", "B", "C", "D", "E", "F");

        die.Faces.Should().HaveCount(6);
        die.Faces.Should().BeEquivalentTo(_expectedFaces);
    }

    [Fact]
    public void Constructor_WithWrongFaceCount_ThrowsArgumentException()
    {
        Action act = () => new Die("A", "B", "C");

        act.Should().Throw<ArgumentException>().WithMessage("*6 faces*");
    }

    [Fact]
    public void Constructor_WithNull_ThrowsArgumentNullException()
    {
        Action act = () => new Die(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Faces_AreConvertedToUpperCase()
    {
        var die = new Die("a", "b", "c", "d", "e", "f");

        die.Faces.Should().BeEquivalentTo(_expectedFaces);
    }

    [Fact]
    public void Roll_ReturnsOneOfTheFaces()
    {
        var die = new Die("A", "B", "C", "D", "E", "F");
        var random = new Random(42);

        string result = die.Roll(random);

        die.Faces.Should().Contain(result);
    }

    [Fact]
    public void Roll_WithNullRandom_ThrowsArgumentNullException()
    {
        var die = new Die("A", "B", "C", "D", "E", "F");

        Action act = () => die.Roll(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
