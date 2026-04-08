// <copyright file="IMusicPlayer.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Audio;

/// <summary>
/// Plays background music with loop, fade, and volume control.
/// </summary>
public interface IMusicPlayer : IDisposable
{
    /// <summary>
    /// Gets or sets a value indicating whether music is muted.
    /// </summary>
    bool IsMuted { get; set; }

    /// <summary>
    /// Gets or sets the music volume (0.0 to 1.0).
    /// </summary>
    float Volume { get; set; }

    /// <summary>
    /// Gets a value indicating whether music is currently playing.
    /// </summary>
    bool IsPlaying { get; }

    /// <summary>
    /// Starts playing the background music track.
    /// </summary>
    /// <param name="filePath">The path to the music file.</param>
    /// <param name="shouldLoop">Whether to loop the track.</param>
    void Play(string filePath, bool shouldLoop = true);

    /// <summary>
    /// Pauses the music.
    /// </summary>
    void PausePlayback();

    /// <summary>
    /// Resumes the music.
    /// </summary>
    void ResumePlayback();

    /// <summary>
    /// Stops the music.
    /// </summary>
    void StopPlayback();
}
