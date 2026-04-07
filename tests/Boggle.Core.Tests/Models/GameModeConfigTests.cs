// <copyright file="GameModeConfigTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Tests.Models;

using Boggle.Core.Models;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the <see cref="GameModeConfig"/> class.
/// </summary>
public sealed class GameModeConfigTests
{
    [Fact]
    public void Standard_HasCorrectGridSize()
    {
        GameModeConfig.Standard.GridSize.Should().Be(4);
    }

    [Fact]
    public void Standard_HasCorrectMinWordLength()
    {
        GameModeConfig.Standard.MinWordLength.Should().Be(3);
    }

    [Fact]
    public void Standard_Has16Dice()
    {
        GameModeConfig.Standard.Dice.Should().HaveCount(16);
    }

    [Fact]
    public void Standard_HasCorrectDefaultTimer()
    {
        GameModeConfig.Standard.DefaultTimerSeconds.Should().Be(180);
    }

    [Fact]
    public void BigBoggle_HasCorrectGridSize()
    {
        GameModeConfig.BigBoggle.GridSize.Should().Be(5);
    }

    [Fact]
    public void BigBoggle_HasCorrectMinWordLength()
    {
        GameModeConfig.BigBoggle.MinWordLength.Should().Be(4);
    }

    [Fact]
    public void BigBoggle_Has25Dice()
    {
        GameModeConfig.BigBoggle.Dice.Should().HaveCount(25);
    }

    [Fact]
    public void BigBoggle_HasCorrectDefaultTimer()
    {
        GameModeConfig.BigBoggle.DefaultTimerSeconds.Should().Be(180);
    }

    [Fact]
    public void SuperBigBoggle_HasCorrectGridSize()
    {
        GameModeConfig.SuperBigBoggle.GridSize.Should().Be(6);
    }

    [Fact]
    public void SuperBigBoggle_HasCorrectMinWordLength()
    {
        GameModeConfig.SuperBigBoggle.MinWordLength.Should().Be(4);
    }

    [Fact]
    public void SuperBigBoggle_Has36Dice()
    {
        GameModeConfig.SuperBigBoggle.Dice.Should().HaveCount(36);
    }

    [Fact]
    public void SuperBigBoggle_HasCorrectDefaultTimer()
    {
        GameModeConfig.SuperBigBoggle.DefaultTimerSeconds.Should().Be(240);
    }

    [Theory]
    [InlineData(GameMode.Standard)]
    [InlineData(GameMode.BigBoggle)]
    [InlineData(GameMode.SuperBigBoggle)]
    public void ForMode_ReturnsMatchingConfig(GameMode mode)
    {
        GameModeConfig config = GameModeConfig.ForMode(mode);

        config.Mode.Should().Be(mode);
    }

    [Fact]
    public void ForMode_Standard_ReturnsSameAsStaticProperty()
    {
        GameModeConfig.ForMode(GameMode.Standard).Should().BeSameAs(GameModeConfig.Standard);
    }

    [Fact]
    public void ForMode_BigBoggle_ReturnsSameAsStaticProperty()
    {
        GameModeConfig.ForMode(GameMode.BigBoggle).Should().BeSameAs(GameModeConfig.BigBoggle);
    }

    [Fact]
    public void ForMode_SuperBigBoggle_ReturnsSameAsStaticProperty()
    {
        GameModeConfig.ForMode(GameMode.SuperBigBoggle).Should().BeSameAs(GameModeConfig.SuperBigBoggle);
    }

    [Fact]
    public void ForMode_InvalidMode_FallsBackToStandard()
    {
        GameModeConfig config = GameModeConfig.ForMode((GameMode)99);

        config.Should().BeSameAs(GameModeConfig.Standard);
    }
}
