// <copyright file="BafflewordDatabase.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Data;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

/// <summary>
/// SQLite database management for the Baffleword application.
/// </summary>
public sealed class BafflewordDatabase : IBafflewordDatabase
{
    private readonly string _connectionString;
    private readonly ILogger<BafflewordDatabase> _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BafflewordDatabase"/> class.
    /// </summary>
    /// <param name="connectionString">The SQLite connection string.</param>
    /// <param name="logger">The logger instance.</param>
    public BafflewordDatabase(string connectionString, ILogger<BafflewordDatabase> logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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
                GameMode TEXT NOT NULL DEFAULT 'Standard',
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
                ON HighScores(GameMode, Score DESC);
            """;

        await command.ExecuteNonQueryAsync().ConfigureAwait(false);

        // Migrate existing databases: add GameMode column if it doesn't exist
        using SqliteCommand checkCommand = connection.CreateCommand();
        checkCommand.CommandText = "SELECT COUNT(*) FROM pragma_table_info('HighScores') WHERE name = 'GameMode'";
        long columnExists = (long)(await checkCommand.ExecuteScalarAsync().ConfigureAwait(false))!;

        if (columnExists == 0)
        {
            using SqliteCommand migrateCommand = connection.CreateCommand();
            migrateCommand.CommandText = "ALTER TABLE HighScores ADD COLUMN GameMode TEXT NOT NULL DEFAULT 'Standard'";
            await migrateCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            _logger.LogInformation("Migrated HighScores table: added GameMode column");
        }

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
