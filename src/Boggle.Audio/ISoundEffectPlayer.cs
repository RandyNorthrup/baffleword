// <copyright file="ISoundEffectPlayer.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Audio;

/// <summary>
/// Plays fire-and-forget sound effects.
/// </summary>
public interface ISoundEffectPlayer : IDisposable
{
    /// <summary>
    /// Gets or sets a value indicating whether sound effects are muted.
    /// </summary>
    bool IsMuted { get; set; }

    /// <summary>
    /// Gets or sets the master volume for sound effects (0.0 to 1.0).
    /// </summary>
    float Volume { get; set; }

    /// <summary>
    /// Plays a sound effect.
    /// </summary>
    /// <param name="effect">The sound effect to play.</param>
    void Play(SoundEffect effect);

    /// <summary>
    /// Preloads all sound effects from the specified directory.
    /// </summary>
    /// <param name="directory">The directory containing WAV files.</param>
    void Preload(string directory);
}
