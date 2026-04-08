// <copyright file="AudioManagerTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Audio.Tests;

using Boggle.Audio;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

/// <summary>
/// Tests for the <see cref="AudioManager"/> class.
/// </summary>
public sealed class AudioManagerTests : IDisposable
{
    private readonly Mock<ISoundEffectPlayer> _sfxMock = new();
    private readonly Mock<IMusicPlayer> _musicMock = new();
    private readonly AudioManager _sut;

    public AudioManagerTests()
    {
        _sut = new AudioManager(_sfxMock.Object, _musicMock.Object, NullLogger<AudioManager>.Instance);
    }

    public void Dispose()
    {
        _sut.Dispose();
    }

    [Fact]
    public void SoundEffects_ReturnsInjectedPlayer()
    {
        _sut.SoundEffects.Should().BeSameAs(_sfxMock.Object);
    }

    [Fact]
    public void Music_ReturnsInjectedPlayer()
    {
        _sut.Music.Should().BeSameAs(_musicMock.Object);
    }

    [Fact]
    public void Initialize_PreloadsAudio()
    {
        _sut.Initialize("sounds", "music");

        _sfxMock.Verify(s => s.Preload("sounds"), Times.Once);
    }

    [Fact]
    public void Dispose_DisposesBothPlayers()
    {
        _sut.Dispose();

        _sfxMock.Verify(s => s.Dispose(), Times.Once);
        _musicMock.Verify(m => m.Dispose(), Times.Once);
    }

    [Fact]
    public void Dispose_CanBeCalledMultipleTimes()
    {
        _sut.Dispose();
        _sut.Dispose();

        _sfxMock.Verify(s => s.Dispose(), Times.Once);
    }
}
