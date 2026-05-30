// <copyright file="IAchievementRepository.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Repositories;

using Baffleword.Core.Models;

/// <summary>
/// Repository for achievement data access.
/// </summary>
public interface IAchievementRepository
{
    /// <summary>
    /// Gets all achievements.
    /// </summary>
    /// <returns>All achievement records.</returns>
    Task<IReadOnlyList<Achievement>> GetAllAsync();

    /// <summary>
    /// Saves or updates an achievement record.
    /// </summary>
    /// <param name="achievement">The achievement to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(Achievement achievement);
}
