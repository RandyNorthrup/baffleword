// <copyright file="SoundFlowPlayback.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Audio;

using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;

/// <summary>
/// Owns one SoundFlow player and its file-backed resources.
/// </summary>
internal sealed class SoundFlowPlayback : IDisposable
{
    private readonly SoundFlowAudioDevice _audioDevice;
    private readonly SoundPlayer _player;
    private readonly StreamDataProvider _dataProvider;
    private readonly FileStream _fileStream;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundFlowPlayback"/> class.
    /// </summary>
    /// <param name="audioDevice">The shared audio device.</param>
    /// <param name="player">The SoundFlow player.</param>
    /// <param name="dataProvider">The SoundFlow data provider.</param>
    /// <param name="fileStream">The source file stream.</param>
    public SoundFlowPlayback(
        SoundFlowAudioDevice audioDevice,
        SoundPlayer player,
        StreamDataProvider dataProvider,
        FileStream fileStream)
    {
        _audioDevice = audioDevice ?? throw new ArgumentNullException(nameof(audioDevice));
        _player = player ?? throw new ArgumentNullException(nameof(player));
        _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
        _fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
    }

    /// <summary>
    /// Occurs when playback ends.
    /// </summary>
    public event EventHandler<EventArgs>? PlaybackEnded
    {
        add => _player.PlaybackEnded += value;
        remove => _player.PlaybackEnded -= value;
    }

    /// <summary>
    /// Gets the current playback state.
    /// </summary>
    public PlaybackState State => _player.State;

    /// <summary>
    /// Gets or sets the playback volume.
    /// </summary>
    public float Volume
    {
        get => _player.Volume;
        set => _player.Volume = value;
    }

    /// <summary>
    /// Starts playback.
    /// </summary>
    public void Play()
    {
        _player.Play();
    }

    /// <summary>
    /// Pauses playback.
    /// </summary>
    public void Pause()
    {
        _player.Pause();
    }

    /// <summary>
    /// Stops playback.
    /// </summary>
    public void Stop()
    {
        _player.Stop();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _audioDevice.RemovePlayer(_player);
        _player.Dispose();
        _dataProvider.Dispose();
        _fileStream.Dispose();
    }
}
