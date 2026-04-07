// <copyright file="HighScoresViewModelTests.cs" company="Boggle">
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

public sealed class HighScoresViewModelTests
{
    private readonly Mock<IHighScoreService> _highScoreService = new();
    private readonly Mock<INavigationService> _navigation = new();

    [Fact]
    public void BackCommand_NavigatesToMainMenu()
    {
        _highScoreService.Setup(s => s.GetTopScoresAsync(It.IsAny<TimeSpan>(), It.IsAny<int>()))
            .ReturnsAsync(new List<HighScoreEntry>());

        var sut = new HighScoresViewModel(_highScoreService.Object, _navigation.Object);

        sut.BackCommand.Execute(null);

        _navigation.Verify(n => n.NavigateTo<MainMenuViewModel>(), Times.Once);
    }

    [Fact]
    public void Scores_InitiallyEmpty()
    {
        _highScoreService.Setup(s => s.GetTopScoresAsync(It.IsAny<TimeSpan>(), It.IsAny<int>()))
            .ReturnsAsync(new List<HighScoreEntry>());

        var sut = new HighScoresViewModel(_highScoreService.Object, _navigation.Object);

        sut.Scores.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadScores_PopulatesScoresCollection()
    {
        var entries = new List<HighScoreEntry>
        {
            new() { Score = 100, WordsFound = 10, LongestWord = "test", AchievedAt = DateTime.Now },
            new() { Score = 80, WordsFound = 8, LongestWord = "word", AchievedAt = DateTime.Now },
        };

        _highScoreService.Setup(s => s.GetTopScoresAsync(It.IsAny<TimeSpan>(), It.IsAny<int>()))
            .ReturnsAsync(entries);

        var sut = new HighScoresViewModel(_highScoreService.Object, _navigation.Object);

        // Allow the async load to complete
        await Task.Delay(100);

        sut.Scores.Should().HaveCount(2);
    }

    [Fact]
    public void Constructor_ThrowsOnNullHighScoreService()
    {
        FluentActions.Invoking(() => new HighScoresViewModel(null!, _navigation.Object))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_ThrowsOnNullNavigation()
    {
        FluentActions.Invoking(() => new HighScoresViewModel(_highScoreService.Object, null!))
            .Should().Throw<ArgumentNullException>();
    }
}
