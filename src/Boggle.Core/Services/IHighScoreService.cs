// <copyright file="IHighScoreService.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Services;

using Boggle.Core.Models;

/// <summary>
/// Manages high score records.
/// </summary>
public interface IHighScoreService
{
    /// <summary>
    /// Attempts to record a new high score from the completed round.
    /// </summary>
    /// <param name="round">The completed game round.</param>
    /// <returns>The high score entry if it qualified, or <see langword="null"/> if it did not.</returns>
    Task<HighScoreEntry?> TryRecordScoreAsync(GameRound round);

    /// <summary>
    /// Gets the top scores for a given timer duration.
    /// </summary>
    /// <param name="timerDuration">The timer duration category.</param>
    /// <param name="count">The maximum number of scores to return.</param>
    /// <returns>The top scores sorted by score descending.</returns>
    Task<IReadOnlyList<HighScoreEntry>> GetTopScoresAsync(TimeSpan timerDuration, int count = 50);
}
