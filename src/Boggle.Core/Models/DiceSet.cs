// <copyright file="DiceSet.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Contains dice sets for all Boggle game modes.
/// </summary>
public static class DiceSet
{
    /// <summary>
    /// The face value representing a blocked position on the board.
    /// </summary>
    public const string BlockedFace = "";

    /// <summary>
    /// Gets the standard set of 16 Boggle dice (Hasbro "New Boggle" C2187).
    /// </summary>
    public static IReadOnlyList<Die> StandardDice { get; } = new Die[]
    {
        new("A", "A", "E", "E", "G", "N"),
        new("A", "B", "B", "J", "O", "O"),
        new("A", "C", "H", "O", "P", "S"),
        new("A", "F", "F", "K", "P", "S"),
        new("A", "O", "O", "T", "T", "W"),
        new("C", "I", "M", "O", "T", "U"),
        new("D", "E", "I", "L", "R", "X"),
        new("D", "E", "L", "R", "V", "Y"),
        new("D", "I", "S", "T", "T", "Y"),
        new("E", "E", "G", "H", "N", "W"),
        new("E", "E", "I", "N", "S", "U"),
        new("E", "H", "R", "T", "V", "W"),
        new("E", "I", "O", "S", "S", "T"),
        new("E", "L", "R", "T", "T", "Y"),
        new("H", "I", "M", "N", "QU", "U"),
        new("H", "L", "N", "N", "R", "Z"),
    };

    /// <summary>
    /// Gets the Big Boggle Deluxe set of 25 dice (5x5 grid).
    /// </summary>
    public static IReadOnlyList<Die> BigBoggleDice { get; } = new Die[]
    {
        new("A", "A", "A", "F", "R", "S"),
        new("A", "A", "E", "E", "E", "E"),
        new("A", "A", "F", "I", "R", "S"),
        new("A", "D", "E", "N", "N", "N"),
        new("A", "E", "E", "E", "E", "M"),
        new("A", "E", "E", "G", "M", "U"),
        new("A", "E", "G", "M", "N", "N"),
        new("A", "F", "I", "R", "S", "Y"),
        new("B", "J", "K", "QU", "X", "Z"),
        new("C", "C", "N", "S", "T", "W"),
        new("C", "E", "I", "I", "L", "T"),
        new("C", "E", "I", "L", "P", "T"),
        new("C", "E", "I", "P", "S", "T"),
        new("D", "D", "L", "N", "O", "R"),
        new("D", "H", "H", "L", "O", "R"),
        new("D", "H", "H", "N", "O", "T"),
        new("D", "H", "L", "N", "O", "R"),
        new("E", "I", "I", "I", "T", "T"),
        new("E", "M", "O", "T", "T", "T"),
        new("E", "N", "S", "S", "S", "U"),
        new("F", "I", "P", "R", "S", "Y"),
        new("G", "O", "R", "R", "V", "W"),
        new("H", "I", "P", "R", "R", "Y"),
        new("N", "O", "O", "T", "U", "W"),
        new("O", "O", "O", "T", "T", "U"),
    };

    /// <summary>
    /// Gets the Super Big Boggle set of 36 dice (6x6 grid).
    /// Includes two-letter combo faces (QU, IN, TH, ER, HE, AN) and blocked faces.
    /// </summary>
    public static IReadOnlyList<Die> SuperBigBoggleDice { get; } = new Die[]
    {
        new("A", "A", "A", "F", "R", "S"),
        new("A", "A", "E", "E", "E", "E"),
        new("A", "A", "E", "E", "O", "O"),
        new("A", "A", "F", "I", "R", "S"),
        new("A", "B", "D", "E", "I", "O"),
        new("A", "D", "E", "N", "N", "N"),
        new("A", "E", "E", "E", "E", "M"),
        new("A", "E", "E", "G", "M", "U"),
        new("A", "E", "G", "M", "N", "N"),
        new("A", "E", "I", "L", "M", "N"),
        new("A", "E", "I", "N", "O", "U"),
        new("A", "F", "I", "R", "S", "Y"),
        new("QU", "IN", "TH", "ER", "HE", "AN"),
        new("B", "B", "J", "K", "X", "Z"),
        new("C", "C", "E", "N", "S", "T"),
        new("C", "D", "D", "L", "N", "N"),
        new("C", "E", "I", "I", "T", "T"),
        new("C", "E", "I", "P", "S", "T"),
        new("C", "F", "G", "N", "U", "Y"),
        new("D", "D", "H", "N", "O", "T"),
        new("D", "H", "H", "L", "O", "R"),
        new("D", "H", "H", "N", "O", "W"),
        new("D", "H", "L", "N", "O", "R"),
        new("E", "H", "I", "L", "R", "S"),
        new("E", "I", "I", "L", "S", "T"),
        new("E", "I", "L", "P", "S", "T"),
        new("E", "I", "O", BlockedFace, BlockedFace, BlockedFace),
        new("E", "M", "T", "T", "T", "O"),
        new("E", "N", "S", "S", "S", "U"),
        new("G", "O", "R", "R", "V", "W"),
        new("H", "I", "R", "S", "T", "V"),
        new("H", "O", "P", "R", "S", "T"),
        new("I", "P", "R", "S", "Y", "Y"),
        new("J", "K", "QU", "W", "X", "Z"),
        new("N", "O", "O", "T", "U", "W"),
        new("O", "O", "O", "T", "T", "U"),
    };

    /// <summary>
    /// Gets the dice set for the specified game mode.
    /// </summary>
    /// <param name="mode">The game mode.</param>
    /// <returns>The dice set for the mode.</returns>
    public static IReadOnlyList<Die> GetDiceForMode(GameMode mode)
    {
        return mode switch
        {
            GameMode.Standard => StandardDice,
            GameMode.BigBoggle => BigBoggleDice,
            GameMode.SuperBigBoggle => SuperBigBoggleDice,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown game mode."),
        };
    }
}
