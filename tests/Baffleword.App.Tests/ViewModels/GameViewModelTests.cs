// <copyright file="GameViewModelTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.Tests.ViewModels;

using Baffleword.App.Navigation;
using Baffleword.App.Services;
using Baffleword.App.ViewModels;
using Baffleword.Audio;
using Baffleword.Core.Models;
using Baffleword.Core.Repositories;
using Baffleword.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public sealed class GameViewModelTests : IDisposable
{
    private readonly Mock<IGameEngine> _gameEngine = new();
    private readonly Mock<INavigationService> _navigation = new();
    private readonly Mock<IAchievementService> _achievementService = new();
    private readonly Mock<IAchievementRepository> _achievementRepository = new();
    private readonly Mock<IHighScoreService> _highScoreService = new();
    private readonly Mock<IStatisticsService> _statisticsService = new();
    private readonly Mock<IAudioManager> _audioManager = new();
    private readonly Mock<ISoundEffectPlayer> _soundEffects = new();
    private readonly Mock<ISettingsRepository> _settingsRepository = new();
    private readonly Mock<IToastService> _toastService = new();
    private readonly GameViewModel _sut;

    public GameViewModelTests()
    {
        _audioManager.Setup(a => a.SoundEffects).Returns(_soundEffects.Object);
        _gameEngine.Setup(g => g.StartRound(It.IsAny<TimeSpan>(), It.IsAny<int>(), It.IsAny<GameMode>()))
            .Returns(new GameRound(CreateTestBoard(), TimeSpan.FromMinutes(3), 3));
        _statisticsService.Setup(s => s.GetStatisticsAsync())
            .ReturnsAsync(new GameStatistics());

        _sut = new GameViewModel(
            _gameEngine.Object,
            _navigation.Object,
            _achievementService.Object,
            _achievementRepository.Object,
            _highScoreService.Object,
            _statisticsService.Object,
            _audioManager.Object,
            _settingsRepository.Object,
            _toastService.Object,
            NullLogger<GameViewModel>.Instance);
    }

    [Fact]
    public void Constructor_StartsRound()
    {
        _gameEngine.Verify(g => g.StartRound(TimeSpan.FromMinutes(3), 3, It.IsAny<GameMode>()), Times.Once);
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
    public void Constructor_SetsTimeRemainingText()
    {
        _sut.TimeRemainingText.Should().Be("3:00");
    }

    [Fact]
    public void Constructor_WithCustomSettings_UsesSettingValues()
    {
        var settingsRepo = new Mock<ISettingsRepository>();
        settingsRepo.Setup(s => s.GetAsync("GameMode")).ReturnsAsync("BigBoard");

        var gameEngine = new Mock<IGameEngine>();
        gameEngine.Setup(g => g.StartRound(It.IsAny<TimeSpan>(), It.IsAny<int>(), It.IsAny<GameMode>()))
            .Returns(new GameRound(CreateTestBoard(), TimeSpan.FromMinutes(3), 4));

        var audioManager = new Mock<IAudioManager>();
        audioManager.Setup(a => a.SoundEffects).Returns(new Mock<ISoundEffectPlayer>().Object);

        using var vm = new GameViewModel(
            gameEngine.Object,
            _navigation.Object,
            _achievementService.Object,
            _achievementRepository.Object,
            _highScoreService.Object,
            _statisticsService.Object,
            audioManager.Object,
            settingsRepo.Object,
            new Mock<IToastService>().Object,
            NullLogger<GameViewModel>.Instance);

        settingsRepo.Verify(s => s.GetAsync("GameMode"), Times.Once);
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

    [Fact]
    public void GroupedFoundWords_InitiallyEmpty()
    {
        _sut.GroupedFoundWords.Should().BeEmpty();
    }

    [Fact]
    public void GroupedFoundWords_GroupsByWordLength()
    {
        _sut.FoundWords.Add(new WordResult("TREE", WordStatus.Valid, 1));
        _sut.FoundWords.Add(new WordResult("PLANT", WordStatus.Valid, 2));
        _sut.FoundWords.Add(new WordResult("FISH", WordStatus.Valid, 1));

        _sut.GroupedFoundWords.Should().HaveCount(2);
        _sut.GroupedFoundWords[0].HeaderText.Should().Be("5 Letters(1)");
        _sut.GroupedFoundWords[1].HeaderText.Should().Be("4 Letters(2)");
        _sut.GroupedFoundWords[1].Words.Select(word => word.Word).Should().Equal("FISH", "TREE");
    }

    [Fact]
    public void DragSelection_ExtendsAdjacentTilesAndSubmitsWord()
    {
        _gameEngine.Setup(g => g.SubmitWord("ABF"))
            .Returns(new WordResult("ABF", WordStatus.Valid, 1));

        _sut.BeginDragSelection(_sut.Tiles[0]);
        _sut.ExtendDragSelection(_sut.Tiles[1]);
        _sut.ExtendDragSelection(_sut.Tiles[5]);

        _sut.CurrentWord.Should().Be("ABF");

        _sut.EndDragSelection();

        _gameEngine.Verify(g => g.SubmitWord("ABF"), Times.Once);
        _soundEffects.Verify(player => player.Play(SoundEffect.TileClick), Times.Exactly(3));
        _sut.CurrentWord.Should().BeEmpty();
        _sut.Tiles.Should().OnlyContain(tile => !tile.IsSelected);
    }

    [Fact]
    public void DragSelection_BacktrackingRemovesLastTile()
    {
        _sut.BeginDragSelection(_sut.Tiles[0]);
        _sut.ExtendDragSelection(_sut.Tiles[1]);

        _sut.ExtendDragSelection(_sut.Tiles[0]);

        _sut.CurrentWord.Should().Be("A");
        _sut.Tiles[0].IsSelected.Should().BeTrue();
        _sut.Tiles[1].IsSelected.Should().BeFalse();
    }

    [Fact]
    public void DragSelection_DoesNotStartWhenPaused()
    {
        _sut.PauseCommand.Execute(null);

        _sut.BeginDragSelection(_sut.Tiles[0]);

        _sut.IsDragging.Should().BeFalse();
        _sut.CurrentWord.Should().BeEmpty();
        _sut.Tiles[0].IsSelected.Should().BeFalse();
    }

    [Fact]
    public void EndDragSelection_AfterQuit_ClearsSelectionWithoutSubmitting()
    {
        _gameEngine.Setup(g => g.EndRound()).Returns(new GameRound(CreateTestBoard(), TimeSpan.FromMinutes(3), 3));

        _sut.BeginDragSelection(_sut.Tiles[0]);
        _sut.QuitCommand.Execute(null);

        Action act = _sut.EndDragSelection;

        act.Should().NotThrow();
        _gameEngine.Verify(g => g.SubmitWord(It.IsAny<string>()), Times.Never);
        _sut.IsDragging.Should().BeFalse();
        _sut.CurrentWord.Should().BeEmpty();
        _sut.Tiles.Should().OnlyContain(tile => !tile.IsSelected);
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
