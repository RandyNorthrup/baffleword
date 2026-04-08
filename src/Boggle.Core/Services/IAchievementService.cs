// <copyright file="IAchievementService.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Services;

using Boggle.Core.Models;

/// <summary>
/// Checks and unlocks achievements based on game events.
/// </summary>
public interface IAchievementService
{
    /// <summary>
    /// Checks all achievement conditions against the completed round and statistics.
    /// </summary>
    /// <param name="round">The completed game round.</param>
    /// <param name="statistics">The player's lifetime statistics.</param>
    /// <returns>A list of newly unlocked achievements.</returns>
    IReadOnlyList<Achievement> CheckAchievements(GameRound round, GameStatistics statistics);

    /// <summary>
    /// Gets all achievement definitions.
    /// </summary>
    /// <returns>All achievements with their current unlock state.</returns>
    IReadOnlyList<Achievement> GetAllAchievements();
}
