// <copyright file="IBoardSolver.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Services;

using Boggle.Core.Models;

/// <summary>
/// Finds all valid words on a Boggle board.
/// </summary>
public interface IBoardSolver
{
    /// <summary>
    /// Finds all valid words on the given board.
    /// </summary>
    /// <param name="board">The game board to solve.</param>
    /// <param name="minimumLength">The minimum word length to find.</param>
    /// <returns>A sorted list of all valid words that can be found on the board.</returns>
    IReadOnlyList<string> Solve(GameBoard board, int minimumLength);
}
