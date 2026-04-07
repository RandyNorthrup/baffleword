// <copyright file="AchievementServiceTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Tests.Services;

using Boggle.Core.Models;
using Boggle.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public sealed class AchievementServiceTests
{
    private readonly AchievementService _sut = new(NullLogger<AchievementService>.Instance);

    [Fact]
    public void GetAllAchievements_Returns20Definitions()
    {
        IReadOnlyList<Achievement> achievements = _sut.GetAllAchievements();

        achievements.Should().HaveCount(26);
    }

    [Fact]
    public void CheckAchievements_FirstRound_UnlocksFirstWords()
    {
        GameRound round = CreateRoundWithWords("CAT");
        GameStatistics stats = new() { TotalRoundsPlayed = 1 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().Contain(a => a.Name == "First Words");
    }

    [Fact]
    public void CheckAchievements_20Words_UnlocksWordHoarder()
    {
        GameRound round = CreateRoundWithWordCount(20);
        GameStatistics stats = new() { TotalRoundsPlayed = 5 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().Contain(a => a.Name == "Word Hoarder");
    }

    [Fact]
    public void CheckAchievements_100Points_UnlocksCenturion()
    {
        GameRound round = CreateRoundWithScore(100);
        GameStatistics stats = new() { TotalRoundsPlayed = 5 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().Contain(a => a.Name == "Centurion");
    }

    [Fact]
    public void CheckAchievements_LongWord_UnlocksLongWord()
    {
        GameRound round = CreateRoundWithWords("ABCDEFG");
        GameStatistics stats = new() { TotalRoundsPlayed = 1 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().Contain(a => a.Name == "Long Word");
    }

    [Fact]
    public void CheckAchievements_MonsterWord_UnlocksMonsterWord()
    {
        GameRound round = CreateRoundWithWords("ABCDEFGH");
        GameStatistics stats = new() { TotalRoundsPlayed = 1 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().Contain(a => a.Name == "Monster Word");
    }

    [Fact]
    public void CheckAchievements_10Rounds_UnlocksDedicated()
    {
        GameRound round = CreateRoundWithWords("CAT");
        GameStatistics stats = new() { TotalRoundsPlayed = 10 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().Contain(a => a.Name == "Dedicated");
    }

    [Fact]
    public void CheckAchievements_NoMistakes_UnlocksNoMistakes()
    {
        GameRound round = CreateRoundWithWords("CAT", "DOG", "BAT");
        GameStatistics stats = new() { TotalRoundsPlayed = 1 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().Contain(a => a.Name == "No Mistakes");
    }

    [Fact]
    public void CheckAchievements_WithInvalidSubmission_DoesNotUnlockNoMistakes()
    {
        GameRound round = CreateRoundWithWords("CAT");
        round.AddWordResult(new WordResult("XYZ", WordStatus.NotInDictionary, 0));
        GameStatistics stats = new() { TotalRoundsPlayed = 1 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().NotContain(a => a.Name == "No Mistakes");
    }

    [Fact]
    public void CheckAchievements_FiveConsecutiveValid_UnlocksStreakMaster()
    {
        GameRound round = CreateRoundWithWords("AAA", "BBB", "CCC", "DDD", "EEE");
        GameStatistics stats = new() { TotalRoundsPlayed = 1 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().Contain(a => a.Name == "Streak Master");
    }

    [Fact]
    public void CheckAchievements_StreakBrokenByInvalid_DoesNotUnlockStreakMaster()
    {
        GameRound round = CreateRoundWithWords("AAA", "BBB");
        round.AddWordResult(new WordResult("XYZ", WordStatus.NotInDictionary, 0));
        round.AddWordResult(new WordResult("CCC", WordStatus.Valid, 1));
        round.AddWordResult(new WordResult("DDD", WordStatus.Valid, 1));
        GameStatistics stats = new() { TotalRoundsPlayed = 1 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().NotContain(a => a.Name == "Streak Master");
    }

    [Fact]
    public void CheckAchievements_ThreeQuWords_UnlocksQuMaster()
    {
        GameRound round = CreateRoundWithWords("QUEEN", "QUEST", "QUAIL");
        GameStatistics stats = new() { TotalRoundsPlayed = 1 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().Contain(a => a.Name == "Qu Master");
    }

    [Fact]
    public void CheckAchievements_AlreadyUnlocked_DoesNotUnlockAgain()
    {
        GameRound round = CreateRoundWithWords("CAT");
        GameStatistics stats = new() { TotalRoundsPlayed = 1 };

        _sut.CheckAchievements(round, stats);
        IReadOnlyList<Achievement> secondCheck = _sut.CheckAchievements(round, stats);

        secondCheck.Should().NotContain(a => a.Name == "First Words");
    }

    [Fact]
    public void LoadState_RestoresUnlockedAchievements()
    {
        var saved = new List<Achievement>
        {
            new() { Id = 1, Name = "First Words", IsUnlocked = true, UnlockedAt = DateTime.UtcNow },
        };

        _sut.LoadState(saved);

        _sut.GetAllAchievements().First(a => a.Id == 1).IsUnlocked.Should().BeTrue();
    }

    [Fact]
    public void CheckAchievements_500UniqueWords_UnlocksVocabularyBuilder()
    {
        GameRound round = CreateRoundWithWords("CAT");
        GameStatistics stats = new() { TotalRoundsPlayed = 50, UniqueWordsFound = 500 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().Contain(a => a.Name == "Vocabulary Builder");
    }

    [Fact]
    public void CheckAchievements_NewPersonalBest_UnlocksHighRoller()
    {
        GameRound round = CreateRoundWithScore(50);
        GameStatistics stats = new() { TotalRoundsPlayed = 5, HighestRoundScore = 40 };

        IReadOnlyList<Achievement> unlocked = _sut.CheckAchievements(round, stats);

        unlocked.Should().Contain(a => a.Name == "High Roller");
    }

    private static GameBoard CreateTestBoard()
    {
        string[] letters = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P"];
        BoardCell[][] cells = new BoardCell[4][];
        for (int row = 0; row < 4; row++)
        {
            cells[row] = new BoardCell[4];
            for (int col = 0; col < 4; col++)
            {
                int i = (row * 4) + col;
                cells[row][col] = new BoardCell(letters[i], row, col);
            }
        }

        return new GameBoard(cells);
    }

    private static GameRound CreateRoundWithWords(params string[] words)
    {
        GameRound round = new(CreateTestBoard(), TimeSpan.FromMinutes(3), 3);
        foreach (string word in words)
        {
            round.AddWordResult(new WordResult(word, WordStatus.Valid, 1));
        }

        return round;
    }

    private static GameRound CreateRoundWithWordCount(int count)
    {
        GameRound round = new(CreateTestBoard(), TimeSpan.FromMinutes(3), 3);
        for (int i = 0; i < count; i++)
        {
            round.AddWordResult(new WordResult($"WORD{i:D3}", WordStatus.Valid, 1));
        }

        return round;
    }

    private static GameRound CreateRoundWithScore(int targetScore)
    {
        GameRound round = new(CreateTestBoard(), TimeSpan.FromMinutes(3), 3);
        round.AddWordResult(new WordResult("TESTWORD", WordStatus.Valid, targetScore));
        return round;
    }
}
