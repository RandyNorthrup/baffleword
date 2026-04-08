// <copyright file="SoundEffectPlayer.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Audio;

using Microsoft.Extensions.Logging;
using NAudio.Wave;

/// <summary>
/// Plays fire-and-forget sound effects using NAudio.
/// </summary>
public sealed class SoundEffectPlayer : ISoundEffectPlayer
{
    private readonly Dictionary<SoundEffect, byte[]> _soundBuffers = [];
    private readonly ILogger<SoundEffectPlayer> _logger;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundEffectPlayer"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public SoundEffectPlayer(ILogger<SoundEffectPlayer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public bool IsMuted { get; set; }

    /// <inheritdoc/>
    public float Volume { get; set; } = 0.8f;

    /// <inheritdoc/>
    public void Preload(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);

        if (!Directory.Exists(directory))
        {
            _logger.LogWarning("Sound effects directory not found: {Directory}", directory);
            return;
        }

        foreach (SoundEffect effect in Enum.GetValues<SoundEffect>())
        {
            string fileName = ConvertEffectToFileName(effect);
            string filePath = Path.Combine(directory, fileName);

            if (File.Exists(filePath))
            {
                _soundBuffers[effect] = File.ReadAllBytes(filePath);
                _logger.LogDebug("Preloaded sound effect: {Effect}", effect);
            }
            else
            {
                _logger.LogWarning("Sound effect file not found: {FilePath}", filePath);
            }
        }

        _logger.LogInformation("Preloaded {Count} sound effects", _soundBuffers.Count);
    }

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Objects are disposed in PlaybackStopped event handler.")]
    public void Play(SoundEffect effect)
    {
        if (_disposed || IsMuted)
        {
            return;
        }

        if (!_soundBuffers.TryGetValue(effect, out byte[]? buffer))
        {
            _logger.LogDebug("Sound effect not loaded: {Effect}", effect);
            return;
        }

        try
        {
            MemoryStream memoryStream = new(buffer);
            WaveFileReader waveReader = new(memoryStream);
            WaveOutEvent waveOut = new() { Volume = Volume };

            try
            {
                waveOut.Init(waveReader);

                waveOut.PlaybackStopped += (_, _) =>
                {
                    waveOut.Dispose();
                    waveReader.Dispose();
                    memoryStream.Dispose();
                };

                waveOut.Play();
            }
            catch
            {
                waveOut.Dispose();
                waveReader.Dispose();
                memoryStream.Dispose();
                throw;
            }
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Failed to play sound effect: {Effect}", effect);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "Failed to read sound data for effect: {Effect}", effect);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (!_disposed)
        {
            _soundBuffers.Clear();
            _disposed = true;
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
            SoundEffect.HighScore => "high_score.wav",
            SoundEffect.Pause => "pause.wav",
            SoundEffect.Resume => "resume.wav",
            SoundEffect.TileShuffle => "tile_shuffle.wav",
            _ => throw new ArgumentOutOfRangeException(nameof(effect), effect, "Unknown sound effect"),
        };
    }
}
