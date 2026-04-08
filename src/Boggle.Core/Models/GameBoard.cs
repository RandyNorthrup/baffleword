// <copyright file="GameBoard.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Represents a Boggle game board of variable size.
/// </summary>
public sealed class GameBoard
{
    private readonly BoardCell[][] _cells;
    private IReadOnlyList<BoardCell>? _allCellsCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameBoard"/> class.
    /// </summary>
    /// <param name="cells">A jagged array of board cells.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "S2368:Public methods should not have multidimensional array parameters", Justification = "A jagged array is the natural representation of a Boggle board.")]
    public GameBoard(BoardCell[][] cells)
    {
        ArgumentNullException.ThrowIfNull(cells);

        if (cells.Length == 0)
        {
            throw new ArgumentException("Board must have at least one row.", nameof(cells));
        }

        int columns = cells[0]?.Length ?? 0;
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i] is null || cells[i].Length != columns)
            {
                throw new ArgumentException("All rows must have the same number of columns.", nameof(cells));
            }
        }

        Rows = cells.Length;
        Columns = columns;
        _cells = cells;
    }

    /// <summary>
    /// Gets the number of rows on the board.
    /// </summary>
    public int Rows { get; }

    /// <summary>
    /// Gets the number of columns on the board.
    /// </summary>
    public int Columns { get; }

    /// <summary>
    /// Gets the total number of cells on the board.
    /// </summary>
    public int TotalCells => Rows * Columns;

    /// <summary>
    /// Gets all cells on the board as a flat list.
    /// </summary>
    public IReadOnlyList<BoardCell> AllCells
    {
        get
        {
            if (_allCellsCache is not null)
            {
                return _allCellsCache;
            }

            var result = new BoardCell[TotalCells];
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    result[(row * Columns) + col] = _cells[row][col];
                }
            }

            _allCellsCache = result;
            return _allCellsCache;
        }
    }

    /// <summary>
    /// Gets the cell at the specified position.
    /// </summary>
    /// <param name="row">The row index.</param>
    /// <param name="column">The column index.</param>
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
