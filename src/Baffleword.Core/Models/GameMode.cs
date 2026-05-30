// <copyright file="GameMode.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Models;

/// <summary>
/// Defines the available Baffleword game modes.
/// </summary>
public enum GameMode
{
    /// <summary>
    /// Standard: 4x4 grid, 16 dice, minimum 3-letter words.
    /// </summary>
    Standard,

    /// <summary>
    /// Big Board: 5x5 grid, 25 dice, minimum 4-letter words.
    /// </summary>
    BigBoard,

    /// <summary>
    /// Super Board: 6x6 grid, 36 dice, minimum 4-letter words,
    /// includes two-letter combo dice and blocked positions.
    /// </summary>
    SuperBoard,
}
