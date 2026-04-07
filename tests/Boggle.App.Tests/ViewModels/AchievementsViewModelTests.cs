// <copyright file="AchievementsViewModelTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.Tests.ViewModels;

using Boggle.App.Navigation;
using Boggle.App.ViewModels;
using Boggle.Core.Models;
using Boggle.Core.Services;
using FluentAssertions;
using Moq;
using Xunit;

public sealed class AchievementsViewModelTests
{
    private readonly Mock<IAchievementService> _achievementService = new();
    private readonly Mock<INavigationService> _navigation = new();

    [Fact]
    public void BackCommand_NavigatesToMainMenu()
    {
        _achievementService.Setup(s => s.GetAllAchievements())
            .Returns(new List<Achievement>());

        var sut = new AchievementsViewModel(_achievementService.Object, _navigation.Object);

        sut.BackCommand.Execute(null);

        _navigation.Verify(n => n.NavigateTo<MainMenuViewModel>(), Times.Once);
    }

    [Fact]
    public void Achievements_LoadedFromService()
    {
        var achievements = new List<Achievement>
        {
            new() { Id = 1, Name = "First Words", Description = "Find your first word", IsUnlocked = true },
            new() { Id = 2, Name = "Word Hoarder", Description = "Find 10 words in one round", IsUnlocked = false },
        };

        _achievementService.Setup(s => s.GetAllAchievements()).Returns(achievements);

        var sut = new AchievementsViewModel(_achievementService.Object, _navigation.Object);

        sut.Achievements.Should().HaveCount(2);
        sut.Achievements[0].Name.Should().Be("First Words");
        sut.Achievements[1].IsUnlocked.Should().BeFalse();
    }

    [Fact]
    public void Achievements_EmptyWhenServiceReturnsEmpty()
    {
        _achievementService.Setup(s => s.GetAllAchievements())
            .Returns(new List<Achievement>());

        var sut = new AchievementsViewModel(_achievementService.Object, _navigation.Object);

        sut.Achievements.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ThrowsOnNullAchievementService()
    {
        FluentActions.Invoking(() => new AchievementsViewModel(null!, _navigation.Object))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_ThrowsOnNullNavigation()
    {
        FluentActions.Invoking(() => new AchievementsViewModel(_achievementService.Object, null!))
            .Should().Throw<ArgumentNullException>();
    }
}
