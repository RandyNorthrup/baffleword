// <copyright file="IHighScoreRepository.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Repositories;

using Boggle.Core.Models;

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
    /// Gets the top scores for a timer duration category.
    /// </summary>
    /// <param name="timerDurationSeconds">The timer duration in seconds.</param>
    /// <param name="count">The maximum number of results.</param>
    /// <returns>The top scores.</returns>
    Task<IReadOnlyList<HighScoreEntry>> GetTopAsync(int timerDurationSeconds, int count);

    /// <summary>
    /// Gets the minimum score currently in the top N for a given timer duration.
    /// </summary>
    /// <param name="timerDurationSeconds">The timer duration in seconds.</param>
    /// <param name="topN">The number of top positions to consider.</param>
    /// <returns>The minimum score in the top N, or 0 if fewer than N entries exist.</returns>
    Task<int> GetMinimumTopScoreAsync(int timerDurationSeconds, int topN);

    /// <summary>
    /// Deletes all high score entries.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ClearAllAsync();
}
