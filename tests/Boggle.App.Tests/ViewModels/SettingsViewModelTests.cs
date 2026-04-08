// <copyright file="SettingsViewModelTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Tests.ViewModels;

using Boggle.App.Navigation;
using Boggle.App.ViewModels;
using Boggle.Audio;
using Boggle.Core.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public sealed class SettingsViewModelTests
{
    private readonly Mock<ISettingsRepository> _settingsRepo = new();
    private readonly Mock<INavigationService> _navigation = new();
    private readonly Mock<IAudioManager> _audioManager = new();
    private readonly Mock<ISoundEffectPlayer> _sfxPlayer = new();
    private readonly Mock<IMusicPlayer> _musicPlayer = new();

    public SettingsViewModelTests()
    {
        _audioManager.Setup(a => a.SoundEffects).Returns(_sfxPlayer.Object);
        _audioManager.Setup(a => a.Music).Returns(_musicPlayer.Object);
        _settingsRepo.Setup(r => r.GetAsync(It.IsAny<string>())).ReturnsAsync((string?)null);
    }

    [Fact]
    public void Constructor_ThrowsOnNullSettingsRepository()
    {
        var act = () => new SettingsViewModel(null!, _navigation.Object, _audioManager.Object, NullLogger<SettingsViewModel>.Instance);
        act.Should().Throw<ArgumentNullException>().WithParameterName("settingsRepository");
    }

    [Fact]
    public void Constructor_ThrowsOnNullNavigation()
    {
        var act = () => new SettingsViewModel(_settingsRepo.Object, null!, _audioManager.Object, NullLogger<SettingsViewModel>.Instance);
        act.Should().Throw<ArgumentNullException>().WithParameterName("navigation");
    }

    [Fact]
    public void Constructor_ThrowsOnNullAudioManager()
    {
        var act = () => new SettingsViewModel(_settingsRepo.Object, _navigation.Object, null!, NullLogger<SettingsViewModel>.Instance);
        act.Should().Throw<ArgumentNullException>().WithParameterName("audioManager");
    }

    [Fact]
    public void DefaultValues_ReadFromAudioManager()
    {
        _sfxPlayer.Setup(p => p.Volume).Returns(0.6f);
        _musicPlayer.Setup(p => p.Volume).Returns(0.4f);

        using var sut = CreateSut();

        sut.SfxVolume.Should().BeApproximately(0.6, 0.01);
        sut.MusicVolume.Should().BeApproximately(0.4, 0.01);
    }

    [Fact]
    public void SfxVolume_ClampsToRange()
    {
        using var sut = CreateSut();
        sut.SfxVolume = -0.5;
        sut.SfxVolume.Should().Be(0.0);

        sut.SfxVolume = 2.0;
        sut.SfxVolume.Should().Be(1.0);
    }

    [Fact]
    public void MusicVolume_ClampsToRange()
    {
        using var sut = CreateSut();
        sut.MusicVolume = -0.5;
        sut.MusicVolume.Should().Be(0.0);

        sut.MusicVolume = 2.0;
        sut.MusicVolume.Should().Be(1.0);
    }

    [Fact]
    public void SfxVolume_UpdatesAudioManagerLive()
    {
        using var sut = CreateSut();
        sut.SfxVolume = 0.3;
        _sfxPlayer.VerifySet(p => p.Volume = 0.3f, Times.Once());
    }

    [Fact]
    public void MusicVolume_UpdatesAudioManagerLive()
    {
        using var sut = CreateSut();
        sut.MusicVolume = 0.7;
        _musicPlayer.VerifySet(p => p.Volume = 0.7f, Times.Once());
    }

    [Fact]
    public void BackCommand_UsesGoBackFirst()
    {
        _navigation.Setup(n => n.GoBack()).Returns(true);
        using var sut = CreateSut();
        sut.BackCommand.Execute(null);
        _navigation.Verify(n => n.GoBack(), Times.Once);
        _navigation.Verify(n => n.NavigateTo<MainMenuViewModel>(), Times.Never);
    }

    [Fact]
    public void BackCommand_FallsBackToMainMenu()
    {
        _navigation.Setup(n => n.GoBack()).Returns(false);
        using var sut = CreateSut();
        sut.BackCommand.Execute(null);
        _navigation.Verify(n => n.NavigateTo<MainMenuViewModel>(), Times.Once);
    }

    [Fact]
    public void SfxVolume_AutoSavesToRepository()
    {
        _settingsRepo.Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        using var sut = CreateSut();
        sut.SfxVolume = 0.6;

        _settingsRepo.Verify(r => r.SetAsync("SfxVolume", "0.6"), Times.Once);
    }

    [Fact]
    public void MusicVolume_AutoSavesToRepository()
    {
        _settingsRepo.Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        using var sut = CreateSut();
        sut.MusicVolume = 0.4;

        _settingsRepo.Verify(r => r.SetAsync("MusicVolume", "0.4"), Times.Once);
    }

    [Fact]
    public async Task LoadSettings_PopulatesFromRepository()
    {
        _settingsRepo.Setup(r => r.GetAsync("SfxVolume")).ReturnsAsync("0.3");
        _settingsRepo.Setup(r => r.GetAsync("MusicVolume")).ReturnsAsync("0.9");

        using var sut = CreateSut();

        // Allow the constructor's async load to complete
        await Task.Delay(200);

        sut.SfxVolume.Should().Be(0.3);
        sut.MusicVolume.Should().Be(0.9);
    }

    [Fact]
    public async Task LoadSettings_DoesNotChangeAudioForVolume()
    {
        _sfxPlayer.Setup(p => p.Volume).Returns(0.8f);
        _musicPlayer.Setup(p => p.Volume).Returns(0.5f);
        _settingsRepo.Setup(r => r.GetAsync("SfxVolume")).ReturnsAsync("0.3");
        _settingsRepo.Setup(r => r.GetAsync("MusicVolume")).ReturnsAsync("0.9");

        _sfxPlayer.Invocations.Clear();
        _musicPlayer.Invocations.Clear();

        using var sut = CreateSut();
        await Task.Delay(200);

        _sfxPlayer.VerifySet(p => p.Volume = It.IsAny<float>(), Times.Never());
        _musicPlayer.VerifySet(p => p.Volume = It.IsAny<float>(), Times.Never());
    }

    [Fact]
    public void Constructor_ReadsIsMutedFromAudioPlayers()
    {
        _sfxPlayer.Setup(p => p.IsMuted).Returns(true);
        _musicPlayer.Setup(p => p.IsMuted).Returns(true);

        using var sut = CreateSut();

        sut.IsSfxMuted.Should().BeTrue();
        sut.IsMusicMuted.Should().BeTrue();
    }

    [Fact]
    public void IsSfxMuted_SetsAudioPlayerDirectly()
    {
        _sfxPlayer.SetupProperty(p => p.IsMuted);
        using var sut = CreateSut();

        sut.IsSfxMuted = true;
        _sfxPlayer.VerifySet(p => p.IsMuted = true);

        sut.IsSfxMuted = false;
        _sfxPlayer.VerifySet(p => p.IsMuted = false);
    }

    [Fact]
    public void IsMusicMuted_SetsAudioPlayerDirectly()
    {
        _musicPlayer.SetupProperty(p => p.IsMuted);
        using var sut = CreateSut();

        sut.IsMusicMuted = true;
        _musicPlayer.VerifySet(p => p.IsMuted = true);

        sut.IsMusicMuted = false;
        _musicPlayer.VerifySet(p => p.IsMuted = false);
    }

    [Fact]
    public void SfxVolume_ReturnsZeroWhenMuted()
    {
        _sfxPlayer.Setup(p => p.Volume).Returns(0.7f);
        _sfxPlayer.Setup(p => p.IsMuted).Returns(true);

        using var sut = CreateSut();

        sut.SfxVolume.Should().Be(0.0);
    }

    [Fact]
    public void MusicVolume_ReturnsZeroWhenMuted()
    {
        _musicPlayer.Setup(p => p.Volume).Returns(0.5f);
        _musicPlayer.Setup(p => p.IsMuted).Returns(true);

        using var sut = CreateSut();

        sut.MusicVolume.Should().Be(0.0);
    }

    [Fact]
    public void SfxVolume_IgnoresSetWhenMuted()
    {
        _sfxPlayer.Setup(p => p.IsMuted).Returns(true);
        using var sut = CreateSut();
        _sfxPlayer.Invocations.Clear();

        sut.SfxVolume = 0.9;
        _sfxPlayer.VerifySet(p => p.Volume = It.IsAny<float>(), Times.Never());
    }

    private SettingsViewModel CreateSut() =>
        new(_settingsRepo.Object, _navigation.Object, _audioManager.Object, NullLogger<SettingsViewModel>.Instance);
}
