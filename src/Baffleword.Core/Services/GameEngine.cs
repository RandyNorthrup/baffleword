// <copyright file="GameEngine.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Services;

using Baffleword.Core.Exceptions;
using Baffleword.Core.Models;
using Microsoft.Extensions.Logging;

/// <summary>
/// Orchestrates the Baffleword game round lifecycle.
/// </summary>
public sealed class GameEngine : IGameEngine
{
    private readonly IBoardGenerator _boardGenerator;
    private readonly IWordValidator _wordValidator;
    private readonly IBoardSolver _boardSolver;
    private readonly IScoringService _scoringService;
    private readonly ILogger<GameEngine> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameEngine"/> class.
    /// </summary>
    /// <param name="boardGenerator">The board generator.</param>
    /// <param name="wordValidator">The word validator.</param>
    /// <param name="boardSolver">The board solver.</param>
    /// <param name="scoringService">The scoring service.</param>
    /// <param name="logger">The logger instance.</param>
    public GameEngine(
        IBoardGenerator boardGenerator,
        IWordValidator wordValidator,
        IBoardSolver boardSolver,
        IScoringService scoringService,
        ILogger<GameEngine> logger)
    {
        _boardGenerator = boardGenerator ?? throw new ArgumentNullException(nameof(boardGenerator));
        _wordValidator = wordValidator ?? throw new ArgumentNullException(nameof(wordValidator));
        _boardSolver = boardSolver ?? throw new ArgumentNullException(nameof(boardSolver));
        _scoringService = scoringService ?? throw new ArgumentNullException(nameof(scoringService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public GameRound? CurrentRound { get; private set; }

    /// <inheritdoc/>
    public GameRound StartRound(TimeSpan timerDuration, int minimumWordLength, GameMode mode = GameMode.Standard)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Starting new {Mode} round with {Duration} timer and min length {MinLength}", mode, timerDuration, minimumWordLength);
        }

        GameBoard board = _boardGenerator.Generate(mode);
        CurrentRound = new GameRound(board, timerDuration, minimumWordLength, mode);
        return CurrentRound;
    }

    /// <inheritdoc/>
    public WordResult SubmitWord(string word)
    {
        if (CurrentRound is null || CurrentRound.State != GameRoundState.Playing)
        {
            throw new GameStateException("No active round to submit words to.");
        }

        string normalizedWord = word.Trim().ToUpperInvariant();

        // Check for duplicate first
        if (CurrentRound.HasBeenSubmitted(normalizedWord))
        {
            var duplicateResult = new WordResult(normalizedWord, WordStatus.AlreadyFound, 0);
            CurrentRound.AddWordResult(duplicateResult);
            return duplicateResult;
        }

        WordStatus status = _wordValidator.Validate(normalizedWord, CurrentRound.Board, CurrentRound.MinimumWordLength);
        int points = status == WordStatus.Valid ? _scoringService.CalculateWordScore(normalizedWord, CurrentRound.Mode) : 0;

        var result = new WordResult(normalizedWord, status, points);
        CurrentRound.AddWordResult(result);

        if (status == WordStatus.Valid && _logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Valid word '{Word}' submitted for {Points} points", normalizedWord, points);
        }

        return result;
    }

    /// <inheritdoc/>
    public GameRound EndRound()
    {
        if (CurrentRound is null)
        {
            throw new GameStateException("No active round to end.");
        }

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Ending round with score {Score}", CurrentRound.Score);
        }

        CurrentRound.State = GameRoundState.Ended;

        // Solve the board to find all possible words
        IReadOnlyList<string> allWords = _boardSolver.Solve(CurrentRound.Board, CurrentRound.MinimumWordLength);
        CurrentRound.AllPossibleWords = allWords;
        CurrentRound.TotalPossibleWords = allWords.Count;

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(
                "Round complete: {FoundWords}/{TotalWords} words ({Percentage:F1}%)",
                CurrentRound.ValidWordCount,
                CurrentRound.TotalPossibleWords,
                CurrentRound.CompletionPercentage);
        }

        GameRound completedRound = CurrentRound;
        CurrentRound = null;
        return completedRound;
    }

    /// <inheritdoc/>
    public void PauseRound()
    {
        if (CurrentRound is null || CurrentRound.State != GameRoundState.Playing)
        {
            throw new GameStateException("No active round to pause.");
        }

        CurrentRound.State = GameRoundState.Paused;
        _logger.LogDebug("Round paused");
    }

    /// <inheritdoc/>
    public void ResumeRound()
    {
        if (CurrentRound is null || CurrentRound.State != GameRoundState.Paused)
        {
            throw new GameStateException("No paused round to resume.");
        }

        CurrentRound.State = GameRoundState.Playing;
        _logger.LogDebug("Round resumed");
    }
}
