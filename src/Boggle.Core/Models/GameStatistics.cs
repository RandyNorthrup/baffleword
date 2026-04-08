// <copyright file="GameStatistics.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Represents aggregate lifetime player statistics.
/// </summary>
public sealed class GameStatistics
{
    /// <summary>
    /// Gets or sets the total number of rounds played.
    /// </summary>
    public int TotalRoundsPlayed { get; set; }

    /// <summary>
    /// Gets or sets the total score across all rounds.
    /// </summary>
    public long TotalScore { get; set; }

    /// <summary>
    /// Gets or sets the highest score in a single round.
    /// </summary>
    public int HighestRoundScore { get; set; }

    /// <summary>
    /// Gets or sets the total number of valid words found.
    /// </summary>
    public int TotalWordsFound { get; set; }

    /// <summary>
    /// Gets or sets the longest word ever found.
    /// </summary>
    public string LongestWordEver { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the best completion percentage achieved.
    /// </summary>
    public double BestCompletionPercentage { get; set; }

    /// <summary>
    /// Gets or sets the total play time.
    /// </summary>
    public TimeSpan TotalPlayTime { get; set; }
}
