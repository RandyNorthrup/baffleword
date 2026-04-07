// <copyright file="DiceSetTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Tests.Models;

using Boggle.Core.Models;
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
    public void Count_Returns16()
    {
        DiceSet.Count.Should().Be(16);
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
    public void BigBoggleDice_Has25Dice()
    {
        DiceSet.BigBoggleDice.Should().HaveCount(25);
    }

    [Fact]
    public void BigBoggleDice_EachDieHas6Faces()
    {
        foreach (Die die in DiceSet.BigBoggleDice)
        {
            die.Faces.Should().HaveCount(6);
        }
    }

    [Fact]
    public void BigBoggleDice_ContainsQuDie()
    {
        DiceSet.BigBoggleDice.Should().Contain(d => d.Faces.Contains("QU"));
    }

    [Fact]
    public void SuperBigBoggleDice_Has36Dice()
    {
        DiceSet.SuperBigBoggleDice.Should().HaveCount(36);
    }

    [Fact]
    public void SuperBigBoggleDice_EachDieHas6Faces()
    {
        foreach (Die die in DiceSet.SuperBigBoggleDice)
        {
            die.Faces.Should().HaveCount(6);
        }
    }

    [Fact]
    public void SuperBigBoggleDice_ContainsBlockedFaces()
    {
        DiceSet.SuperBigBoggleDice.Should().Contain(d => d.Faces.Contains(DiceSet.BlockedFace));
    }

    [Fact]
    public void SuperBigBoggleDice_ContainsDigraphDie()
    {
        DiceSet.SuperBigBoggleDice.Should().Contain(d =>
            d.Faces.Contains("QU") && d.Faces.Contains("IN") && d.Faces.Contains("TH") &&
            d.Faces.Contains("ER") && d.Faces.Contains("HE") && d.Faces.Contains("AN"));
    }

    [Theory]
    [InlineData(GameMode.Standard, 16)]
    [InlineData(GameMode.BigBoggle, 25)]
    [InlineData(GameMode.SuperBigBoggle, 36)]
    public void GetDiceForMode_ReturnsCorrectCount(GameMode mode, int expected)
    {
        DiceSet.GetDiceForMode(mode).Should().HaveCount(expected);
    }
}
