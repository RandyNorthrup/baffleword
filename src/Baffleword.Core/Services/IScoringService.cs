// <copyright file="IScoringService.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Services;

using Baffleword.Core.Models;

/// <summary>
/// Calculates points for words based on Baffleword scoring rules.
/// </summary>
public interface IScoringService
{
    /// <summary>
    /// Calculates the point value for a word based on its length and game mode.
    /// </summary>
    /// <param name="word">The word to score.</param>
    /// <param name="mode">The game mode.</param>
    /// <returns>The point value.</returns>
    int CalculateWordScore(string word, GameMode mode = GameMode.Standard);

    /// <summary>
    /// Gets the effective letter count for a word, accounting for "QU" as 2 letters.
    /// </summary>
    /// <param name="word">The word to measure.</param>
    /// <returns>The effective letter count.</returns>
    int GetEffectiveLength(string word);
}
