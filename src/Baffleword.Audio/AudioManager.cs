// <copyright file="AudioManager.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Audio;

using Microsoft.Extensions.Logging;

/// <summary>
/// Top-level audio manager that coordinates sound effects and music.
/// </summary>
public sealed class AudioManager : IAudioManager
{
    private readonly ILogger<AudioManager> _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioManager"/> class.
    /// </summary>
    /// <param name="soundEffects">The sound effect player.</param>
    /// <param name="music">The music player.</param>
    /// <param name="logger">The logger instance.</param>
    public AudioManager(ISoundEffectPlayer soundEffects, IMusicPlayer music, ILogger<AudioManager> logger)
    {
        SoundEffects = soundEffects ?? throw new ArgumentNullException(nameof(soundEffects));
        Music = music ?? throw new ArgumentNullException(nameof(music));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public ISoundEffectPlayer SoundEffects { get; }

    /// <inheritdoc/>
    public IMusicPlayer Music { get; }

    /// <inheritdoc/>
    public void Initialize(string soundsDirectory, string musicDirectory)
    {
        _logger.LogInformation("Initializing audio system");
        SoundEffects.Preload(soundsDirectory);

        string gameLoopPath = Path.Combine(musicDirectory, "game_loop.wav");
        if (File.Exists(gameLoopPath))
        {
            Music.Play(gameLoopPath, shouldLoop: true);
            _logger.LogInformation("Background music started");
        }

        _logger.LogInformation("Audio system initialized");
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            SoundEffects.Dispose();
            Music.Dispose();
            _disposed = true;
            _logger.LogDebug("AudioManager disposed");
        }
    }
}
