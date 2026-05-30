// <copyright file="IHighScoreRepository.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Repositories;

using Baffleword.Core.Models;

/// <summary>
/// Repository for high score data access.
/// </summary>
public interface IHighScoreRepository
{
    /// <summary>
    /// Adds a new high score entry.
    /// </summary>
    /// <param name="entry">The entry to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(HighScoreEntry entry);

    /// <summary>
    /// Gets the top scores for a game mode.
    /// </summary>
    /// <param name="gameMode">The game mode.</param>
    /// <param name="count">The maximum number of results.</param>
    /// <returns>The top scores.</returns>
    Task<IReadOnlyList<HighScoreEntry>> GetTopAsync(string gameMode, int count);

    /// <summary>
    /// Gets the minimum score currently in the top N for a given game mode.
    /// </summary>
    /// <param name="gameMode">The game mode.</param>
    /// <param name="topN">The number of top positions to consider.</param>
    /// <returns>The minimum score in the top N, or 0 if fewer than N entries exist.</returns>
    Task<int> GetMinimumTopScoreAsync(string gameMode, int topN);
}
