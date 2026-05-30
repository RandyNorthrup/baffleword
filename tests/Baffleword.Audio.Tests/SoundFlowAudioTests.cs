// <copyright file="SoundFlowAudioTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Audio.Tests;

using Baffleword.Audio;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public sealed class SoundFlowAudioTests
{
    [Fact]
    public void AudioDevice_BeforePlayback_IsNotAvailable()
    {
        using var device = new SoundFlowAudioDevice(NullLogger<SoundFlowAudioDevice>.Instance);

        device.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void AudioDevice_Dispose_CanBeCalledMultipleTimes()
    {
        using var device = new SoundFlowAudioDevice(NullLogger<SoundFlowAudioDevice>.Instance);

        device.Dispose();
        device.Dispose();

        device.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void SoundEffectPlayer_PlayWithoutPreload_DoesNotInitializeDevice()
    {
        using var device = new SoundFlowAudioDevice(NullLogger<SoundFlowAudioDevice>.Instance);
        using var sut = new SoundFlowSoundEffectPlayer(device, NullLogger<SoundFlowSoundEffectPlayer>.Instance);

        sut.Play(SoundEffect.TileClick);

        device.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void SoundEffectPlayer_PreloadMissingDirectory_DoesNotThrow()
    {
        using var device = new SoundFlowAudioDevice(NullLogger<SoundFlowAudioDevice>.Instance);
        using var sut = new SoundFlowSoundEffectPlayer(device, NullLogger<SoundFlowSoundEffectPlayer>.Instance);

        Action act = () => sut.Preload(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(-1f, 0f)]
    [InlineData(0.25f, 0.25f)]
    [InlineData(2f, 1f)]
    public void SoundEffectPlayer_Volume_IsClamped(float value, float expected)
    {
        using var device = new SoundFlowAudioDevice(NullLogger<SoundFlowAudioDevice>.Instance);
        using var sut = new SoundFlowSoundEffectPlayer(device, NullLogger<SoundFlowSoundEffectPlayer>.Instance);

        sut.Volume = value;

        sut.Volume.Should().Be(expected);
    }

    [Fact]
    public void MusicPlayer_PlayMissingFile_DoesNotInitializeDevice()
    {
        using var device = new SoundFlowAudioDevice(NullLogger<SoundFlowAudioDevice>.Instance);
        using var sut = new SoundFlowMusicPlayer(device, NullLogger<SoundFlowMusicPlayer>.Instance);

        sut.Play(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".wav"));

        sut.IsPlaying.Should().BeFalse();
        device.IsAvailable.Should().BeFalse();
    }

    [Theory]
    [InlineData(-1f, 0f)]
    [InlineData(0.5f, 0.5f)]
    [InlineData(3f, 1f)]
    public void MusicPlayer_Volume_IsClamped(float value, float expected)
    {
        using var device = new SoundFlowAudioDevice(NullLogger<SoundFlowAudioDevice>.Instance);
        using var sut = new SoundFlowMusicPlayer(device, NullLogger<SoundFlowMusicPlayer>.Instance);

        sut.Volume = value;

        sut.Volume.Should().Be(expected);
    }

    [Fact]
    public void MusicPlayer_Mute_CanToggleBeforePlayback()
    {
        using var device = new SoundFlowAudioDevice(NullLogger<SoundFlowAudioDevice>.Instance);
        using var sut = new SoundFlowMusicPlayer(device, NullLogger<SoundFlowMusicPlayer>.Instance);

        sut.IsMuted = true;
        sut.IsMuted = false;

        sut.IsMuted.Should().BeFalse();
    }
}
