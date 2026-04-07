// <copyright file="StatisticsRepository.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Data.Repositories;

using Boggle.Core.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

/// <summary>
/// SQLite-based statistics repository.
/// </summary>
public sealed class StatisticsRepository : IStatisticsRepository
{
    private readonly BoggleDatabase _database;
    private readonly ILogger<StatisticsRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsRepository"/> class.
    /// </summary>
    /// <param name="database">The database instance.</param>
    /// <param name="logger">The logger instance.</param>
    public StatisticsRepository(BoggleDatabase database, ILogger<StatisticsRepository> logger)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<string?> GetAsync(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        using SqliteConnection connection = _database.CreateConnection();
        await connection.OpenAsync().ConfigureAwait(false);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT Value FROM Statistics WHERE Key = @Key";
        command.Parameters.AddWithValue("@Key", key);

        object? result = await command.ExecuteScalarAsync().ConfigureAwait(false);
        return result as string;
    }

    /// <inheritdoc/>
    public async Task SetAsync(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        using SqliteConnection connection = _database.CreateConnection();
        await connection.OpenAsync().ConfigureAwait(false);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "INSERT OR REPLACE INTO Statistics (Key, Value) VALUES (@Key, @Value)";
        command.Parameters.AddWithValue("@Key", key);
        command.Parameters.AddWithValue("@Value", value);

        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ClearAllAsync()
    {
        using SqliteConnection connection = _database.CreateConnection();
        await connection.OpenAsync().ConfigureAwait(false);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Statistics";
        await command.ExecuteNonQueryAsync().ConfigureAwait(false);

        _logger.LogInformation("All statistics cleared");
    }
}
