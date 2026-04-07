// <copyright file="GameViewModelTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.Tests.ViewModels;

using Boggle.App.Navigation;
using Boggle.App.ViewModels;
using Boggle.Audio;
using Boggle.Core.Models;
using Boggle.Core.Repositories;
using Boggle.Core.Services;
using FluentAssertions;
using Moq;
using Xunit;

public sealed class GameViewModelTests : IDisposable
{
    private readonly Mock<IGameEngine> _gameEngine = new();
    private readonly Mock<INavigationService> _navigation = new();
    private readonly Mock<IAchievementService> _achievementService = new();
    private readonly Mock<IHighScoreService> _highScoreService = new();
    private readonly Mock<IStatisticsService> _statisticsService = new();
    private readonly Mock<IAudioManager> _audioManager = new();
    private readonly Mock<ISettingsRepository> _settingsRepository = new();
    private readonly GameViewModel _sut;

    public GameViewModelTests()
    {
        _audioManager.Setup(a => a.SoundEffects).Returns(new Mock<ISoundEffectPlayer>().Object);
        _gameEngine.Setup(g => g.StartRound(It.IsAny<TimeSpan>(), It.IsAny<int>()))
            .Returns(new GameRound(CreateTestBoard(), TimeSpan.FromMinutes(3), 3));
        _statisticsService.Setup(s => s.GetStatisticsAsync())
            .ReturnsAsync(new GameStatistics());

        _sut = new GameViewModel(
            _gameEngine.Object,
            _navigation.Object,
            _achievementService.Object,
            _highScoreService.Object,
            _statisticsService.Object,
            _audioManager.Object,
            _settingsRepository.Object);
    }

    [Fact]
    public void Constructor_StartsRound()
    {
        _gameEngine.Verify(g => g.StartRound(TimeSpan.FromMinutes(3), 3), Times.Once);
    }

    [Fact]
    public void Constructor_InitializesBoardLetters()
    {
        _sut.BoardLetters.Should().HaveCount(4);
        _sut.BoardLetters[0].Should().HaveCount(4);
    }

    [Fact]
    public void Constructor_SetsTimerDuration()
    {
        _sut.TimerDuration.Should().Be(TimeSpan.FromMinutes(3));
    }

    [Fact]
    public void Constructor_SetsTimeRemaining()
    {
        _sut.TimeRemaining.Should().Be(TimeSpan.FromMinutes(3));
    }

    [Fact]
    public void Constructor_WithCustomSettings_UsesSettingValues()
    {
        var settingsRepo = new Mock<ISettingsRepository>();
        settingsRepo.Setup(s => s.GetAsync("TimerDurationSeconds")).ReturnsAsync("120");
        settingsRepo.Setup(s => s.GetAsync("MinWordLength")).ReturnsAsync("4");

        var gameEngine = new Mock<IGameEngine>();
        gameEngine.Setup(g => g.StartRound(It.IsAny<TimeSpan>(), It.IsAny<int>()))
            .Returns(new GameRound(CreateTestBoard(), TimeSpan.FromSeconds(120), 4));

        var audioManager = new Mock<IAudioManager>();
        audioManager.Setup(a => a.SoundEffects).Returns(new Mock<ISoundEffectPlayer>().Object);

        using var vm = new GameViewModel(
            gameEngine.Object,
            _navigation.Object,
            _achievementService.Object,
            _highScoreService.Object,
            _statisticsService.Object,
            audioManager.Object,
            settingsRepo.Object);

        settingsRepo.Verify(s => s.GetAsync("TimerDurationSeconds"), Times.Once);
        settingsRepo.Verify(s => s.GetAsync("MinWordLength"), Times.Once);
    }

    [Fact]
    public void SubmitWord_UpdatesFoundWords()
    {
        _gameEngine.Setup(g => g.SubmitWord("TEST"))
            .Returns(new WordResult("TEST", WordStatus.Valid, 1));

        _sut.CurrentWord = "TEST";
        _sut.SubmitWordCommand.Execute(null);

        _sut.FoundWords.Should().ContainSingle();
        _sut.FoundWords[0].Word.Should().Be("TEST");
    }

    [Fact]
    public void SubmitWord_ClearsCurrentWord()
    {
        _gameEngine.Setup(g => g.SubmitWord("TEST"))
            .Returns(new WordResult("TEST", WordStatus.Valid, 1));

        _sut.CurrentWord = "TEST";
        _sut.SubmitWordCommand.Execute(null);

        _sut.CurrentWord.Should().BeEmpty();
    }

    [Fact]
    public void SubmitWord_ValidWord_SetsFeedback()
    {
        _gameEngine.Setup(g => g.SubmitWord("TEST"))
            .Returns(new WordResult("TEST", WordStatus.Valid, 3));

        _sut.CurrentWord = "TEST";
        _sut.SubmitWordCommand.Execute(null);

        _sut.LastSubmissionFeedback.Should().Contain("+3");
    }

    [Fact]
    public void SubmitWord_NotInDict_SetsFeedback()
    {
        _gameEngine.Setup(g => g.SubmitWord("XYZ"))
            .Returns(new WordResult("XYZ", WordStatus.NotInDictionary, 0));

        _sut.CurrentWord = "XYZ";
        _sut.SubmitWordCommand.Execute(null);

        _sut.LastSubmissionFeedback.Should().Be("Not a word!");
    }

    [Fact]
    public void Pause_SetsIsPaused()
    {
        _sut.PauseCommand.Execute(null);

        _sut.IsPaused.Should().BeTrue();
        _gameEngine.Verify(g => g.PauseRound(), Times.Once);
    }

    [Fact]
    public void Resume_ClearsIsPaused()
    {
        _sut.PauseCommand.Execute(null);
        _sut.ResumeCommand.Execute(null);

        _sut.IsPaused.Should().BeFalse();
        _gameEngine.Verify(g => g.ResumeRound(), Times.Once);
    }

    [Fact]
    public void Quit_NavigatesToMainMenu()
    {
        _gameEngine.Setup(g => g.CurrentRound).Returns(new GameRound(CreateTestBoard(), TimeSpan.FromMinutes(3), 3));
        _gameEngine.Setup(g => g.EndRound()).Returns(new GameRound(CreateTestBoard(), TimeSpan.FromMinutes(3), 3));

        _sut.QuitCommand.Execute(null);

        _navigation.Verify(n => n.NavigateTo<MainMenuViewModel>(), Times.Once);
    }

    [Fact]
    public void Score_InitiallyZero()
    {
        _sut.Score.Should().Be(0);
    }

    [Fact]
    public void IsPaused_InitiallyFalse()
    {
        _sut.IsPaused.Should().BeFalse();
    }

    [Fact]
    public void FoundWords_InitiallyEmpty()
    {
        _sut.FoundWords.Should().BeEmpty();
    }

    public void Dispose()
    {
        _sut.Dispose();
    }

    private static GameBoard CreateTestBoard()
    {
        string[] letters = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P"];
        BoardCell[][] cells = new BoardCell[4][];
        for (int row = 0; row < 4; row++)
        {
            cells[row] = new BoardCell[4];
            for (int col = 0; col < 4; col++)
            {
                int i = (row * 4) + col;
                cells[row][col] = new BoardCell(letters[i], row, col);
            }
        }

        return new GameBoard(cells);
    }
}
