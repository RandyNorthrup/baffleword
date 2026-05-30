// <copyright file="SoundFlowSoundEffectPlayer.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Audio;

using Microsoft.Extensions.Logging;

/// <summary>
/// Plays sound effects using SoundFlow.
/// </summary>
public sealed class SoundFlowSoundEffectPlayer : ISoundEffectPlayer
{
    private const int PlaybackCleanupDelayMilliseconds = 250;

    private readonly Dictionary<SoundEffect, string> _soundFiles = [];
    private readonly List<SoundFlowPlayback> _activePlaybacks = [];
    private readonly SoundFlowAudioDevice _audioDevice;
    private readonly ILogger<SoundFlowSoundEffectPlayer> _logger;
    private readonly Lock _lock = new();
    private float _volume = 0.8f;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundFlowSoundEffectPlayer"/> class.
    /// </summary>
    /// <param name="audioDevice">The shared SoundFlow audio device.</param>
    /// <param name="logger">The logger instance.</param>
    public SoundFlowSoundEffectPlayer(SoundFlowAudioDevice audioDevice, ILogger<SoundFlowSoundEffectPlayer> logger)
    {
        _audioDevice = audioDevice ?? throw new ArgumentNullException(nameof(audioDevice));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public bool IsMuted { get; set; }

    /// <inheritdoc/>
    public float Volume
    {
        get => _volume;
        set => _volume = Math.Clamp(value, 0f, 1f);
    }

    /// <inheritdoc/>
    public void Preload(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);

        if (!Directory.Exists(directory))
        {
            _logger.LogWarning("Sound effects directory not found: {Directory}", directory);
            return;
        }

        _soundFiles.Clear();
        foreach (SoundEffect effect in Enum.GetValues<SoundEffect>())
        {
            string fileName = ConvertEffectToFileName(effect);
            string filePath = Path.Combine(directory, fileName);

            if (File.Exists(filePath))
            {
                _soundFiles[effect] = filePath;
            }
            else
            {
                _logger.LogWarning("Sound effect file not found: {FilePath}", filePath);
            }
        }

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Loaded {Count} sound effect paths", _soundFiles.Count);
        }
    }

    /// <inheritdoc/>
    public void Play(SoundEffect effect)
    {
        if (_disposed || IsMuted)
        {
            return;
        }

        if (!_soundFiles.TryGetValue(effect, out string? filePath))
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Sound effect not loaded: {Effect}", effect);
            }

            return;
        }

        SoundFlowPlayback? playback = null;
        try
        {
            if (!_audioDevice.TryCreatePlayback(filePath, out playback) || playback is null)
            {
                return;
            }

            SoundFlowPlayback activePlayback = playback;
            EventHandler<EventArgs>? playbackEnded = null;
            playbackEnded = (_, _) =>
            {
                if (playbackEnded is not null)
                {
                    activePlayback.PlaybackEnded -= playbackEnded;
                }

                _ = CleanupPlaybackAfterDelayAsync(activePlayback);
            };

            activePlayback.Volume = Volume;
            activePlayback.PlaybackEnded += playbackEnded;
            lock (_lock)
            {
                if (_disposed)
                {
                    activePlayback.PlaybackEnded -= playbackEnded;
                    return;
                }

                _activePlaybacks.Add(activePlayback);
            }

            activePlayback.Play();
            playback = null;
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            _logger.LogError(ex, "Failed to play sound effect: {Effect}", effect);
            if (playback is not null)
            {
                CleanupPlayback(playback);
                playback = null;
            }
        }
        finally
        {
            playback?.Dispose();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        List<SoundFlowPlayback> activePlaybacks;
        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }

            _soundFiles.Clear();
            activePlaybacks = [.. _activePlaybacks];
            _activePlaybacks.Clear();
            _disposed = true;
        }

        foreach (SoundFlowPlayback playback in activePlaybacks)
        {
            playback.Dispose();
        }
    }

    private static string ConvertEffectToFileName(SoundEffect effect)
    {
        return effect switch
        {
            SoundEffect.TileClick => "tile_click.wav",
            SoundEffect.WordValid => "word_valid.wav",
            SoundEffect.WordInvalid => "word_invalid.wav",
            SoundEffect.TimerTick => "timer_tick.wav",
            SoundEffect.TimerWarning => "timer_warning.wav",
            SoundEffect.RoundStart => "round_start.wav",
            SoundEffect.RoundEnd => "round_end.wav",
            SoundEffect.AchievementUnlock => "achievement_unlock.wav",
            SoundEffect.ButtonHover => "button_hover.wav",
            SoundEffect.ButtonClick => "button_click.wav",
            SoundEffect.Pause => "pause.wav",
            SoundEffect.Resume => "resume.wav",
            _ => throw new ArgumentOutOfRangeException(nameof(effect), effect, "Unknown sound effect"),
        };
    }

    private void CleanupPlayback(SoundFlowPlayback playback)
    {
        lock (_lock)
        {
            _activePlaybacks.Remove(playback);
        }

        playback.Dispose();
    }

    private async Task CleanupPlaybackAfterDelayAsync(SoundFlowPlayback playback)
    {
        try
        {
            await Task.Delay(PlaybackCleanupDelayMilliseconds).ConfigureAwait(false);
            CleanupPlayback(playback);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            _logger.LogDebug(ex, "Failed to clean up sound effect playback");
        }
    }
}
