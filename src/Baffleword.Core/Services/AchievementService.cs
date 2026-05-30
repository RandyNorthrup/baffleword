// <copyright file="AchievementService.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Services;

using Baffleword.Core.Models;
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
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Achievement unlocked: {Name}", achievement.Name);
                }
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
            8 => CountValidWordsInFirstMinute(round) >= 10,                          // Speed Demon
            9 => round.CompletionPercentage >= 50.0,                                 // Perfectionist
            10 => round.CompletionPercentage >= 75.0,                                // Completionist
            11 => stats.TotalRoundsPlayed >= 10,                                     // Dedicated
            12 => stats.TotalRoundsPlayed >= 50,                                     // Veteran
            13 => stats.TotalRoundsPlayed >= 100,                                    // Marathon
            14 => round.Score > stats.HighestRoundScore,                             // High Roller
            15 => HasValidWordWithinSeconds(round, 5),                               // Quick Draw
            16 => stats.TotalWordsFound >= 500,                                     // Vocabulary Builder
            17 => stats.TotalWordsFound >= 1000,                                    // Word Scholar
            18 => HasConsecutiveValidStreak(round, 5),                               // Streak Master
            19 => round.InvalidSubmissionCount == 0 && round.ValidWordCount > 0,     // No Mistakes
            20 => CountQuWords(round) >= 3,                                          // Qu Master
            21 => round.Mode == GameMode.BigBoard && stats.TotalRoundsPlayed >= 1,  // Big Thinker
            22 => round.Mode == GameMode.SuperBoard && stats.TotalRoundsPlayed >= 1, // Super Solver
            23 => round.Mode == GameMode.BigBoard && round.Score >= 150,            // Big Score
            24 => round.Mode == GameMode.SuperBoard && round.Score >= 150,       // Super Score
            25 => round.Mode == GameMode.BigBoard && round.ValidWordCount >= 25,    // Big Word Finder
            26 => round.Mode == GameMode.SuperBoard && round.ValidWordCount >= 25, // Super Word Finder
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

    private static int CountValidWordsInFirstMinute(GameRound round)
    {
        DateTime cutoff = round.StartedAt.AddSeconds(60);
        return round.SubmittedWords.Count(w =>
            w.Status == WordStatus.Valid && w.SubmittedAt <= cutoff);
    }

    private static bool HasValidWordWithinSeconds(GameRound round, int seconds)
    {
        DateTime cutoff = round.StartedAt.AddSeconds(seconds);
        return round.SubmittedWords.Any(w =>
            w.Status == WordStatus.Valid && w.SubmittedAt <= cutoff);
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
            new() { Id = 16, Name = "Vocabulary Builder", Description = "Find 500 words (lifetime)" },
            new() { Id = 17, Name = "Word Scholar", Description = "Find 1000 words (lifetime)" },
            new() { Id = 18, Name = "Streak Master", Description = "Find 5 valid words in a row" },
            new() { Id = 19, Name = "No Mistakes", Description = "Complete a round with no invalid submissions" },
            new() { Id = 20, Name = "Qu Master", Description = "Find 3 words containing \"Qu\" in one round" },
            new() { Id = 21, Name = "Big Thinker", Description = "Complete a Big Board round" },
            new() { Id = 22, Name = "Super Solver", Description = "Complete a Super Board round" },
            new() { Id = 23, Name = "Big Score", Description = "Score 150+ points in a Big Board round" },
            new() { Id = 24, Name = "Super Score", Description = "Score 150+ points in a Super Board round" },
            new() { Id = 25, Name = "Big Word Finder", Description = "Find 25 words in a Big Board round" },
            new() { Id = 26, Name = "Super Word Finder", Description = "Find 25 words in a Super Board round" },
        ];
    }
}
