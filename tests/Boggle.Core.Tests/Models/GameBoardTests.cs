// <copyright file="GameBoardTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Tests.Models;

using Boggle.Core.Models;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the <see cref="GameBoard"/> class.
/// </summary>
public sealed class GameBoardTests
{
    [Fact]
    public void Constructor_WithValid4x4Cells_CreatesBoard()
    {
        GameBoard board = CreateTestBoard();

        board[0, 0].Letter.Should().Be("A");
        board[3, 3].Letter.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithMismatchedColumnCounts_ThrowsArgumentException()
    {
        BoardCell[][] cells = new BoardCell[2][];
        cells[0] = new BoardCell[3];
        cells[1] = new BoardCell[4];

        for (int col = 0; col < 3; col++)
        {
            cells[0][col] = new BoardCell("A", 0, col);
        }

        for (int col = 0; col < 4; col++)
        {
            cells[1][col] = new BoardCell("A", 1, col);
        }

        Action act = () => new GameBoard(cells);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_WithNull_ThrowsArgumentNullException()
    {
        Action act = () => new GameBoard(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AllCells_Returns16Cells()
    {
        GameBoard board = CreateTestBoard();

        board.AllCells.Should().HaveCount(16);
    }

    [Fact]
    public void GetNeighbors_CornerCell_Returns3Neighbors()
    {
        GameBoard board = CreateTestBoard();
        BoardCell corner = board[0, 0];

        board.GetNeighbors(corner).Should().HaveCount(3);
    }

    [Fact]
    public void GetNeighbors_EdgeCell_Returns5Neighbors()
    {
        GameBoard board = CreateTestBoard();
        BoardCell edge = board[0, 1];

        board.GetNeighbors(edge).Should().HaveCount(5);
    }

    [Fact]
    public void GetNeighbors_CenterCell_Returns8Neighbors()
    {
        GameBoard board = CreateTestBoard();
        BoardCell center = board[1, 1];

        board.GetNeighbors(center).Should().HaveCount(8);
    }

    [Fact]
    public void GetNeighbors_WithNull_ThrowsArgumentNullException()
    {
        GameBoard board = CreateTestBoard();

        Action act = () => board.GetNeighbors(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_5x5Board_SetsCorrectDimensions()
    {
        GameBoard board = Create5x5Board();

        board.Rows.Should().Be(5);
        board.Columns.Should().Be(5);
        board.AllCells.Should().HaveCount(25);
    }

    [Fact]
    public void Constructor_6x6Board_SetsCorrectDimensions()
    {
        GameBoard board = Create6x6Board();

        board.Rows.Should().Be(6);
        board.Columns.Should().Be(6);
        board.AllCells.Should().HaveCount(36);
    }

    [Fact]
    public void GetNeighbors_5x5CenterCell_Returns8Neighbors()
    {
        GameBoard board = Create5x5Board();
        BoardCell center = board[2, 2];

        board.GetNeighbors(center).Should().HaveCount(8);
    }

    [Fact]
    public void GetNeighbors_5x5CornerCell_Returns3Neighbors()
    {
        GameBoard board = Create5x5Board();
        BoardCell corner = board[0, 0];

        board.GetNeighbors(corner).Should().HaveCount(3);
    }

    [Fact]
    public void GetNeighbors_6x6CenterCell_Returns8Neighbors()
    {
        GameBoard board = Create6x6Board();
        BoardCell center = board[3, 3];

        board.GetNeighbors(center).Should().HaveCount(8);
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

    private static GameBoard Create5x5Board()
    {
        BoardCell[][] cells = new BoardCell[5][];
        for (int row = 0; row < 5; row++)
        {
            cells[row] = new BoardCell[5];
            for (int col = 0; col < 5; col++)
            {
                cells[row][col] = new BoardCell(((char)('A' + (((row * 5) + col) % 26))).ToString(), row, col);
            }
        }

        return new GameBoard(cells);
    }

    private static GameBoard Create6x6Board()
    {
        BoardCell[][] cells = new BoardCell[6][];
        for (int row = 0; row < 6; row++)
        {
            cells[row] = new BoardCell[6];
            for (int col = 0; col < 6; col++)
            {
                cells[row][col] = new BoardCell(((char)('A' + (((row * 6) + col) % 26))).ToString(), row, col);
            }
        }

        return new GameBoard(cells);
    }
}
