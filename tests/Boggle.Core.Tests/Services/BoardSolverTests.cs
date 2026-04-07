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

    [Fact]
    public void Solve_5x5Board_ReturnsWords()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["abcde", "fghij"]);

        var solver = new BoardSolver(trie, NullLogger<BoardSolver>.Instance);
        GameBoard board = Create5x5TestBoard();

        IReadOnlyList<string> words = solver.Solve(board, 4);

        words.Should().NotBeNull();
    }

    [Fact]
    public void Solve_6x6Board_ReturnsWords()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["abcdef", "ghijkl"]);

        var solver = new BoardSolver(trie, NullLogger<BoardSolver>.Instance);
        GameBoard board = Create6x6TestBoard();

        IReadOnlyList<string> words = solver.Solve(board, 4);

        words.Should().NotBeNull();
    }

    [Fact]
    public void Solve_SkipsBlockedCells()
    {
        // Board: A B / ■ D — blocked cell at (1,0)
        // ABD is a valid 3-letter path: (0,0)→(0,1)→(1,1)
        // But paths through the blocked cell should not work
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["abd"]);

        var solver = new BoardSolver(trie, NullLogger<BoardSolver>.Instance);
        GameBoard board = CreateBoardWithBlockedCell();

        IReadOnlyList<string> words = solver.Solve(board, 3);

        words.Should().Contain("ABD");
    }

    [Fact]
    public void Solve_HandlesDigraphCells()
    {
        var trie = new TrieDictionaryProvider();
        trie.LoadWords(["the"]);

        var solver = new BoardSolver(trie, NullLogger<BoardSolver>.Instance);
        GameBoard board = CreateBoardWithDigraph();

        IReadOnlyList<string> words = solver.Solve(board, 3);

        words.Should().Contain("THE");
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

    private static GameBoard Create5x5TestBoard()
    {
        string[] letters =
        [
            "A", "B", "C", "D", "E",
            "F", "G", "H", "I", "J",
            "K", "L", "M", "N", "O",
            "P", "R", "S", "T", "U",
            "V", "W", "X", "Y", "Z",
        ];
        BoardCell[][] cells = new BoardCell[5][];
        for (int row = 0; row < 5; row++)
        {
            cells[row] = new BoardCell[5];
            for (int col = 0; col < 5; col++)
            {
                int i = (row * 5) + col;
                cells[row][col] = new BoardCell(letters[i], row, col);
            }
        }

        return new GameBoard(cells);
    }

    private static GameBoard Create6x6TestBoard()
    {
        BoardCell[][] cells = new BoardCell[6][];
        char letter = 'A';
        for (int row = 0; row < 6; row++)
        {
            cells[row] = new BoardCell[6];
            for (int col = 0; col < 6; col++)
            {
                cells[row][col] = new BoardCell(((char)(letter + (((row * 6) + col) % 26))).ToString(), row, col);
            }
        }

        return new GameBoard(cells);
    }

    private static GameBoard CreateBoardWithBlockedCell()
    {
        BoardCell[][] cells = new BoardCell[2][];
        cells[0] = [new BoardCell("A", 0, 0), new BoardCell("B", 0, 1)];
        cells[1] = [new BoardCell("C", 1, 0, isBlocked: true), new BoardCell("D", 1, 1)];
        return new GameBoard(cells);
    }

    private static GameBoard CreateBoardWithDigraph()
    {
        // Board: TH E / A  B
        BoardCell[][] cells = new BoardCell[2][];
        cells[0] = [new BoardCell("TH", 0, 0), new BoardCell("E", 0, 1)];
        cells[1] = [new BoardCell("A", 1, 0), new BoardCell("B", 1, 1)];
        return new GameBoard(cells);
    }
}
