// <copyright file="IBoardGenerator.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Services;

using Boggle.Core.Models;

/// <summary>
/// Generates Boggle game boards from the official dice set.
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
