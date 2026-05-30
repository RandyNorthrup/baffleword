// <copyright file="GameModeConfig.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Models;

/// <summary>
/// Configuration parameters for a specific Baffleword game mode.
/// </summary>
public sealed class GameModeConfig
{
    /// <summary>
    /// Gets the standard configuration for standard Baffleword (4x4).
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
    /// Gets the standard configuration for Big Board (5x5).
    /// </summary>
    public static GameModeConfig BigBoard { get; } = new()
    {
        Mode = GameMode.BigBoard,
        GridSize = 5,
        Dice = DiceSet.BigBoardDice,
        MinWordLength = 4,
        DefaultTimerSeconds = 180,
    };

    /// <summary>
    /// Gets the standard configuration for Super Board (6x6).
    /// </summary>
    public static GameModeConfig SuperBoard { get; } = new()
    {
        Mode = GameMode.SuperBoard,
        GridSize = 6,
        Dice = DiceSet.SuperBoardDice,
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
            GameMode.BigBoard => BigBoard,
            GameMode.SuperBoard => SuperBoard,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown game mode."),
        };
    }
}
