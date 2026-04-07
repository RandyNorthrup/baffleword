// <copyright file="GameBoard.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Represents a 4×4 Boggle game board.
/// </summary>
public sealed class GameBoard
{
    /// <summary>
    /// The number of rows on the board.
    /// </summary>
    public const int Rows = 4;

    /// <summary>
    /// The number of columns on the board.
    /// </summary>
    public const int Columns = 4;

    /// <summary>
    /// The total number of cells on the board.
    /// </summary>
    public const int TotalCells = Rows * Columns;

    private readonly BoardCell[][] _cells;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameBoard"/> class.
    /// </summary>
    /// <param name="cells">A 4×4 jagged array of board cells.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "S2368:Public methods should not have multidimensional array parameters", Justification = "A 4x4 jagged array is the natural representation of a Boggle board.")]
    public GameBoard(BoardCell[][] cells)
    {
        ArgumentNullException.ThrowIfNull(cells);

        if (cells.Length != Rows)
        {
            throw new ArgumentException($"Board must have {Rows} rows.", nameof(cells));
        }

        for (int i = 0; i < Rows; i++)
        {
            if (cells[i] is null || cells[i].Length != Columns)
            {
                throw new ArgumentException($"Each row must have {Columns} columns.", nameof(cells));
            }
        }

        _cells = cells;
    }

    /// <summary>
    /// Gets all cells on the board as a flat list.
    /// </summary>
    public IReadOnlyList<BoardCell> AllCells
    {
        get
        {
            var result = new List<BoardCell>(TotalCells);
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    result.Add(_cells[row][col]);
                }
            }

            return result;
        }
    }

    /// <summary>
    /// Gets the cell at the specified position.
    /// </summary>
    /// <param name="row">The row index (0-3).</param>
    /// <param name="column">The column index (0-3).</param>
    /// <returns>The board cell at the specified position.</returns>
    public BoardCell this[int row, int column] => _cells[row][column];

    /// <summary>
    /// Gets all cells adjacent to the specified cell.
    /// </summary>
    /// <param name="cell">The cell to find neighbors for.</param>
    /// <returns>All adjacent cells (up to 8).</returns>
    public IReadOnlyList<BoardCell> GetNeighbors(BoardCell cell)
    {
        ArgumentNullException.ThrowIfNull(cell);

        var neighbors = new List<BoardCell>(8);
        for (int row = Math.Max(0, cell.Row - 1); row <= Math.Min(Rows - 1, cell.Row + 1); row++)
        {
            for (int col = Math.Max(0, cell.Column - 1); col <= Math.Min(Columns - 1, cell.Column + 1); col++)
            {
                if (row != cell.Row || col != cell.Column)
                {
                    neighbors.Add(_cells[row][col]);
                }
            }
        }

        return neighbors;
    }
}
