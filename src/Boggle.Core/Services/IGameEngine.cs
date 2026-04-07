// <copyright file="IGameEngine.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Services;

using Boggle.Core.Models;

/// <summary>
/// Orchestrates the Boggle game round lifecycle.
/// </summary>
public interface IGameEngine
{
    /// <summary>
    /// Gets the current game round, or <see langword="null"/> if no round is active.
    /// </summary>
    GameRound? CurrentRound { get; }

    /// <summary>
    /// Starts a new game round.
    /// </summary>
    /// <param name="timerDuration">The timer duration for the round.</param>
    /// <param name="minimumWordLength">The minimum word length.</param>
    /// <returns>The newly created game round.</returns>
    GameRound StartRound(TimeSpan timerDuration, int minimumWordLength);

    /// <summary>
    /// Submits a word for validation in the current round.
    /// </summary>
    /// <param name="word">The word to submit.</param>
    /// <returns>The validation result.</returns>
    WordResult SubmitWord(string word);

    /// <summary>
    /// Ends the current round and solves the board.
    /// </summary>
    /// <returns>The completed game round with solve results.</returns>
    GameRound EndRound();

    /// <summary>
    /// Pauses the current round.
    /// </summary>
    void PauseRound();

    /// <summary>
    /// Resumes the current round.
    /// </summary>
    void ResumeRound();
}
