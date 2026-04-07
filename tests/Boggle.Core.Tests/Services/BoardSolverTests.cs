// <copyright file="BoardSolverTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Tests.Services;

using Boggle.Core.Dictionary;
using Boggle.Core.Models;
using Boggle.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

/// <summary>
/// Tests for the <see cref="BoardSolver"/> class.
/// </summary>
public sealed class BoardSolverTests
{
    [Fact]
    public void Solve_ReturnsWordsFromBoard()
    {
        // Set up trie with known words that exist on the test board
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["abc", "abf", "bfk", "efg"]);

        var solver = new BoardSolver(trie, NullLogger<BoardSolver>.Instance);
        GameBoard board = CreateTestBoard();

        IReadOnlyList<string> words = solver.Solve(board, 3);

        words.Should().NotBeEmpty();
    }

    [Fact]
    public void Solve_RespectsMinimumWordLength()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["ab", "abc", "abcd"]);

        var solver = new BoardSolver(trie, NullLogger<BoardSolver>.Instance);
        GameBoard board = CreateTestBoard();

        IReadOnlyList<string> words = solver.Solve(board, 3);

        words.Should().AllSatisfy(w => w.Length.Should().BeGreaterThanOrEqualTo(3));
    }

    [Fact]
    public void Solve_DoesNotRevisitCells()
    {
        // "ABA" requires revisiting cell (0,0) which shouldn't be allowed
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["aba"]);

        var solver = new BoardSolver(trie, NullLogger<BoardSolver>.Instance);
        GameBoard board = CreateTestBoard();

        IReadOnlyList<string> words = solver.Solve(board, 3);

        words.Should().NotContain("ABA");
    }

    [Fact]
    public void Solve_WithEmptyDictionary_ReturnsEmpty()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords([]);

        var solver = new BoardSolver(trie, NullLogger<BoardSolver>.Instance);
        GameBoard board = CreateTestBoard();

        IReadOnlyList<string> words = solver.Solve(board, 3);

        words.Should().BeEmpty();
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
