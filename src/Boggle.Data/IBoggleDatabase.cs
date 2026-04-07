// <copyright file="IBoggleDatabase.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Data;

/// <summary>
/// Provides database lifecycle management.
/// </summary>
public interface IBoggleDatabase : IDisposable
{
    /// <summary>
    /// Ensures the database schema is created and up to date.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task InitializeAsync();
}
