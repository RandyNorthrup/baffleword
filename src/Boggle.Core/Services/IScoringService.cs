// <copyright file="IScoringService.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Services;

/// <summary>
/// Calculates points for words based on Boggle scoring rules.
/// </summary>
public interface IScoringService
{
    /// <summary>
    /// Calculates the point value for a word based on its length.
    /// </summary>
    /// <param name="word">The word to score.</param>
    /// <returns>The point value.</returns>
    int CalculateWordScore(string word);

    /// <summary>
    /// Gets the effective letter count for a word, accounting for "QU" as 2 letters.
    /// </summary>
    /// <param name="word">The word to measure.</param>
    /// <returns>The effective letter count.</returns>
    int GetEffectiveLength(string word);
}
