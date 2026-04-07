// <copyright file="Achievement.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Represents an achievement definition and its unlock state.
/// </summary>
public sealed class Achievement
{
    /// <summary>
    /// Gets or sets the unique achievement identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the achievement name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the achievement description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether this achievement has been unlocked.
    /// </summary>
    public bool IsUnlocked { get; set; }

    /// <summary>
    /// Gets or sets the date/time when this achievement was unlocked.
    /// </summary>
    public DateTime? UnlockedAt { get; set; }
}
