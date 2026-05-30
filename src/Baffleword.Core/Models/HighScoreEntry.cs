// <copyright file="HighScoreEntry.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Models;

/// <summary>
/// Represents a single high score record.
/// </summary>
public sealed class HighScoreEntry
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the total score.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the number of valid words found.
    /// </summary>
    public int WordsFound { get; set; }

    /// <summary>
    /// Gets or sets the longest word found.
    /// </summary>
    public string LongestWord { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the completion percentage.
    /// </summary>
    public double CompletionPercentage { get; set; }

    /// <summary>
    /// Gets or sets the timer duration used for this round.
    /// </summary>
    public TimeSpan TimerDuration { get; set; }

    /// <summary>
    /// Gets or sets the game mode used for this round.
    /// </summary>
    public GameMode GameMode { get; set; }

    /// <summary>
    /// Gets or sets when this score was achieved.
    /// </summary>
    public DateTime AchievedAt { get; set; }
}
