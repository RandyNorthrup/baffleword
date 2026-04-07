// <copyright file="GameRound.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Represents the state of a single Boggle game round.
/// </summary>
public sealed class GameRound
{
    private readonly List<WordResult> _submittedWords = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="GameRound"/> class.
    /// </summary>
    /// <param name="board">The game board for this round.</param>
    /// <param name="timerDuration">The total timer duration.</param>
    /// <param name="minimumWordLength">The minimum word length required.</param>
    /// <param name="mode">The game mode for this round.</param>
    public GameRound(GameBoard board, TimeSpan timerDuration, int minimumWordLength, GameMode mode = GameMode.Standard)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentOutOfRangeException.ThrowIfLessThan(minimumWordLength, 3);

        Board = board;
        TimerDuration = timerDuration;
        MinimumWordLength = minimumWordLength;
        Mode = mode;
        StartedAt = DateTime.UtcNow;
        State = GameRoundState.Playing;
    }

    /// <summary>
    /// Gets the game board for this round.
    /// </summary>
    public GameBoard Board { get; }

    /// <summary>
    /// Gets the game mode for this round.
    /// </summary>
    public GameMode Mode { get; }

    /// <summary>
    /// Gets the timer duration.
    /// </summary>
    public TimeSpan TimerDuration { get; }

    /// <summary>
    /// Gets the minimum word length.
    /// </summary>
    public int MinimumWordLength { get; }

    /// <summary>
    /// Gets the time the round started.
    /// </summary>
    public DateTime StartedAt { get; }

    /// <summary>
    /// Gets or sets the current state of the round.
    /// </summary>
    public GameRoundState State { get; set; }

    /// <summary>
    /// Gets the list of submitted word results.
    /// </summary>
    public IReadOnlyList<WordResult> SubmittedWords => _submittedWords;

    /// <summary>
    /// Gets the current score for this round.
    /// </summary>
    public int Score => _submittedWords.Where(w => w.Status == WordStatus.Valid).Sum(w => w.Points);

    /// <summary>
    /// Gets the number of valid words found.
    /// </summary>
    public int ValidWordCount => _submittedWords.Count(w => w.Status == WordStatus.Valid);

    /// <summary>
    /// Gets the number of invalid submissions.
    /// </summary>
    public int InvalidSubmissionCount => _submittedWords.Count(w => w.Status != WordStatus.Valid);

    /// <summary>
    /// Gets or sets the total possible words on this board (set after solving).
    /// </summary>
    public int TotalPossibleWords { get; set; }

    /// <summary>
    /// Gets or sets all possible words on this board (set after solving).
    /// </summary>
    public IReadOnlyList<string>? AllPossibleWords { get; set; }

    /// <summary>
    /// Gets the completion percentage (valid words found / total possible).
    /// </summary>
    public double CompletionPercentage =>
        TotalPossibleWords > 0 ? (double)ValidWordCount / TotalPossibleWords * 100.0 : 0.0;

    /// <summary>
    /// Adds a word result to this round.
    /// </summary>
    /// <param name="result">The word result to add.</param>
    public void AddWordResult(WordResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        _submittedWords.Add(result);
    }

    /// <summary>
    /// Checks whether a word has already been submitted (case-insensitive).
    /// </summary>
    /// <param name="word">The word to check.</param>
    /// <returns><see langword="true"/> if the word was already submitted; otherwise, <see langword="false"/>.</returns>
    public bool HasBeenSubmitted(string word)
    {
        return _submittedWords.Any(w =>
            w.Status == WordStatus.Valid &&
            string.Equals(w.Word, word, StringComparison.OrdinalIgnoreCase));
    }
}
