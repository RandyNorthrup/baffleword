// <copyright file="MainMenuViewModelTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.Tests.ViewModels;

using Boggle.App.Navigation;
using Boggle.App.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

public sealed class MainMenuViewModelTests
{
    private readonly Mock<INavigationService> _navigation = new();
    private readonly MainMenuViewModel _sut;

    public MainMenuViewModelTests()
    {
        _sut = new MainMenuViewModel(_navigation.Object);
    }

    [Fact]
    public void NewGameCommand_NavigatesToGameViewModel()
    {
        _sut.NewGameCommand.Execute(null);

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
        _sut.NewGameCommand.Should().NotBeNull();
        _sut.HighScoresCommand.Should().NotBeNull();
        _sut.AchievementsCommand.Should().NotBeNull();
        _sut.HowToPlayCommand.Should().NotBeNull();
        _sut.SettingsCommand.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithNullNavigation_ThrowsArgumentNullException()
    {
        Action act = () => new MainMenuViewModel(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
