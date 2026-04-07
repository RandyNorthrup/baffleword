// <copyright file="BoardSolver.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Services;

using System.Text;
using Boggle.Core.Dictionary;
using Boggle.Core.Models;
using Microsoft.Extensions.Logging;

/// <summary>
/// Finds all valid words on a Boggle board using Trie-guided DFS.
/// </summary>
public sealed class BoardSolver : IBoardSolver
{
    private readonly IDictionaryProvider _dictionary;
    private readonly ILogger<BoardSolver> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoardSolver"/> class.
    /// </summary>
    /// <param name="dictionary">The dictionary provider with prefix checking.</param>
    /// <param name="logger">The logger instance.</param>
    public BoardSolver(IDictionaryProvider dictionary, ILogger<BoardSolver> logger)
    {
        _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> Solve(GameBoard board, int minimumLength)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentOutOfRangeException.ThrowIfLessThan(minimumLength, 3);

        _logger.LogDebug("Solving board with minimum word length {MinLength}", minimumLength);

        HashSet<string> foundWords = new(StringComparer.OrdinalIgnoreCase);
        bool[][] visited = new bool[board.Rows][];
        for (int i = 0; i < board.Rows; i++)
        {
            visited[i] = new bool[board.Columns];
        }

        StringBuilder currentWord = new(board.TotalCells);

        foreach (BoardCell cell in board.AllCells)
        {
            if (!cell.IsBlocked)
            {
                DfsSolve(board, cell, visited, currentWord, foundWords, minimumLength);
            }
        }

        List<string> result = [.. foundWords.OrderBy(w => w.Length).ThenBy(w => w, StringComparer.OrdinalIgnoreCase)];
        _logger.LogDebug("Found {Count} valid words on board", result.Count);
        return result;
    }

    private void DfsSolve(
        GameBoard board,
        BoardCell cell,
        bool[][] visited,
        StringBuilder currentWord,
        HashSet<string> foundWords,
        int minimumLength)
    {
        visited[cell.Row][cell.Column] = true;
        currentWord.Append(cell.Letter);

        string prefix = currentWord.ToString();

        if (_dictionary.HasPrefix(prefix))
        {
            if (prefix.Length >= minimumLength && _dictionary.IsValidWord(prefix))
            {
                foundWords.Add(prefix);
            }

            foreach (BoardCell neighbor in board.GetNeighbors(cell))
            {
                if (!neighbor.IsBlocked && !visited[neighbor.Row][neighbor.Column])
                {
                    DfsSolve(board, neighbor, visited, currentWord, foundWords, minimumLength);
                }
            }
        }

        // Backtrack
        currentWord.Length -= cell.Letter.Length;
        visited[cell.Row][cell.Column] = false;
    }
}
