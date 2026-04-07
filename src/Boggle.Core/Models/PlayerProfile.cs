// <copyright file="PlayerProfile.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Represents player settings and preferences.
/// </summary>
public sealed class PlayerProfile
{
    /// <summary>
    /// Gets or sets the sound effects volume (0-100).
    /// </summary>
    public int SoundEffectsVolume { get; set; } = 80;

    /// <summary>
    /// Gets or sets the music volume (0-100).
    /// </summary>
    public int MusicVolume { get; set; } = 50;

    /// <summary>
    /// Gets or sets the timer duration in minutes.
    /// </summary>
    public int TimerDurationMinutes { get; set; } = 3;

    /// <summary>
    /// Gets or sets the minimum word length.
    /// </summary>
    public int MinimumWordLength { get; set; } = 3;
}
