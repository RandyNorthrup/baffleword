// <copyright file="HighScoreRepository.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Data.Repositories;

using System.Globalization;
using Boggle.Core.Models;
using Boggle.Core.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

/// <summary>
/// SQLite-based high score repository.
/// </summary>
public sealed class HighScoreRepository : IHighScoreRepository
{
    private readonly BoggleDatabase _database;
    private readonly ILogger<HighScoreRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HighScoreRepository"/> class.
    /// </summary>
    /// <param name="database">The database instance.</param>
    /// <param name="logger">The logger instance.</param>
    public HighScoreRepository(BoggleDatabase database, ILogger<HighScoreRepository> logger)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task AddAsync(HighScoreEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        using SqliteConnection connection = _database.CreateConnection();
        await connection.OpenAsync().ConfigureAwait(false);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO HighScores (Score, WordsFound, LongestWord, CompletionPercentage, TimerDurationSeconds, GameMode, AchievedAt)
            VALUES (@Score, @WordsFound, @LongestWord, @CompletionPercentage, @TimerDurationSeconds, @GameMode, @AchievedAt)
            """;

        command.Parameters.AddWithValue("@Score", entry.Score);
        command.Parameters.AddWithValue("@WordsFound", entry.WordsFound);
        command.Parameters.AddWithValue("@LongestWord", entry.LongestWord);
        command.Parameters.AddWithValue("@CompletionPercentage", entry.CompletionPercentage);
        command.Parameters.AddWithValue("@TimerDurationSeconds", (int)entry.TimerDuration.TotalSeconds);
        command.Parameters.AddWithValue("@GameMode", entry.GameMode.ToString());
        command.Parameters.AddWithValue("@AchievedAt", entry.AchievedAt.ToString("O", CultureInfo.InvariantCulture));

        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        _logger.LogDebug("High score {Score} recorded", entry.Score);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<HighScoreEntry>> GetTopAsync(string gameMode, int count)
    {
        using SqliteConnection connection = _database.CreateConnection();
        await connection.OpenAsync().ConfigureAwait(false);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Score, WordsFound, LongestWord, CompletionPercentage, TimerDurationSeconds, GameMode, AchievedAt
            FROM HighScores
            WHERE GameMode = @GameMode
            ORDER BY Score DESC
            LIMIT @Count
            """;

        command.Parameters.AddWithValue("@GameMode", gameMode);
        command.Parameters.AddWithValue("@Count", count);

        var entries = new List<HighScoreEntry>();
        using SqliteDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            entries.Add(ReadHighScoreEntry(reader));
        }

        return entries;
    }

    /// <inheritdoc/>
    public async Task<int> GetMinimumTopScoreAsync(string gameMode, int topN)
    {
        using SqliteConnection connection = _database.CreateConnection();
        await connection.OpenAsync().ConfigureAwait(false);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT MIN(Score) FROM (
                SELECT Score FROM HighScores
                WHERE GameMode = @GameMode
                ORDER BY Score DESC
                LIMIT @TopN
            )
            """;

        command.Parameters.AddWithValue("@GameMode", gameMode);
        command.Parameters.AddWithValue("@TopN", topN);

        object? result = await command.ExecuteScalarAsync().ConfigureAwait(false);
        return result is long score ? (int)score : 0;
    }

    /// <inheritdoc/>
    public async Task ClearAllAsync()
    {
        using SqliteConnection connection = _database.CreateConnection();
        await connection.OpenAsync().ConfigureAwait(false);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM HighScores";
        await command.ExecuteNonQueryAsync().ConfigureAwait(false);

        _logger.LogInformation("All high scores cleared");
    }

    private static HighScoreEntry ReadHighScoreEntry(SqliteDataReader reader)
    {
        string gameModeStr = reader.GetString(6);
        GameMode gameMode = Enum.TryParse<GameMode>(gameModeStr, out GameMode gm) ? gm : GameMode.Standard;

        return new HighScoreEntry
        {
            Id = reader.GetInt32(0),
            Score = reader.GetInt32(1),
            WordsFound = reader.GetInt32(2),
            LongestWord = reader.GetString(3),
            CompletionPercentage = reader.GetDouble(4),
            TimerDuration = TimeSpan.FromSeconds(reader.GetInt32(5)),
            GameMode = gameMode,
            AchievedAt = DateTime.Parse(reader.GetString(7), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
        };
    }
}
