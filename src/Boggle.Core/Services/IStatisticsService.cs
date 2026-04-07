// <copyright file="IStatisticsService.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Services;

using Boggle.Core.Models;

/// <summary>
/// Tracks and retrieves lifetime player statistics.
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// Updates statistics after a completed round.
    /// </summary>
    /// <param name="round">The completed game round.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateStatisticsAsync(GameRound round);

    /// <summary>
    /// Gets the current player statistics.
    /// </summary>
    /// <returns>The player's lifetime statistics.</returns>
    Task<GameStatistics> GetStatisticsAsync();
}
