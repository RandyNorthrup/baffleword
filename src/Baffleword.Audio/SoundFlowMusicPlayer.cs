// <copyright file="SoundFlowMusicPlayer.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Audio;

using Microsoft.Extensions.Logging;
using SoundFlow.Enums;

/// <summary>
/// Plays background music using SoundFlow.
/// </summary>
public sealed class SoundFlowMusicPlayer : IMusicPlayer
{
    private readonly SoundFlowAudioDevice _audioDevice;
    private readonly ILogger<SoundFlowMusicPlayer> _logger;
    private readonly Lock _lock = new();
    private SoundFlowPlayback? _playback;
    private string? _currentFilePath;
    private bool _loop;
    private bool _disposed;
    private bool _isMuted;
    private float _volume = 0.5f;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundFlowMusicPlayer"/> class.
    /// </summary>
    /// <param name="audioDevice">The shared SoundFlow audio device.</param>
    /// <param name="logger">The logger instance.</param>
    public SoundFlowMusicPlayer(SoundFlowAudioDevice audioDevice, ILogger<SoundFlowMusicPlayer> logger)
    {
        _audioDevice = audioDevice ?? throw new ArgumentNullException(nameof(audioDevice));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            lock (_lock)
            {
                if (_isMuted == value)
                {
                    return;
                }

                _isMuted = value;
                if (_playback is null)
                {
                    return;
                }

                if (_isMuted)
                {
                    _playback.Pause();
                }
                else
                {
                    _playback.Play();
                }
            }
        }
    }

    /// <inheritdoc/>
    public float Volume
    {
        get => _volume;
        set
        {
            lock (_lock)
            {
                _volume = Math.Clamp(value, 0f, 1f);
                if (_playback is { } playback)
                {
                    playback.Volume = _volume;
                }
            }
        }
    }

    /// <inheritdoc/>
    public bool IsPlaying
    {
        get
        {
            lock (_lock)
            {
                return _playback?.State == PlaybackState.Playing;
            }
        }
    }

    /// <inheritdoc/>
    public void Play(string filePath, bool shouldLoop = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }

            StopPlayback();

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Music file not found: {FilePath}", filePath);
                return;
            }

            SoundFlowPlayback? playback = null;
            try
            {
                if (!_audioDevice.TryCreatePlayback(filePath, out playback) || playback is null)
                {
                    return;
                }

                _playback = playback;
                _currentFilePath = filePath;
                _loop = shouldLoop;
                _playback.Volume = _volume;
                _playback.PlaybackEnded += OnPlaybackEnded;

                if (!_isMuted)
                {
                    _playback.Play();
                }

                playback = null;
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Music started: {FilePath}", filePath);
                }
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                _logger.LogError(ex, "Failed to play music: {FilePath}", filePath);
                if (playback is not null && ReferenceEquals(_playback, playback))
                {
                    StopPlayback();
                    playback = null;
                }
            }
            finally
            {
                playback?.Dispose();
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_lock)
        {
            if (!_disposed)
            {
                StopPlayback();
                _disposed = true;
                _logger.LogDebug("SoundFlowMusicPlayer disposed");
            }
        }
    }

    private void StopPlayback()
    {
        if (_playback is not null)
        {
            _playback.PlaybackEnded -= OnPlaybackEnded;
            _playback.Stop();
            _playback.Dispose();
            _playback = null;
        }

        _currentFilePath = null;
    }

    private void OnPlaybackEnded(object? sender, EventArgs e)
    {
        string? filePath;

        lock (_lock)
        {
            if (!_loop || _disposed || _isMuted || _currentFilePath is null)
            {
                return;
            }

            filePath = _currentFilePath;
            StopPlayback();
        }

        Play(filePath, shouldLoop: true);
    }
}
