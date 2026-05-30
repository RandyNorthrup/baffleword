// <copyright file="ISettingsRepository.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Repositories;

/// <summary>
/// Repository for application settings.
/// </summary>
public interface ISettingsRepository
{
    /// <summary>
    /// Gets a setting value by key.
    /// </summary>
    /// <param name="key">The setting key.</param>
    /// <returns>The setting value, or <see langword="null"/> if not found.</returns>
    Task<string?> GetAsync(string key);

    /// <summary>
    /// Sets a setting value.
    /// </summary>
    /// <param name="key">The setting key.</param>
    /// <param name="value">The setting value.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync(string key, string value);
}
