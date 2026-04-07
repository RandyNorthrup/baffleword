// <copyright file="IWordValidator.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Services;

using Boggle.Core.Models;

/// <summary>
/// Validates words submitted by the player.
/// </summary>
public interface IWordValidator
{
    /// <summary>
    /// Validates a word against the dictionary and the current board.
    /// </summary>
    /// <param name="word">The word to validate.</param>
    /// <param name="board">The current game board.</param>
    /// <param name="minimumLength">The minimum word length required.</param>
    /// <returns>The validation status of the word.</returns>
    WordStatus Validate(string word, GameBoard board, int minimumLength);
}
