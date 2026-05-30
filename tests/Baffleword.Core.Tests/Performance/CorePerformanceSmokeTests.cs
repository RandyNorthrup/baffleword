// <copyright file="CorePerformanceSmokeTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Tests.Performance;

using System.Diagnostics;
using Baffleword.Core.Dictionary;
using Baffleword.Core.Models;
using Baffleword.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

/// <summary>
/// Coarse performance smoke tests for core gameplay paths.
/// </summary>
public sealed class CorePerformanceSmokeTests
{
    [Fact]
    public void BoardGeneration_ThousandSuperBigBoards_CompletesWithinBudget()
    {
        var generator = new BoardGenerator(NullLogger<BoardGenerator>.Instance, new Random(123));
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 1_000; i++)
        {
            generator.Generate(GameMode.SuperBoard);
        }

        stopwatch.Stop();
        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(3));
    }

    [Fact]
    public void WordValidation_RepeatedBoardSearches_CompletesWithinBudget()
    {
        var dictionary = new TrieDictionaryProvider();
        dictionary.LoadWords(["abf", "aei", "cfi", "mnop", "aeim"]);
        var validator = new WordValidator(dictionary, NullLogger<WordValidator>.Instance);
        GameBoard board = CreateTestBoard();
        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < 5_000; i++)
        {
            validator.Validate("ABF", board, 3).Should().Be(WordStatus.Valid);
            validator.Validate("ZZZ", board, 3).Should().Be(WordStatus.NotInDictionary);
        }

        stopwatch.Stop();
        stopwatch.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(3));
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
}
