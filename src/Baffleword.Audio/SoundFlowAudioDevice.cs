// <copyright file="SoundFlowAudioDevice.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Audio;

using Microsoft.Extensions.Logging;
using SoundFlow.Abstracts.Devices;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Providers;
using SoundFlow.Structs;

/// <summary>
/// Shared SoundFlow playback device for cross-platform audio.
/// </summary>
public sealed class SoundFlowAudioDevice : IDisposable
{
    private readonly Lock _lock = new();
    private readonly ILogger<SoundFlowAudioDevice> _logger;
    private MiniAudioEngine? _engine;
    private AudioPlaybackDevice? _device;
    private bool _isUnavailable;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundFlowAudioDevice"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public SoundFlowAudioDevice(ILogger<SoundFlowAudioDevice> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets the shared playback format.
    /// </summary>
    public AudioFormat Format { get; } = AudioFormat.DvdHq;

    /// <summary>
    /// Gets a value indicating whether the device is initialized and available.
    /// </summary>
    public bool IsAvailable
    {
        get
        {
            lock (_lock)
            {
                return _device is not null && _engine is not null && !_disposed;
            }
        }
    }

    /// <summary>
    /// Removes a player from the shared mixer.
    /// </summary>
    /// <param name="player">The player to remove.</param>
    public void RemovePlayer(SoundPlayer player)
    {
        ArgumentNullException.ThrowIfNull(player);

        lock (_lock)
        {
            _device?.MasterMixer.RemoveComponent(player);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _device?.Stop();
            _device?.Dispose();
            _engine?.Dispose();
            _device = null;
            _engine = null;
        }
    }

    /// <summary>
    /// Creates a file-backed player and adds it to the shared mixer.
    /// </summary>
    /// <param name="filePath">The audio file path.</param>
    /// <param name="playback">The created playback object.</param>
    /// <returns><see langword="true"/> when the player was created and added.</returns>
    internal bool TryCreatePlayback(string filePath, out SoundFlowPlayback? playback)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        playback = null;
        SoundPlayer? player = null;
        StreamDataProvider? dataProvider = null;
        FileStream? fileStream = null;

        lock (_lock)
        {
            if (!TryEnsureInitialized())
            {
                return false;
            }

            try
            {
                fileStream = File.OpenRead(filePath);
                dataProvider = new StreamDataProvider(_engine!, fileStream, options: null!);
                player = new SoundPlayer(_engine!, Format, dataProvider);
                _device!.MasterMixer.AddComponent(player);
                playback = new SoundFlowPlayback(this, player, dataProvider, fileStream);

                player = null;
                dataProvider = null;
                fileStream = null;
                return true;
            }
            catch (Exception ex) when (ex is not OutOfMemoryException)
            {
                _logger.LogError(ex, "Failed to create audio playback for {FilePath}", filePath);
                CleanupPlayer(player, dataProvider, fileStream);
                return false;
            }
        }
    }

    private void CleanupPlayer(SoundPlayer? player, StreamDataProvider? dataProvider, FileStream? fileStream)
    {
        if (player is not null)
        {
            _device?.MasterMixer.RemoveComponent(player);
        }

        player?.Dispose();
        dataProvider?.Dispose();
        fileStream?.Dispose();
    }

    private bool TryEnsureInitialized()
    {
        if (_device is not null && _engine is not null)
        {
            return true;
        }

        if (_disposed || _isUnavailable)
        {
            return false;
        }

        try
        {
            _engine = new MiniAudioEngine();
            _engine.UpdateAudioDevicesInfo();
            _device = _engine.InitializePlaybackDevice(deviceInfo: null, Format);
            _device.Start();
            _logger.LogInformation("SoundFlow audio device initialized");
            return true;
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            _logger.LogWarning(ex, "SoundFlow audio device is unavailable");
            _device?.Dispose();
            _engine?.Dispose();
            _device = null;
            _engine = null;
            _isUnavailable = true;
            return false;
        }
    }
}
