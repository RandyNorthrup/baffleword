// <copyright file="MusicPlayer.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Audio;

using Microsoft.Extensions.Logging;
using NAudio.Wave;

/// <summary>
/// Plays background music with loop support using NAudio.
/// </summary>
public sealed class MusicPlayer : IMusicPlayer
{
    private readonly ILogger<MusicPlayer> _logger;
    private WaveOutEvent? _waveOut;
    private AudioFileReader? _audioReader;
    private bool _loop;
    private string? _currentFilePath;
    private bool _disposed;
    private float _volume = 0.5f;
    private bool _isMuted;

    /// <summary>
    /// Initializes a new instance of the <see cref="MusicPlayer"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public MusicPlayer(ILogger<MusicPlayer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public bool IsMuted
    {
        get => _isMuted;
        set
        {
            if (_isMuted == value)
            {
                return;
            }

            _isMuted = value;
            if (_isMuted)
            {
                PausePlayback();
            }
            else if (_waveOut?.PlaybackState == PlaybackState.Paused)
            {
                ResumePlayback();
            }
            else if (_waveOut?.PlaybackState == PlaybackState.Stopped)
            {
                _waveOut.Play();
            }
        }
    }

    /// <inheritdoc/>
    public float Volume
    {
        get => _volume;
        set
        {
            _volume = Math.Clamp(value, 0f, 1f);
            if (_audioReader is not null)
            {
                _audioReader.Volume = _volume;
            }
        }
    }

    /// <inheritdoc/>
    public bool IsPlaying => _waveOut?.PlaybackState == PlaybackState.Playing;

    /// <inheritdoc/>
    public void Play(string filePath, bool shouldLoop = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

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

        try
        {
            _currentFilePath = filePath;
            _loop = shouldLoop;
            _audioReader = new AudioFileReader(filePath) { Volume = _volume };
            _waveOut = new WaveOutEvent();
            _waveOut.Init(_audioReader);

            _waveOut.PlaybackStopped += OnPlaybackStopped;

            if (!_isMuted)
            {
                _waveOut.Play();
            }

            _logger.LogInformation("Music started: {FilePath}", filePath);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to play music: {FilePath}", filePath);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Failed to read music file: {FilePath}", filePath);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            StopPlayback();
            _disposed = true;
            _logger.LogDebug("MusicPlayer disposed");
        }
    }

    private void PausePlayback()
    {
        if (_waveOut?.PlaybackState == PlaybackState.Playing)
        {
            _waveOut.Pause();
            _logger.LogDebug("Music paused");
        }
    }

    private void ResumePlayback()
    {
        if (_waveOut?.PlaybackState == PlaybackState.Paused)
        {
            _waveOut.Play();
            _logger.LogDebug("Music resumed");
        }
    }

    private void StopPlayback()
    {
        if (_waveOut is not null)
        {
            _waveOut.PlaybackStopped -= OnPlaybackStopped;
            _waveOut.Stop();
            _waveOut.Dispose();
            _waveOut = null;
        }

        _audioReader?.Dispose();
        _audioReader = null;

        _currentFilePath = null;
    }

    private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        if (e.Exception is not null)
        {
            _logger.LogError(e.Exception, "Music playback error");
            return;
        }

        if (_loop && !_disposed && !_isMuted && _currentFilePath is not null && _audioReader is not null)
        {
            _audioReader.Position = 0;
            _waveOut?.Play();
        }
    }
}
