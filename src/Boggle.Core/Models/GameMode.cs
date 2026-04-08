// <copyright file="GameMode.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Defines the available Boggle game modes.
/// </summary>
public enum GameMode
{
    /// <summary>
    /// Standard Boggle: 4x4 grid, 16 dice, minimum 3-letter words.
    /// </summary>
    Standard,

    /// <summary>
    /// Big Boggle: 5x5 grid, 25 dice, minimum 4-letter words.
    /// </summary>
    BigBoggle,

    /// <summary>
    /// Super Big Boggle: 6x6 grid, 36 dice, minimum 4-letter words,
    /// includes two-letter combo dice and blocked positions.
    /// </summary>
    SuperBigBoggle,
}
