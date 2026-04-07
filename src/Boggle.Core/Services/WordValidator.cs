// <copyright file="WordValidator.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Services;

using Boggle.Core.Dictionary;
using Boggle.Core.Models;
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
            _logger.LogDebug("Word '{Word}' not found in dictionary", normalizedWord);
            return WordStatus.NotInDictionary;
        }

        if (!CanTraceOnBoard(normalizedWord, board))
        {
            _logger.LogDebug("Word '{Word}' cannot be traced on board", normalizedWord);
            return WordStatus.NotOnBoard;
        }

        return WordStatus.Valid;
    }

    private static bool CanTraceOnBoard(string word, GameBoard board)
    {
        return board.AllCells.Where(c => !c.IsBlocked).Any(cell =>
            TryMatchFromCell(word, 0, cell, board, new HashSet<(int Row, int Column)>()));
    }

    private static bool TryMatchFromCell(string word, int charIndex, BoardCell cell, GameBoard board, HashSet<(int Row, int Column)> visited)
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

        visited.Add((cell.Row, cell.Column));

        bool found = board.GetNeighbors(cell)
            .Where(n => !n.IsBlocked && !visited.Contains((n.Row, n.Column)))
            .Any(neighbor => TryMatchFromCell(word, nextCharIndex, neighbor, board, visited));

        visited.Remove((cell.Row, cell.Column));
        return found;
    }
}
