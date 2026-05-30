// <copyright file="StatisticsRepository.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Data.Repositories;

using Baffleword.Core.Repositories;
using Microsoft.Data.Sqlite;

/// <summary>
/// SQLite-based statistics repository.
/// </summary>
public sealed class StatisticsRepository : IStatisticsRepository
{
    private readonly BafflewordDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsRepository"/> class.
    /// </summary>
    /// <param name="database">The database instance.</param>
    public StatisticsRepository(BafflewordDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
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
}
