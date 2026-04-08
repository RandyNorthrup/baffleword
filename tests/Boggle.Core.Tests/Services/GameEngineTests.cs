// <copyright file="GameEngineTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Tests.Services;

using Boggle.Core.Exceptions;
using Boggle.Core.Models;
using Boggle.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public sealed class GameEngineTests
{
    private readonly Mock<IBoardGenerator> _boardGenerator = new();
    private readonly Mock<IWordValidator> _wordValidator = new();
    private readonly Mock<IBoardSolver> _boardSolver = new();
    private readonly Mock<IScoringService> _scoringService = new();
    private readonly GameEngine _sut;

    public GameEngineTests()
    {
        _boardGenerator.Setup(b => b.Generate(It.IsAny<GameMode>())).Returns(CreateTestBoard);
        _sut = new GameEngine(
            _boardGenerator.Object,
            _wordValidator.Object,
            _boardSolver.Object,
            _scoringService.Object,
            NullLogger<GameEngine>.Instance);
    }

    [Fact]
    public void StartRound_CreatesNewRound()
    {
        GameRound round = _sut.StartRound(TimeSpan.FromMinutes(3), 3);

        round.Should().NotBeNull();
        round.State.Should().Be(GameRoundState.Playing);
        round.TimerDuration.Should().Be(TimeSpan.FromMinutes(3));
        round.MinimumWordLength.Should().Be(3);
    }

    [Fact]
    public void StartRound_SetsCurrentRound()
    {
        GameRound round = _sut.StartRound(TimeSpan.FromMinutes(3), 3);

        _sut.CurrentRound.Should().BeSameAs(round);
    }

    [Fact]
    public void SubmitWord_WhenNoActiveRound_ThrowsGameStateException()
    {
        Action act = () => _sut.SubmitWord("TEST");

        act.Should().Throw<GameStateException>();
    }

    [Fact]
    public void SubmitWord_ValidWord_ReturnsValidResult()
    {
        _sut.StartRound(TimeSpan.FromMinutes(3), 3);
        _wordValidator.Setup(v => v.Validate("TEST", It.IsAny<GameBoard>(), 3))
            .Returns(WordStatus.Valid);
        _scoringService.Setup(s => s.CalculateWordScore("TEST", It.IsAny<GameMode>())).Returns(1);

        WordResult result = _sut.SubmitWord("test");

        result.Word.Should().Be("TEST");
        result.Status.Should().Be(WordStatus.Valid);
        result.Points.Should().Be(1);
    }

    [Fact]
    public void SubmitWord_InvalidWord_ReturnsZeroPoints()
    {
        _sut.StartRound(TimeSpan.FromMinutes(3), 3);
        _wordValidator.Setup(v => v.Validate("XYZ", It.IsAny<GameBoard>(), 3))
            .Returns(WordStatus.NotInDictionary);

        WordResult result = _sut.SubmitWord("xyz");

        result.Status.Should().Be(WordStatus.NotInDictionary);
        result.Points.Should().Be(0);
    }

    [Fact]
    public void SubmitWord_DuplicateWord_ReturnsAlreadyFound()
    {
        _sut.StartRound(TimeSpan.FromMinutes(3), 3);
        _wordValidator.Setup(v => v.Validate("TEST", It.IsAny<GameBoard>(), 3))
            .Returns(WordStatus.Valid);
        _scoringService.Setup(s => s.CalculateWordScore("TEST", It.IsAny<GameMode>())).Returns(1);

        _sut.SubmitWord("test");
        WordResult duplicate = _sut.SubmitWord("test");

        duplicate.Status.Should().Be(WordStatus.AlreadyFound);
        duplicate.Points.Should().Be(0);
    }

    [Fact]
    public void SubmitWord_TooShort_ReturnsTooShort()
    {
        _sut.StartRound(TimeSpan.FromMinutes(3), 3);
        _wordValidator.Setup(v => v.Validate("AT", It.IsAny<GameBoard>(), 3))
            .Returns(WordStatus.TooShort);

        WordResult result = _sut.SubmitWord("at");

        result.Status.Should().Be(WordStatus.TooShort);
        result.Points.Should().Be(0);
    }

    [Fact]
    public void EndRound_ReturnsCompletedRound()
    {
        _sut.StartRound(TimeSpan.FromMinutes(3), 3);
        _boardSolver.Setup(s => s.Solve(It.IsAny<GameBoard>(), 3))
            .Returns(new List<string> { "CAT", "DOG", "BAT" });

        GameRound completed = _sut.EndRound();

        completed.State.Should().Be(GameRoundState.Ended);
        completed.TotalPossibleWords.Should().Be(3);
        completed.AllPossibleWords.Should().HaveCount(3);
    }

    [Fact]
    public void EndRound_ClearsCurrentRound()
    {
        _sut.StartRound(TimeSpan.FromMinutes(3), 3);
        _boardSolver.Setup(s => s.Solve(It.IsAny<GameBoard>(), 3))
            .Returns(new List<string>());

        _sut.EndRound();

        _sut.CurrentRound.Should().BeNull();
    }

    [Fact]
    public void EndRound_WhenNoActiveRound_ThrowsGameStateException()
    {
        Action act = () => _sut.EndRound();

        act.Should().Throw<GameStateException>();
    }

    [Fact]
    public void PauseRound_SetsStateToPaused()
    {
        _sut.StartRound(TimeSpan.FromMinutes(3), 3);

        _sut.PauseRound();

        _sut.CurrentRound!.State.Should().Be(GameRoundState.Paused);
    }

    [Fact]
    public void PauseRound_WhenNotPlaying_ThrowsGameStateException()
    {
        Action act = () => _sut.PauseRound();

        act.Should().Throw<GameStateException>();
    }

    [Fact]
    public void ResumeRound_SetsStateToPlaying()
    {
        _sut.StartRound(TimeSpan.FromMinutes(3), 3);
        _sut.PauseRound();

        _sut.ResumeRound();

        _sut.CurrentRound!.State.Should().Be(GameRoundState.Playing);
    }

    [Fact]
    public void ResumeRound_WhenNotPaused_ThrowsGameStateException()
    {
        Action act = () => _sut.ResumeRound();

        act.Should().Throw<GameStateException>();
    }

    [Fact]
    public void EndRound_CalculatesCompletionPercentage()
    {
        _sut.StartRound(TimeSpan.FromMinutes(3), 3);
        _wordValidator.Setup(v => v.Validate("CAT", It.IsAny<GameBoard>(), 3))
            .Returns(WordStatus.Valid);
        _scoringService.Setup(s => s.CalculateWordScore("CAT", It.IsAny<GameMode>())).Returns(1);
        _boardSolver.Setup(s => s.Solve(It.IsAny<GameBoard>(), 3))
            .Returns(new List<string> { "CAT", "DOG" });

        _sut.SubmitWord("cat");
        GameRound completed = _sut.EndRound();

        completed.CompletionPercentage.Should().Be(50.0);
    }

    [Fact]
    public void SubmitWord_WhenPaused_ThrowsGameStateException()
    {
        _sut.StartRound(TimeSpan.FromMinutes(3), 3);
        _sut.PauseRound();

        Action act = () => _sut.SubmitWord("test");

        act.Should().Throw<GameStateException>();
    }

    [Fact]
    public void StartRound_CallsBoardGenerator()
    {
        _sut.StartRound(TimeSpan.FromMinutes(3), 3);

        _boardGenerator.Verify(b => b.Generate(It.IsAny<GameMode>()), Times.Once);
    }

    [Fact]
    public void SubmitWord_NormalizesWordToUpperCase()
    {
        _sut.StartRound(TimeSpan.FromMinutes(3), 3);
        _wordValidator.Setup(v => v.Validate("HELLO", It.IsAny<GameBoard>(), 3))
            .Returns(WordStatus.Valid);
        _scoringService.Setup(s => s.CalculateWordScore("HELLO", It.IsAny<GameMode>())).Returns(2);

        WordResult result = _sut.SubmitWord("  hello  ");

        result.Word.Should().Be("HELLO");
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
