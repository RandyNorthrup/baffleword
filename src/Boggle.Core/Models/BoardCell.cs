// <copyright file="BoardCell.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
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
    /// <param name="row">The row position (0-3).</param>
    /// <param name="column">The column position (0-3).</param>
    public BoardCell(string letter, int row, int column)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(letter);
        ArgumentOutOfRangeException.ThrowIfNegative(row);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(row, 3);
        ArgumentOutOfRangeException.ThrowIfNegative(column);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(column, 3);

        Letter = letter.ToUpperInvariant();
        Row = row;
        Column = column;
    }

    /// <summary>
    /// Gets the letter displayed on this cell (e.g., "A", "Qu").
    /// </summary>
    public string Letter { get; }

    /// <summary>
    /// Gets the row position (0-3).
    /// </summary>
    public int Row { get; }

    /// <summary>
    /// Gets the column position (0-3).
    /// </summary>
    public int Column { get; }

    /// <summary>
    /// Gets a value indicating whether this cell contains the "Qu" digraph.
    /// </summary>
    public bool IsQu => string.Equals(Letter, "QU", StringComparison.Ordinal);

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
