// <copyright file="WordValidator.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Services;

using Baffleword.Core.Dictionary;
using Baffleword.Core.Models;
using Microsoft.Extensions.Logging;

/// <summary>
/// Validates words against the dictionary and checks board traceability via DFS.
/// </summary>
public sealed class WordValidator : IWordValidator
{
    private readonly IDictionaryProvider _dictionary;
    private readonly ILogger<WordValidator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WordValidator"/> class.
    /// </summary>
    /// <param name="dictionary">The dictionary provider for word lookup.</param>
    /// <param name="logger">The logger instance.</param>
    public WordValidator(IDictionaryProvider dictionary, ILogger<WordValidator> logger)
    {
        _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public WordStatus Validate(string word, GameBoard board, int minimumLength)
    {
        ArgumentNullException.ThrowIfNull(board);

        if (string.IsNullOrWhiteSpace(word))
        {
            return WordStatus.TooShort;
        }

        string normalizedWord = word.Trim().ToUpperInvariant();

        if (normalizedWord.Length < minimumLength)
        {
            return WordStatus.TooShort;
        }

        if (!_dictionary.IsValidWord(normalizedWord))
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Word '{Word}' not found in dictionary", normalizedWord);
            }

            return WordStatus.NotInDictionary;
        }

        if (!CanTraceOnBoard(normalizedWord, board))
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Word '{Word}' cannot be traced on board", normalizedWord);
            }

            return WordStatus.NotOnBoard;
        }

        return WordStatus.Valid;
    }

    private static bool CanTraceOnBoard(string word, GameBoard board)
    {
        bool[][] visited = CreateVisitedGrid(board);
        foreach (BoardCell cell in board.AllCells)
        {
            if (!cell.IsBlocked && TryMatchFromCell(word, 0, cell, board, visited))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryMatchFromCell(string word, int charIndex, BoardCell cell, GameBoard board, bool[][] visited)
    {
        if (charIndex >= word.Length || cell.IsBlocked)
        {
            return false;
        }

        // Check if this cell matches the current character(s)
        int charsConsumed;
        if (cell.IsDigraph)
        {
            // Multi-letter cell (QU, TH, IN, etc.) must match all letters at current position
            if (charIndex + cell.Letter.Length > word.Length)
            {
                return false;
            }

            for (int k = 0; k < cell.Letter.Length; k++)
            {
                if (word[charIndex + k] != cell.Letter[k])
                {
                    return false;
                }
            }

            charsConsumed = cell.Letter.Length;
        }
        else
        {
            if (word[charIndex] != cell.Letter[0])
            {
                return false;
            }

            charsConsumed = 1;
        }

        int nextCharIndex = charIndex + charsConsumed;

        if (nextCharIndex == word.Length)
        {
            return true;
        }

        visited[cell.Row][cell.Column] = true;

        foreach (BoardCell neighbor in board.GetNeighbors(cell))
        {
            if (!neighbor.IsBlocked &&
                !visited[neighbor.Row][neighbor.Column] &&
                TryMatchFromCell(word, nextCharIndex, neighbor, board, visited))
            {
                visited[cell.Row][cell.Column] = false;
                return true;
            }
        }

        visited[cell.Row][cell.Column] = false;
        return false;
    }

    private static bool[][] CreateVisitedGrid(GameBoard board)
    {
        bool[][] visited = new bool[board.Rows][];
        for (int row = 0; row < board.Rows; row++)
        {
            visited[row] = new bool[board.Columns];
        }

        return visited;
    }
}
