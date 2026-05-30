// <copyright file="HighScoresViewModelTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.Tests.ViewModels;

using Baffleword.App.Navigation;
using Baffleword.App.ViewModels;
using Baffleword.Core.Models;
using Baffleword.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public sealed class HighScoresViewModelTests
{
    private readonly Mock<IHighScoreService> _highScoreService = new();
    private readonly Mock<INavigationService> _navigation = new();

    public HighScoresViewModelTests()
    {
        _highScoreService.Setup(s => s.GetTopScoresAsync(It.IsAny<GameMode>(), It.IsAny<int>()))
            .ReturnsAsync(new List<HighScoreEntry>());
    }

    [Fact]
    public void BackCommand_NavigatesToMainMenu()
    {
        var sut = new HighScoresViewModel(_highScoreService.Object, _navigation.Object, NullLogger<HighScoresViewModel>.Instance);

        sut.BackCommand.Execute(null);

        _navigation.Verify(n => n.NavigateTo<MainMenuViewModel>(), Times.Once);
    }

    [Fact]
    public void Scores_InitiallyEmpty()
    {
        var sut = new HighScoresViewModel(_highScoreService.Object, _navigation.Object, NullLogger<HighScoresViewModel>.Instance);

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

        _highScoreService.Setup(s => s.GetTopScoresAsync(It.IsAny<GameMode>(), It.IsAny<int>()))
            .ReturnsAsync(entries);

        var sut = new HighScoresViewModel(_highScoreService.Object, _navigation.Object, NullLogger<HighScoresViewModel>.Instance);

        // Allow the async load to complete
        await Task.Delay(100, TestContext.Current.CancellationToken);

        sut.Scores.Should().HaveCount(2);
    }

    [Fact]
    public void SelectedMode_DefaultsToStandard()
    {
        var sut = new HighScoresViewModel(_highScoreService.Object, _navigation.Object, NullLogger<HighScoresViewModel>.Instance);

        sut.SelectedMode.Should().Be(GameMode.Standard);
        sut.IsStandardSelected.Should().BeTrue();
        sut.IsBigBoardSelected.Should().BeFalse();
        sut.IsSuperBoardSelected.Should().BeFalse();
    }

    [Fact]
    public void ShowBigBoardCommand_ChangesSelectedMode()
    {
        var sut = new HighScoresViewModel(_highScoreService.Object, _navigation.Object, NullLogger<HighScoresViewModel>.Instance);

        sut.ShowBigBoardCommand.Execute(null);

        sut.SelectedMode.Should().Be(GameMode.BigBoard);
        sut.IsBigBoardSelected.Should().BeTrue();
    }

    [Fact]
    public void Constructor_ThrowsOnNullHighScoreService()
    {
        FluentActions.Invoking(() => new HighScoresViewModel(null!, _navigation.Object, NullLogger<HighScoresViewModel>.Instance))
            .Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_ThrowsOnNullNavigation()
    {
        FluentActions.Invoking(() => new HighScoresViewModel(_highScoreService.Object, null!, NullLogger<HighScoresViewModel>.Instance))
            .Should().Throw<ArgumentNullException>();
    }
}
