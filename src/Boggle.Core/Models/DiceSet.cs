// <copyright file="DiceSet.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Contains the official "New Boggle" 16-die set (Hasbro #C2187).
/// </summary>
public static class DiceSet
{
    /// <summary>
    /// Gets the standard set of 16 Boggle dice.
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
    /// Gets the number of dice in a standard Boggle set.
    /// </summary>
    public static int Count => StandardDice.Count;
}
