// <copyright file="BoardCell.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Represents a single cell on the Boggle game board.
/// </summary>
public sealed class BoardCell
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoardCell"/> class.
    /// </summary>
    /// <param name="letter">The letter displayed on this cell.</param>
    /// <param name="row">The row position.</param>
    /// <param name="column">The column position.</param>
    /// <param name="isBlocked">Whether this cell is a blocked position.</param>
    public BoardCell(string letter, int row, int column, bool isBlocked = false)
    {
        ArgumentNullException.ThrowIfNull(letter);
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfNegative(column);

        IsBlocked = isBlocked;
        Letter = isBlocked ? string.Empty : letter.ToUpperInvariant();
        Row = row;
        Column = column;
    }

    /// <summary>
    /// Gets the letter displayed on this cell (e.g., "A", "QU", "TH").
    /// </summary>
    public string Letter { get; }

    /// <summary>
    /// Gets the row position.
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// Gets the column position.
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Gets a value indicating whether this cell is a blocked position (cannot be used in words).
    /// </summary>
    public bool IsBlocked { get; }

    /// <summary>
    /// Gets a value indicating whether this cell contains a two-letter digraph (e.g., "QU", "TH", "IN").
    /// </summary>
    public bool IsDigraph => !IsBlocked && Letter.Length > 1;

    /// <summary>
    /// Determines whether this cell is adjacent to another cell (including diagonals).
    /// </summary>
    /// <param name="other">The other cell to check adjacency against.</param>
    /// <returns><see langword="true"/> if the cells are adjacent; otherwise, <see langword="false"/>.</returns>
    public bool IsAdjacentTo(BoardCell other)
    {
        ArgumentNullException.ThrowIfNull(other);

        if (Row == other.Row && Column == other.Column)
        {
            return false;
        }

        int rowDiff = Math.Abs(Row - other.Row);
        int colDiff = Math.Abs(Column - other.Column);
        return rowDiff <= 1 && colDiff <= 1;
    }
}
