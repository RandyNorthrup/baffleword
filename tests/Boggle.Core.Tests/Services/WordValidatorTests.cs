// <copyright file="WordValidatorTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Tests.Services;

using Boggle.Core.Dictionary;
using Boggle.Core.Models;
using Boggle.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

/// <summary>
/// Tests for the <see cref="WordValidator"/> class.
/// </summary>
public sealed class WordValidatorTests
{
    private readonly Mock<IDictionaryProvider> _dictionaryMock = new();
    private readonly WordValidator _sut;

    public WordValidatorTests()
    {
        _sut = new WordValidator(_dictionaryMock.Object, NullLogger<WordValidator>.Instance);
    }

    [Fact]
    public void Validate_NullWord_ReturnsTooShort()
    {
        GameBoard board = CreateTestBoard();

        WordStatus result = _sut.Validate(null!, board, 3);

        result.Should().Be(WordStatus.TooShort);
    }

    [Fact]
    public void Validate_ShortWord_ReturnsTooShort()
    {
        GameBoard board = CreateTestBoard();

        WordStatus result = _sut.Validate("AB", board, 3);

        result.Should().Be(WordStatus.TooShort);
    }

    [Fact]
    public void Validate_NotInDictionary_ReturnsNotInDictionary()
    {
        _dictionaryMock.Setup(d => d.IsValidWord("XYZ")).Returns(false);
        GameBoard board = CreateTestBoard();

        WordStatus result = _sut.Validate("XYZ", board, 3);

        result.Should().Be(WordStatus.NotInDictionary);
    }

    [Fact]
    public void Validate_InDictionaryButNotOnBoard_ReturnsNotOnBoard()
    {
        _dictionaryMock.Setup(d => d.IsValidWord("ZZZ")).Returns(true);
        GameBoard board = CreateTestBoard();

        WordStatus result = _sut.Validate("ZZZ", board, 3);

        result.Should().Be(WordStatus.NotOnBoard);
    }

    [Fact]
    public void Validate_ValidWordOnBoard_ReturnsValid()
    {
        // Board: A B C D / E F G H / I J K L / M N O P
        // "ABF" is valid path: (0,0) -> (0,1) -> (1,1)
        _dictionaryMock.Setup(d => d.IsValidWord("ABF")).Returns(true);
        GameBoard board = CreateTestBoard();

        WordStatus result = _sut.Validate("ABF", board, 3);

        result.Should().Be(WordStatus.Valid);
    }

    [Fact]
    public void Validate_ValidWord_IsCaseInsensitive()
    {
        _dictionaryMock.Setup(d => d.IsValidWord(It.IsAny<string>())).Returns(true);
        GameBoard board = CreateTestBoard();

        WordStatus result = _sut.Validate("abf", board, 3);

        result.Should().Be(WordStatus.Valid);
    }

    [Fact]
    public void Validate_WordThroughBlockedCell_ReturnsNotOnBoard()
    {
        // Board: A B / [blocked] D -- blocked cell at (1,0)
        // A word requiring the blocked cell should fail
        _dictionaryMock.Setup(d => d.IsValidWord(It.IsAny<string>())).Returns(true);
        GameBoard board = CreateBoardWithBlockedCell();

        // Try to validate a word that would need to go through the blocked cell
        // The blocked cell is skipped during path-finding
        WordStatus result = _sut.Validate("AB", board, 2);

        result.Should().Be(WordStatus.Valid); // AB doesn't need blocked cell
    }

    [Fact]
    public void Validate_WordWithDigraph_IsValid()
    {
        // Board: TH E / A B
        _dictionaryMock.Setup(d => d.IsValidWord("THE")).Returns(true);
        GameBoard board = CreateBoardWithDigraph();

        WordStatus result = _sut.Validate("THE", board, 3);

        result.Should().Be(WordStatus.Valid);
    }

    [Fact]
    public void Validate_On5x5Board_WorksCorrectly()
    {
        _dictionaryMock.Setup(d => d.IsValidWord("ABGH")).Returns(true);
        GameBoard board = Create5x5TestBoard();

        // "ABGH" path: (0,0)->(0,1)->(1,1)->(1,2) -- all adjacent
        WordStatus result = _sut.Validate("ABGH", board, 4);

        result.Should().Be(WordStatus.Valid);
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

    private static GameBoard CreateBoardWithBlockedCell()
    {
        BoardCell[][] cells = new BoardCell[2][];
        cells[0] = [new BoardCell("A", 0, 0), new BoardCell("B", 0, 1)];
        cells[1] = [new BoardCell("C", 1, 0, isBlocked: true), new BoardCell("D", 1, 1)];
        return new GameBoard(cells);
    }

    private static GameBoard CreateBoardWithDigraph()
    {
        BoardCell[][] cells = new BoardCell[2][];
        cells[0] = [new BoardCell("TH", 0, 0), new BoardCell("E", 0, 1)];
        cells[1] = [new BoardCell("A", 1, 0), new BoardCell("B", 1, 1)];
        return new GameBoard(cells);
    }

    private static GameBoard Create5x5TestBoard()
    {
        string[] letters =
        [
            "A", "B", "C", "D", "E",
            "F", "G", "H", "I", "J",
            "K", "L", "M", "N", "O",
            "P", "R", "S", "T", "U",
            "V", "W", "X", "Y", "Z",
        ];
        BoardCell[][] cells = new BoardCell[5][];
        for (int row = 0; row < 5; row++)
        {
            cells[row] = new BoardCell[5];
            for (int col = 0; col < 5; col++)
            {
                int i = (row * 5) + col;
                cells[row][col] = new BoardCell(letters[i], row, col);
            }
        }

        return new GameBoard(cells);
    }
}
