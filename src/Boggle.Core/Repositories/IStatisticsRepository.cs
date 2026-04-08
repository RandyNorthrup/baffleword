// <copyright file="IStatisticsRepository.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Repositories;

/// <summary>
/// Repository for player statistics data access.
/// </summary>
public interface IStatisticsRepository
{
    /// <summary>
    /// Gets a statistic value by key.
    /// </summary>
    /// <param name="key">The statistic key.</param>
    /// <returns>The value, or <see langword="null"/> if not found.</returns>
    Task<string?> GetAsync(string key);

    /// <summary>
    /// Sets a statistic value.
    /// </summary>
    /// <param name="key">The statistic key.</param>
    /// <param name="value">The value to store.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync(string key, string value);
}
