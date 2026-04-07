// <copyright file="GameBoardTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
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
    public void Constructor_WithWrongSize_ThrowsArgumentException()
    {
        BoardCell[][] cells = new BoardCell[3][];
        for (int i = 0; i < 3; i++)
        {
            cells[i] = new BoardCell[3];
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
