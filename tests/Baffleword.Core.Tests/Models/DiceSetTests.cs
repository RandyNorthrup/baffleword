// <copyright file="DiceSetTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Tests.Models;

using Baffleword.Core.Models;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the <see cref="DiceSet"/> class.
/// </summary>
public sealed class DiceSetTests
{
    [Fact]
    public void StandardDice_Has16Dice()
    {
        DiceSet.StandardDice.Should().HaveCount(16);
    }

    [Fact]
    public void StandardDice_EachDieHas6Faces()
    {
        foreach (Die die in DiceSet.StandardDice)
        {
            die.Faces.Should().HaveCount(6);
        }
    }

    [Fact]
    public void StandardDice_ContainsQuDie()
    {
        DiceSet.StandardDice.Should().Contain(d => d.Faces.Contains("QU"));
    }

    [Fact]
    public void StandardDice_AllFacesAreUpperCase()
    {
        foreach (Die die in DiceSet.StandardDice)
        {
            foreach (string face in die.Faces)
            {
                face.Should().Be(face.ToUpperInvariant());
            }
        }
    }

    [Fact]
    public void BigBoardDice_Has25Dice()
    {
        DiceSet.BigBoardDice.Should().HaveCount(25);
    }

    [Fact]
    public void BigBoardDice_EachDieHas6Faces()
    {
        foreach (Die die in DiceSet.BigBoardDice)
        {
            die.Faces.Should().HaveCount(6);
        }
    }

    [Fact]
    public void BigBoardDice_ContainsQuDie()
    {
        DiceSet.BigBoardDice.Should().Contain(d => d.Faces.Contains("QU"));
    }

    [Fact]
    public void SuperBoardDice_Has36Dice()
    {
        DiceSet.SuperBoardDice.Should().HaveCount(36);
    }

    [Fact]
    public void SuperBoardDice_EachDieHas6Faces()
    {
        foreach (Die die in DiceSet.SuperBoardDice)
        {
            die.Faces.Should().HaveCount(6);
        }
    }

    [Fact]
    public void SuperBoardDice_ContainsBlockedFaces()
    {
        DiceSet.SuperBoardDice.Should().Contain(d => d.Faces.Contains(DiceSet.BlockedFace));
    }

    [Fact]
    public void SuperBoardDice_ContainsDigraphDie()
    {
        DiceSet.SuperBoardDice.Should().Contain(d =>
            d.Faces.Contains("QU") && d.Faces.Contains("IN") && d.Faces.Contains("TH") &&
            d.Faces.Contains("ER") && d.Faces.Contains("HE") && d.Faces.Contains("AN"));
    }

    [Theory]
    [InlineData(GameMode.Standard, 16)]
    [InlineData(GameMode.BigBoard, 25)]
    [InlineData(GameMode.SuperBoard, 36)]
    public void GetDiceForMode_ReturnsCorrectCount(GameMode mode, int expected)
    {
        DiceSet.GetDiceForMode(mode).Should().HaveCount(expected);
    }
}
