// <copyright file="ScoringService.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Services;

/// <summary>
/// Calculates Boggle word scores according to official rules.
/// </summary>
public sealed class ScoringService : IScoringService
{
    /// <inheritdoc/>
    public int CalculateWordScore(string word)
    {
        int length = GetEffectiveLength(word);
        return length switch
        {
            < 3 => 0,
            3 or 4 => 1,
            5 => 2,
            6 => 3,
            7 => 5,
            _ => 11,
        };
    }

    /// <inheritdoc/>
    public int GetEffectiveLength(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return 0;
        }

        // The word itself IS the effective length; "QU" on a die represents
        // 2 letters but the word spelling already contains both Q and U.
        return word.Length;
    }
}
