// <copyright file="BoggleDatabase.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Data;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

/// <summary>
/// SQLite database management for the Boggle application.
/// </summary>
public sealed class BoggleDatabase : IBoggleDatabase
{
    private readonly string _connectionString;
    private readonly ILogger<BoggleDatabase> _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoggleDatabase"/> class.
    /// </summary>
    /// <param name="connectionString">The SQLite connection string.</param>
    /// <param name="logger">The logger instance.</param>
    public BoggleDatabase(string connectionString, ILogger<BoggleDatabase> logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the connection string used by this database.
    /// </summary>
    public string ConnectionString => _connectionString;

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing database");

        using SqliteConnection connection = CreateConnection();
        await connection.OpenAsync().ConfigureAwait(false);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS HighScores (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Score INTEGER NOT NULL,
                WordsFound INTEGER NOT NULL,
                LongestWord TEXT NOT NULL DEFAULT '',
                CompletionPercentage REAL NOT NULL DEFAULT 0,
                TimerDurationSeconds INTEGER NOT NULL,
                AchievedAt TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Achievements (
                Id INTEGER PRIMARY KEY,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL,
                IsUnlocked INTEGER NOT NULL DEFAULT 0,
                UnlockedAt TEXT
            );

            CREATE TABLE IF NOT EXISTS Settings (
                Key TEXT PRIMARY KEY,
                Value TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Statistics (
                Key TEXT PRIMARY KEY,
                Value TEXT NOT NULL
            );

            CREATE INDEX IF NOT EXISTS IX_HighScores_Score
                ON HighScores(TimerDurationSeconds, Score DESC);
            """;

        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
        _logger.LogInformation("Database initialized successfully");
    }

    /// <summary>
    /// Creates a new <see cref="SqliteConnection"/> to the database.
    /// </summary>
    /// <returns>A new connection (not yet opened).</returns>
    public SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _logger.LogDebug("Database disposed");
        }
    }
}
