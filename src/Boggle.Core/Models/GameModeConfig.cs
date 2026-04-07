// <copyright file="GameModeConfig.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Configuration parameters for a specific Boggle game mode.
/// </summary>
public sealed class GameModeConfig
{
    /// <summary>
    /// Gets the standard configuration for standard Boggle (4×4).
    /// </summary>
    public static GameModeConfig Standard { get; } = new()
    {
        Mode = GameMode.Standard,
        GridSize = 4,
        Dice = DiceSet.StandardDice,
        MinWordLength = 3,
        DefaultTimerSeconds = 180,
    };

    /// <summary>
    /// Gets the standard configuration for Big Boggle (5×5).
    /// </summary>
    public static GameModeConfig BigBoggle { get; } = new()
    {
        Mode = GameMode.BigBoggle,
        GridSize = 5,
        Dice = DiceSet.BigBoggleDice,
        MinWordLength = 4,
        DefaultTimerSeconds = 180,
    };

    /// <summary>
    /// Gets the standard configuration for Super Big Boggle (6×6).
    /// </summary>
    public static GameModeConfig SuperBigBoggle { get; } = new()
    {
        Mode = GameMode.SuperBigBoggle,
        GridSize = 6,
        Dice = DiceSet.SuperBigBoggleDice,
        MinWordLength = 4,
        DefaultTimerSeconds = 240,
    };

    /// <summary>
    /// Gets the game mode.
    /// </summary>
    public GameMode Mode { get; init; }

    /// <summary>
    /// Gets the number of rows on the board.
    /// </summary>
    public int GridSize { get; init; }

    /// <summary>
    /// Gets the dice set for this mode.
    /// </summary>
    public required IReadOnlyList<Die> Dice { get; init; }

    /// <summary>
    /// Gets the minimum word length for this mode.
    /// </summary>
    public int MinWordLength { get; init; }

    /// <summary>
    /// Gets the default timer duration in seconds.
    /// </summary>
    public int DefaultTimerSeconds { get; init; }

    /// <summary>
    /// Gets the configuration for the specified game mode.
    /// </summary>
    /// <param name="mode">The game mode.</param>
    /// <returns>The configuration for the specified mode.</returns>
    public static GameModeConfig ForMode(GameMode mode)
    {
        return mode switch
        {
            GameMode.Standard => Standard,
            GameMode.BigBoggle => BigBoggle,
            GameMode.SuperBigBoggle => SuperBigBoggle,
            _ => Standard,
        };
    }
}
