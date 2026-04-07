// <copyright file="AchievementService.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Services;

using Boggle.Core.Models;
using Microsoft.Extensions.Logging;

/// <summary>
/// Checks and unlocks achievements based on game events.
/// </summary>
public sealed class AchievementService : IAchievementService
{
    private readonly ILogger<AchievementService> _logger;
    private readonly List<Achievement> _achievements;

    /// <summary>
    /// Initializes a new instance of the <see cref="AchievementService"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public AchievementService(ILogger<AchievementService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _achievements = CreateAchievementDefinitions();
    }

    /// <inheritdoc/>
    public IReadOnlyList<Achievement> GetAllAchievements() => _achievements;

    /// <inheritdoc/>
    public IReadOnlyList<Achievement> CheckAchievements(GameRound round, GameStatistics statistics)
    {
        ArgumentNullException.ThrowIfNull(round);
        ArgumentNullException.ThrowIfNull(statistics);

        var newlyUnlocked = new List<Achievement>();

        foreach (Achievement achievement in _achievements)
        {
            if (achievement.IsUnlocked)
            {
                continue;
            }

            if (IsConditionMet(achievement.Id, round, statistics))
            {
                achievement.IsUnlocked = true;
                achievement.UnlockedAt = DateTime.UtcNow;
                newlyUnlocked.Add(achievement);
                _logger.LogInformation("Achievement unlocked: {Name}", achievement.Name);
            }
        }

        return newlyUnlocked;
    }

    /// <summary>
    /// Loads previously unlocked achievement states from persistence.
    /// </summary>
    /// <param name="savedAchievements">The saved achievement states.</param>
    public void LoadState(IReadOnlyList<Achievement> savedAchievements)
    {
        ArgumentNullException.ThrowIfNull(savedAchievements);

        foreach (Achievement saved in savedAchievements)
        {
            Achievement? definition = _achievements.Find(a => a.Id == saved.Id);
            if (definition is not null)
            {
                definition.IsUnlocked = saved.IsUnlocked;
                definition.UnlockedAt = saved.UnlockedAt;
            }
        }
    }

    private static bool IsConditionMet(int achievementId, GameRound round, GameStatistics stats)
    {
        return achievementId switch
        {
            1 => stats.TotalRoundsPlayed >= 1,                                      // First Words
            2 => round.ValidWordCount >= 20,                                         // Word Hoarder
            3 => round.ValidWordCount >= 30,                                         // Linguist
            4 => round.Score >= 100,                                                 // Centurion
            5 => round.Score >= 200,                                                 // Double Century
            6 => HasWordOfLength(round, 7),                                          // Long Word
            7 => HasWordOfLength(round, 8),                                          // Monster Word
            8 => false,                                                              // Speed Demon (checked externally with timing)
            9 => round.CompletionPercentage >= 50.0,                                 // Perfectionist
            10 => round.CompletionPercentage >= 75.0,                                // Completionist
            11 => stats.TotalRoundsPlayed >= 10,                                     // Dedicated
            12 => stats.TotalRoundsPlayed >= 50,                                     // Veteran
            13 => stats.TotalRoundsPlayed >= 100,                                    // Marathon
            14 => round.Score > stats.HighestRoundScore,                             // High Roller
            15 => false,                                                              // Quick Draw (checked externally with timing)
            16 => stats.UniqueWordsFound >= 500,                                     // Vocabulary Builder
            17 => stats.UniqueWordsFound >= 1000,                                    // Word Scholar
            18 => HasConsecutiveValidStreak(round, 5),                               // Streak Master
            19 => round.InvalidSubmissionCount == 0 && round.ValidWordCount > 0,     // No Mistakes
            20 => CountQuWords(round) >= 3,                                          // Qu Master
            _ => false,
        };
    }

    private static bool HasWordOfLength(GameRound round, int minLength)
    {
        return round.SubmittedWords.Any(w =>
            w.Status == WordStatus.Valid && w.Word.Length >= minLength);
    }

    private static bool HasConsecutiveValidStreak(GameRound round, int requiredStreak)
    {
        int streak = 0;
        foreach (WordResult word in round.SubmittedWords)
        {
            if (word.Status == WordStatus.Valid)
            {
                streak++;
                if (streak >= requiredStreak)
                {
                    return true;
                }
            }
            else
            {
                streak = 0;
            }
        }

        return false;
    }

    private static int CountQuWords(GameRound round)
    {
        return round.SubmittedWords.Count(w =>
            w.Status == WordStatus.Valid &&
            w.Word.Contains("QU", StringComparison.OrdinalIgnoreCase));
    }

    private static List<Achievement> CreateAchievementDefinitions()
    {
        return
        [
            new() { Id = 1, Name = "First Words", Description = "Complete your first round" },
            new() { Id = 2, Name = "Word Hoarder", Description = "Find 20 words in a single round" },
            new() { Id = 3, Name = "Linguist", Description = "Find 30 words in a single round" },
            new() { Id = 4, Name = "Centurion", Description = "Score 100+ points in a single round" },
            new() { Id = 5, Name = "Double Century", Description = "Score 200+ points in a single round" },
            new() { Id = 6, Name = "Long Word", Description = "Find a word with 7+ letters" },
            new() { Id = 7, Name = "Monster Word", Description = "Find a word with 8+ letters" },
            new() { Id = 8, Name = "Speed Demon", Description = "Find 10 words in the first minute" },
            new() { Id = 9, Name = "Perfectionist", Description = "Find 50%+ of all possible words" },
            new() { Id = 10, Name = "Completionist", Description = "Find 75%+ of all possible words" },
            new() { Id = 11, Name = "Dedicated", Description = "Play 10 rounds" },
            new() { Id = 12, Name = "Veteran", Description = "Play 50 rounds" },
            new() { Id = 13, Name = "Marathon", Description = "Play 100 rounds" },
            new() { Id = 14, Name = "High Roller", Description = "Achieve a new personal best score" },
            new() { Id = 15, Name = "Quick Draw", Description = "Submit a valid word within 5 seconds" },
            new() { Id = 16, Name = "Vocabulary Builder", Description = "Find 500 unique words (lifetime)" },
            new() { Id = 17, Name = "Word Scholar", Description = "Find 1000 unique words (lifetime)" },
            new() { Id = 18, Name = "Streak Master", Description = "Find 5 valid words in a row" },
            new() { Id = 19, Name = "No Mistakes", Description = "Complete a round with no invalid submissions" },
            new() { Id = 20, Name = "Qu Master", Description = "Find 3 words containing \"Qu\" in one round" },
        ];
    }
}
