// <copyright file="GameMode.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Defines the available Boggle game modes.
/// </summary>
public enum GameMode
{
    /// <summary>
    /// Standard Boggle: 4×4 grid, 16 dice, minimum 3-letter words.
    /// </summary>
    Standard,

    /// <summary>
    /// Big Boggle: 5×5 grid, 25 dice, minimum 4-letter words.
    /// </summary>
    BigBoggle,

    /// <summary>
    /// Super Big Boggle: 6×6 grid, 36 dice, minimum 4-letter words,
    /// includes two-letter combo dice and blocked positions.
    /// </summary>
    SuperBigBoggle,
}
