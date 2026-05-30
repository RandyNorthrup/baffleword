// <copyright file="IAudioManager.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Audio;

/// <summary>
/// Top-level audio management combining sound effects and music.
/// </summary>
public interface IAudioManager : IDisposable
{
    /// <summary>
    /// Gets the sound effect player.
    /// </summary>
    ISoundEffectPlayer SoundEffects { get; }

    /// <summary>
    /// Gets the music player.
    /// </summary>
    IMusicPlayer Music { get; }

    /// <summary>
    /// Initializes the audio system and preloads assets.
    /// </summary>
    /// <param name="soundsDirectory">The directory containing sound effect WAV files.</param>
    /// <param name="musicDirectory">The directory containing music files.</param>
    void Initialize(string soundsDirectory, string musicDirectory);
}
