// <copyright file="RoundResultsViewModelTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Tests.ViewModels;

using Boggle.App.Navigation;
using Boggle.App.ViewModels;
using Boggle.Core.Models;
using FluentAssertions;
using Moq;
using Xunit;

public sealed class RoundResultsViewModelTests
{
    private readonly Mock<INavigationService> _navigation = new();
    private readonly RoundResultsViewModel _sut;

    public RoundResultsViewModelTests()
    {
        _sut = new RoundResultsViewModel(_navigation.Object);
    }

    [Fact]
    public void LoadResults_SetsScore()
    {
        GameRound round = CreateCompletedRound();

        _sut.LoadResults(round);

        _sut.Score.Should().Be(round.Score);
    }

    [Fact]
    public void LoadResults_SetsWordsFound()
    {
        GameRound round = CreateCompletedRound();

        _sut.LoadResults(round);

        _sut.WordsFound.Should().Be(round.ValidWordCount);
    }

    [Fact]
    public void LoadResults_SetsCompletionPercentage()
    {
        GameRound round = CreateCompletedRound();
        round.TotalPossibleWords = 10;

        _sut.LoadResults(round);

        _sut.CompletionPercentage.Should().Be(round.CompletionPercentage);
    }

    [Fact]
    public void LoadResults_SetsLongestWord()
    {
        GameRound round = CreateCompletedRound();

        _sut.LoadResults(round);

        _sut.LongestWord.Should().Be("TESTING");
    }

    [Fact]
    public void LoadResults_PopulatesFoundWords()
    {
        GameRound round = CreateCompletedRound();

        _sut.LoadResults(round);

        _sut.FoundWords.Should().HaveCount(2);
    }

    [Fact]
    public void LoadResults_PopulatesMissedWords()
    {
        GameRound round = CreateCompletedRound();
        round.AllPossibleWords = new List<string> { "CAT", "TESTING", "HELLO", "WORLD" };

        _sut.LoadResults(round);

        _sut.MissedWords.Should().HaveCount(2);
        _sut.MissedWords.Should().Contain("HELLO");
        _sut.MissedWords.Should().Contain("WORLD");
    }

    [Fact]
    public void NewRoundCommand_NavigatesToGameViewModel()
    {
        _sut.NewRoundCommand.Execute(null);

        _navigation.Verify(n => n.NavigateTo<GameViewModel>(), Times.Once);
    }

    [Fact]
    public void MainMenuCommand_NavigatesToMainMenu()
    {
        _sut.MainMenuCommand.Execute(null);

        _navigation.Verify(n => n.NavigateTo<MainMenuViewModel>(), Times.Once);
    }

    [Fact]
    public void Constructor_WithNullNavigation_ThrowsArgumentNullException()
    {
        Action act = () => new RoundResultsViewModel(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void LoadResults_WithNullRound_ThrowsArgumentNullException()
    {
        Action act = () => _sut.LoadResults(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void LoadResults_NoValidWords_LongestWordIsEmpty()
    {
        GameRound round = new(CreateTestBoard(), TimeSpan.FromMinutes(3), 3);
        round.AddWordResult(new WordResult("XY", WordStatus.TooShort, 0));

        _sut.LoadResults(round);

        _sut.LongestWord.Should().BeEmpty();
    }

    private static GameRound CreateCompletedRound()
    {
        GameRound round = new(CreateTestBoard(), TimeSpan.FromMinutes(3), 3);
        round.AddWordResult(new WordResult("CAT", WordStatus.Valid, 1));
        round.AddWordResult(new WordResult("TESTING", WordStatus.Valid, 5));
        return round;
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
