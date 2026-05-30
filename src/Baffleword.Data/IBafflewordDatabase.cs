// <copyright file="IBafflewordDatabase.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Data;

/// <summary>
/// Provides database lifecycle management.
/// </summary>
public interface IBafflewordDatabase : IDisposable
{
    /// <summary>
    /// Ensures the database schema is created and up to date.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InitializeAsync();
}
