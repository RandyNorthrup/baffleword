// <copyright file="GameModeConfigTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Tests.Models;

using Baffleword.Core.Models;
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
    public void BigBoard_HasCorrectGridSize()
    {
        GameModeConfig.BigBoard.GridSize.Should().Be(5);
    }

    [Fact]
    public void BigBoard_HasCorrectMinWordLength()
    {
        GameModeConfig.BigBoard.MinWordLength.Should().Be(4);
    }

    [Fact]
    public void BigBoard_Has25Dice()
    {
        GameModeConfig.BigBoard.Dice.Should().HaveCount(25);
    }

    [Fact]
    public void BigBoard_HasCorrectDefaultTimer()
    {
        GameModeConfig.BigBoard.DefaultTimerSeconds.Should().Be(180);
    }

    [Fact]
    public void SuperBoard_HasCorrectGridSize()
    {
        GameModeConfig.SuperBoard.GridSize.Should().Be(6);
    }

    [Fact]
    public void SuperBoard_HasCorrectMinWordLength()
    {
        GameModeConfig.SuperBoard.MinWordLength.Should().Be(4);
    }

    [Fact]
    public void SuperBoard_Has36Dice()
    {
        GameModeConfig.SuperBoard.Dice.Should().HaveCount(36);
    }

    [Fact]
    public void SuperBoard_HasCorrectDefaultTimer()
    {
        GameModeConfig.SuperBoard.DefaultTimerSeconds.Should().Be(240);
    }

    [Theory]
    [InlineData(GameMode.Standard)]
    [InlineData(GameMode.BigBoard)]
    [InlineData(GameMode.SuperBoard)]
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
    public void ForMode_BigBoard_ReturnsSameAsStaticProperty()
    {
        GameModeConfig.ForMode(GameMode.BigBoard).Should().BeSameAs(GameModeConfig.BigBoard);
    }

    [Fact]
    public void ForMode_SuperBoard_ReturnsSameAsStaticProperty()
    {
        GameModeConfig.ForMode(GameMode.SuperBoard).Should().BeSameAs(GameModeConfig.SuperBoard);
    }

    [Fact]
    public void ForMode_InvalidMode_ThrowsArgumentOutOfRangeException()
    {
        Action act = () => GameModeConfig.ForMode((GameMode)99);

        act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("mode");
    }
}
