// <copyright file="SettingsViewModelTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.Tests.ViewModels;

using Boggle.App.Navigation;
using Boggle.App.ViewModels;
using Boggle.Audio;
using Boggle.Core.Repositories;
using FluentAssertions;
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
        var act = () => new SettingsViewModel(null!, _navigation.Object, _audioManager.Object);
        act.Should().Throw<ArgumentNullException>().WithParameterName("settingsRepository");
    }

    [Fact]
    public void Constructor_ThrowsOnNullNavigation()
    {
        var act = () => new SettingsViewModel(_settingsRepo.Object, null!, _audioManager.Object);
        act.Should().Throw<ArgumentNullException>().WithParameterName("navigation");
    }

    [Fact]
    public void Constructor_ThrowsOnNullAudioManager()
    {
        var act = () => new SettingsViewModel(_settingsRepo.Object, _navigation.Object, null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("audioManager");
    }

    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var sut = CreateSut();

        sut.TimerDurationSeconds.Should().Be(180);
        sut.IsTimer3Min.Should().BeTrue();
        sut.SfxVolume.Should().Be(0.8);
        sut.MusicVolume.Should().Be(0.5);
    }

    [Fact]
    public void TimerPresets_SetCorrectDuration()
    {
        var sut = CreateSut();

        sut.IsTimer1Min = true;
        sut.TimerDurationSeconds.Should().Be(60);
        sut.IsTimer1Min.Should().BeTrue();
        sut.IsTimer3Min.Should().BeFalse();
        sut.IsTimer5Min.Should().BeFalse();

        sut.IsTimer5Min = true;
        sut.TimerDurationSeconds.Should().Be(300);
        sut.IsTimer1Min.Should().BeFalse();
        sut.IsTimer3Min.Should().BeFalse();
        sut.IsTimer5Min.Should().BeTrue();

        sut.IsTimer3Min = true;
        sut.TimerDurationSeconds.Should().Be(180);
    }

    [Fact]
    public void SfxVolume_ClampsToRange()
    {
        var sut = CreateSut();
        sut.SfxVolume = -0.5;
        sut.SfxVolume.Should().Be(0.0);

        sut.SfxVolume = 2.0;
        sut.SfxVolume.Should().Be(1.0);
    }

    [Fact]
    public void MusicVolume_ClampsToRange()
    {
        var sut = CreateSut();
        sut.MusicVolume = -0.5;
        sut.MusicVolume.Should().Be(0.0);

        sut.MusicVolume = 2.0;
        sut.MusicVolume.Should().Be(1.0);
    }

    [Fact]
    public void SfxVolume_UpdatesAudioManagerLive()
    {
        var sut = CreateSut();
        sut.SfxVolume = 0.3;
        _sfxPlayer.VerifySet(p => p.Volume = 0.3f, Times.Once());
    }

    [Fact]
    public void MusicVolume_UpdatesAudioManagerLive()
    {
        var sut = CreateSut();
        sut.MusicVolume = 0.7;
        _musicPlayer.VerifySet(p => p.Volume = 0.7f, Times.Once());
    }

    [Fact]
    public void BackCommand_NavigatesToMainMenu()
    {
        var sut = CreateSut();
        sut.BackCommand.Execute(null);
        _navigation.Verify(n => n.NavigateTo<MainMenuViewModel>(), Times.Once);
    }

    [Fact]
    public void SaveCommand_PersistsSettingsAndNavigates()
    {
        _settingsRepo.Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        var sut = CreateSut();
        sut.TimerDurationSeconds = 120;
        sut.SfxVolume = 0.6;
        sut.MusicVolume = 0.4;

        sut.SaveCommand.Execute(null);

        _settingsRepo.Verify(r => r.SetAsync("TimerDurationSeconds", "120"), Times.Once);
        _settingsRepo.Verify(r => r.SetAsync("SfxVolume", "0.6"), Times.Once);
        _settingsRepo.Verify(r => r.SetAsync("MusicVolume", "0.4"), Times.Once);
        _navigation.Verify(n => n.NavigateTo<MainMenuViewModel>(), Times.Once);
    }

    [Fact]
    public async Task LoadSettings_PopulatesFromRepository()
    {
        _settingsRepo.Setup(r => r.GetAsync("TimerDurationSeconds")).ReturnsAsync("240");
        _settingsRepo.Setup(r => r.GetAsync("SfxVolume")).ReturnsAsync("0.3");
        _settingsRepo.Setup(r => r.GetAsync("MusicVolume")).ReturnsAsync("0.9");

        var sut = CreateSut();

        // Allow the constructor's async load to complete
        await Task.Delay(200);

        sut.TimerDurationSeconds.Should().Be(240);
        sut.SfxVolume.Should().Be(0.3);
        sut.MusicVolume.Should().Be(0.9);
    }

    private SettingsViewModel CreateSut() =>
        new(_settingsRepo.Object, _navigation.Object, _audioManager.Object);
}
