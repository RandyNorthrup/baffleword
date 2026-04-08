// <copyright file="MainMenuViewModelTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Tests.ViewModels;

using Boggle.App.Navigation;
using Boggle.App.ViewModels;
using Boggle.Core.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public sealed class MainMenuViewModelTests
{
    private readonly Mock<INavigationService> _navigation = new();
    private readonly Mock<ISettingsRepository> _settingsRepo = new();
    private readonly MainMenuViewModel _sut;

    public MainMenuViewModelTests()
    {
        _settingsRepo.Setup(r => r.SetAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        _sut = new MainMenuViewModel(_navigation.Object, _settingsRepo.Object, NullLogger<MainMenuViewModel>.Instance);
    }

    [Fact]
    public void PlayStandardCommand_SetsGameModeAndNavigates()
    {
        _sut.PlayStandardCommand.Execute(null);

        _settingsRepo.Verify(r => r.SetAsync("GameMode", "Standard"), Times.Once);
        _navigation.Verify(n => n.NavigateTo<GameViewModel>(), Times.Once);
    }

    [Fact]
    public void PlayBigBoggleCommand_SetsGameModeAndNavigates()
    {
        _sut.PlayBigBoggleCommand.Execute(null);

        _settingsRepo.Verify(r => r.SetAsync("GameMode", "BigBoggle"), Times.Once);
        _navigation.Verify(n => n.NavigateTo<GameViewModel>(), Times.Once);
    }

    [Fact]
    public void PlaySuperBigBoggleCommand_SetsGameModeAndNavigates()
    {
        _sut.PlaySuperBigBoggleCommand.Execute(null);

        _settingsRepo.Verify(r => r.SetAsync("GameMode", "SuperBigBoggle"), Times.Once);
        _navigation.Verify(n => n.NavigateTo<GameViewModel>(), Times.Once);
    }

    [Fact]
    public void HighScoresCommand_NavigatesToHighScoresViewModel()
    {
        _sut.HighScoresCommand.Execute(null);

        _navigation.Verify(n => n.NavigateTo<HighScoresViewModel>(), Times.Once);
    }

    [Fact]
    public void AchievementsCommand_NavigatesToAchievementsViewModel()
    {
        _sut.AchievementsCommand.Execute(null);

        _navigation.Verify(n => n.NavigateTo<AchievementsViewModel>(), Times.Once);
    }

    [Fact]
    public void HowToPlayCommand_NavigatesToHowToPlayViewModel()
    {
        _sut.HowToPlayCommand.Execute(null);

        _navigation.Verify(n => n.NavigateTo<HowToPlayViewModel>(), Times.Once);
    }

    [Fact]
    public void SettingsCommand_NavigatesToSettingsViewModel()
    {
        _sut.SettingsCommand.Execute(null);

        _navigation.Verify(n => n.NavigateTo<SettingsViewModel>(), Times.Once);
    }

    [Fact]
    public void AllCommands_AreNotNull()
    {
        _sut.PlayStandardCommand.Should().NotBeNull();
        _sut.PlayBigBoggleCommand.Should().NotBeNull();
        _sut.PlaySuperBigBoggleCommand.Should().NotBeNull();
        _sut.HighScoresCommand.Should().NotBeNull();
        _sut.AchievementsCommand.Should().NotBeNull();
        _sut.HowToPlayCommand.Should().NotBeNull();
        _sut.SettingsCommand.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullNavigation_ThrowsArgumentNullException()
    {
        Action act = () => new MainMenuViewModel(null!, _settingsRepo.Object, NullLogger<MainMenuViewModel>.Instance);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullSettingsRepository_ThrowsArgumentNullException()
    {
        Action act = () => new MainMenuViewModel(_navigation.Object, null!, NullLogger<MainMenuViewModel>.Instance);

        act.Should().Throw<ArgumentNullException>();
    }
}
