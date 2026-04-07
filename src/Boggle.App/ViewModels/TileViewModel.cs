// <copyright file="TileViewModel.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.ViewModels;

/// <summary>
/// ViewModel for a single board tile supporting drag selection.
/// </summary>
public sealed class TileViewModel : ViewModelBase
{
    private bool _isSelected;
    private int _selectionOrder;

    /// <summary>
    /// Initializes a new instance of the <see cref="TileViewModel"/> class.
    /// </summary>
    /// <param name="letter">The letter displayed on this tile.</param>
    /// <param name="row">The row position (0-3).</param>
    /// <param name="column">The column position (0-3).</param>
    public TileViewModel(string letter, int row, int column)
    {
        Letter = letter;
        Row = row;
        Column = column;
    }

    /// <summary>
    /// Gets the letter displayed on this tile.
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
    /// Gets or sets a value indicating whether this tile is part of the current selection.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    /// <summary>
    /// Gets or sets the 1-based order in which this tile was selected.
    /// </summary>
    public int SelectionOrder
    {
        get => _selectionOrder;
        set => SetProperty(ref _selectionOrder, value);
    }

    /// <summary>
    /// Determines whether this tile is adjacent to another tile.
    /// </summary>
    /// <param name="other">The other tile.</param>
    /// <returns><see langword="true"/> if the tiles are adjacent.</returns>
    public bool IsAdjacentTo(TileViewModel other)
    {
        if (other is null || (Row == other.Row && Column == other.Column))
        {
            return false;
        }

        return Math.Abs(Row - other.Row) <= 1 && Math.Abs(Column - other.Column) <= 1;
    }
}
