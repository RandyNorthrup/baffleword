// <copyright file="IAchievementRepository.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Repositories;

using Boggle.Core.Models;

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

    /// <summary>
    /// Deletes all achievement records.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ClearAllAsync();
}
