// <copyright file="IBoardGenerator.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Services;

using Baffleword.Core.Models;

/// <summary>
/// Generates Baffleword game boards.
/// </summary>
public interface IBoardGenerator
{
    /// <summary>
    /// Generates a new randomized game board for the specified game mode.
    /// </summary>
    /// <param name="mode">The game mode determining board size and dice.</param>
    /// <returns>A new <see cref="GameBoard"/> with randomly rolled dice.</returns>
    GameBoard Generate(GameMode mode = GameMode.Standard);
}
