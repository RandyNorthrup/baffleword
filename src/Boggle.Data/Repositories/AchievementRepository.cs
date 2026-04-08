// <copyright file="AchievementRepository.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Data.Repositories;

using System.Globalization;
using Boggle.Core.Models;
using Boggle.Core.Repositories;
using Microsoft.Data.Sqlite;

/// <summary>
/// SQLite-based achievement repository.
/// </summary>
public sealed class AchievementRepository : IAchievementRepository
{
    private readonly BoggleDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="AchievementRepository"/> class.
    /// </summary>
    /// <param name="database">The database instance.</param>
    public AchievementRepository(BoggleDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Achievement>> GetAllAsync()
    {
        using SqliteConnection connection = _database.CreateConnection();
        await connection.OpenAsync().ConfigureAwait(false);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, Description, IsUnlocked, UnlockedAt FROM Achievements ORDER BY Id";

        var achievements = new List<Achievement>();
        using SqliteDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            bool isUnlockedAtNull = await reader.IsDBNullAsync(4).ConfigureAwait(false);
            achievements.Add(new Achievement
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                IsUnlocked = reader.GetInt32(3) != 0,
                UnlockedAt = isUnlockedAtNull
                    ? null
                    : DateTime.Parse(reader.GetString(4), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
            });
        }

        return achievements;
    }

    /// <inheritdoc/>
    public async Task SaveAsync(Achievement achievement)
    {
        ArgumentNullException.ThrowIfNull(achievement);

        using SqliteConnection connection = _database.CreateConnection();
        await connection.OpenAsync().ConfigureAwait(false);

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = """
            INSERT OR REPLACE INTO Achievements (Id, Name, Description, IsUnlocked, UnlockedAt)
            VALUES (@Id, @Name, @Description, @IsUnlocked, @UnlockedAt)
            """;

        command.Parameters.AddWithValue("@Id", achievement.Id);
        command.Parameters.AddWithValue("@Name", achievement.Name);
        command.Parameters.AddWithValue("@Description", achievement.Description);
        command.Parameters.AddWithValue("@IsUnlocked", achievement.IsUnlocked ? 1 : 0);
        object unlockedAtValue = achievement.UnlockedAt.HasValue
            ? achievement.UnlockedAt.Value.ToString("O", CultureInfo.InvariantCulture)
            : (object)DBNull.Value;
        command.Parameters.AddWithValue("@UnlockedAt", unlockedAtValue);

        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }
}
