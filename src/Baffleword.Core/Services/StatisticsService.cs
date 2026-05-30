// <copyright file="StatisticsService.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Services;

using System.Globalization;
using Baffleword.Core.Models;
using Baffleword.Core.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Tracks and retrieves lifetime player statistics.
/// </summary>
public sealed class StatisticsService : IStatisticsService
{
    private readonly IStatisticsRepository _repository;
    private readonly ILogger<StatisticsService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsService"/> class.
    /// </summary>
    /// <param name="repository">The statistics repository.</param>
    /// <param name="logger">The logger instance.</param>
    public StatisticsService(IStatisticsRepository repository, ILogger<StatisticsService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task UpdateStatisticsAsync(GameRound round)
    {
        ArgumentNullException.ThrowIfNull(round);

        GameStatistics stats = await GetStatisticsAsync().ConfigureAwait(false);

        stats.TotalRoundsPlayed++;
        stats.TotalScore += round.Score;
        stats.TotalWordsFound += round.ValidWordCount;

        if (round.Score > stats.HighestRoundScore)
        {
            stats.HighestRoundScore = round.Score;
        }

        if (round.CompletionPercentage > stats.BestCompletionPercentage)
        {
            stats.BestCompletionPercentage = round.CompletionPercentage;
        }

        string longestThisRound = round.SubmittedWords
            .Where(w => w.Status == WordStatus.Valid)
            .OrderByDescending(w => w.Word.Length)
            .Select(w => w.Word)
            .FirstOrDefault() ?? string.Empty;

        if (longestThisRound.Length > stats.LongestWordEver.Length)
        {
            stats.LongestWordEver = longestThisRound;
        }

        stats.TotalPlayTime += round.TimerDuration;

        await SaveStatisticsAsync(stats).ConfigureAwait(false);
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Statistics updated: {Rounds} rounds, {Score} total score", stats.TotalRoundsPlayed, stats.TotalScore);
        }
    }

    /// <inheritdoc/>
    public async Task<GameStatistics> GetStatisticsAsync()
    {
        var stats = new GameStatistics
        {
            TotalRoundsPlayed = await GetIntAsync("TotalRoundsPlayed").ConfigureAwait(false),
            TotalScore = await GetLongAsync("TotalScore").ConfigureAwait(false),
            HighestRoundScore = await GetIntAsync("HighestRoundScore").ConfigureAwait(false),
            TotalWordsFound = await GetIntAsync("TotalWordsFound").ConfigureAwait(false),
            LongestWordEver = await GetStringAsync("LongestWordEver").ConfigureAwait(false),
            BestCompletionPercentage = await GetDoubleAsync("BestCompletionPercentage").ConfigureAwait(false),
            TotalPlayTime = TimeSpan.FromSeconds(await GetDoubleAsync("TotalPlayTimeSeconds").ConfigureAwait(false)),
        };

        return stats;
    }

    private async Task SaveStatisticsAsync(GameStatistics stats)
    {
        await _repository.SetAsync("TotalRoundsPlayed", stats.TotalRoundsPlayed.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
        await _repository.SetAsync("TotalScore", stats.TotalScore.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
        await _repository.SetAsync("HighestRoundScore", stats.HighestRoundScore.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
        await _repository.SetAsync("TotalWordsFound", stats.TotalWordsFound.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
        await _repository.SetAsync("LongestWordEver", stats.LongestWordEver).ConfigureAwait(false);
        await _repository.SetAsync("BestCompletionPercentage", stats.BestCompletionPercentage.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
        await _repository.SetAsync("TotalPlayTimeSeconds", stats.TotalPlayTime.TotalSeconds.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
    }

    private async Task<int> GetIntAsync(string key)
    {
        string? value = await _repository.GetAsync(key).ConfigureAwait(false);
        return value is not null && int.TryParse(value, CultureInfo.InvariantCulture, out int result) ? result : 0;
    }

    private async Task<long> GetLongAsync(string key)
    {
        string? value = await _repository.GetAsync(key).ConfigureAwait(false);
        return value is not null && long.TryParse(value, CultureInfo.InvariantCulture, out long result) ? result : 0L;
    }

    private async Task<double> GetDoubleAsync(string key)
    {
        string? value = await _repository.GetAsync(key).ConfigureAwait(false);
        return value is not null && double.TryParse(value, CultureInfo.InvariantCulture, out double result) ? result : 0.0;
    }

    private async Task<string> GetStringAsync(string key)
    {
        string? value = await _repository.GetAsync(key).ConfigureAwait(false);
        return value ?? string.Empty;
    }
}
