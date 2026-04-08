// <copyright file="HighScoreService.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Services;

using Boggle.Core.Models;
using Boggle.Core.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Manages high score records.
/// </summary>
public sealed class HighScoreService : IHighScoreService
{
    private const int MaxTopScores = 50;
    private readonly IHighScoreRepository _repository;
    private readonly ILogger<HighScoreService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HighScoreService"/> class.
    /// </summary>
    /// <param name="repository">The high score repository.</param>
    /// <param name="logger">The logger instance.</param>
    public HighScoreService(IHighScoreRepository repository, ILogger<HighScoreService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<HighScoreEntry?> TryRecordScoreAsync(GameRound round)
    {
        ArgumentNullException.ThrowIfNull(round);

        string gameMode = round.Mode.ToString();
        int minimumScore = await _repository.GetMinimumTopScoreAsync(gameMode, MaxTopScores).ConfigureAwait(false);

        if (round.Score <= minimumScore)
        {
            _logger.LogDebug("Score {Score} did not qualify for top {Max}", round.Score, MaxTopScores);
            return null;
        }

        string longestWord = round.SubmittedWords
            .Where(w => w.Status == WordStatus.Valid)
            .OrderByDescending(w => w.Word.Length)
            .Select(w => w.Word)
            .FirstOrDefault() ?? string.Empty;

        var entry = new HighScoreEntry
        {
            Score = round.Score,
            WordsFound = round.ValidWordCount,
            LongestWord = longestWord,
            CompletionPercentage = round.CompletionPercentage,
            TimerDuration = round.TimerDuration,
            GameMode = round.Mode,
            AchievedAt = DateTime.UtcNow,
        };

        await _repository.AddAsync(entry).ConfigureAwait(false);
        _logger.LogInformation("New high score recorded: {Score} points", entry.Score);
        return entry;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<HighScoreEntry>> GetTopScoresAsync(GameMode gameMode, int count = 50)
    {
        return await _repository.GetTopAsync(gameMode.ToString(), count).ConfigureAwait(false);
    }
}
