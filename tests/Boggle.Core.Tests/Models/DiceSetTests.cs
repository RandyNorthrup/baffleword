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
}
