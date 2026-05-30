// <copyright file="GameView.axaml.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.Views;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using Baffleword.App.ViewModels;

/// <summary>
/// Interactive game board view.
/// </summary>
public sealed partial class GameView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameView"/> class.
    /// </summary>
    public GameView()
    {
        InitializeComponent();
    }

    private static TileViewModel? GetTileFromElement(object? element)
    {
        if (element is not Visual visual)
        {
            return null;
        }

        return visual
            .GetSelfAndVisualAncestors()
            .OfType<Control>()
            .Select(control => control.Tag as TileViewModel ?? control.DataContext as TileViewModel)
            .FirstOrDefault(tile => tile is not null);
    }

    private static Border? GetTileBorder(object? element)
    {
        if (element is not Visual visual)
        {
            return null;
        }

        return visual
            .GetSelfAndVisualAncestors()
            .OfType<Border>()
            .FirstOrDefault(border => border.Tag is TileViewModel);
    }

    private static bool IsPointerInsideTileCenter(Border border, PointerEventArgs e)
    {
        double width = border.Bounds.Width;
        double height = border.Bounds.Height;
        if (width <= 0 || height <= 0)
        {
            return false;
        }

        double marginX = width * 0.16;
        double marginY = height * 0.16;
        Point position = e.GetPosition(border);

        return position.X >= marginX &&
            position.X <= width - marginX &&
            position.Y >= marginY &&
            position.Y <= height - marginY;
    }

    private TileViewModel? GetTileAtPointer(PointerEventArgs e)
    {
        IInputElement? hitElement = BoardGrid.InputHitTest(e.GetPosition(BoardGrid));
        return GetTileFromElement(hitElement);
    }

    private void OnBoardPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not GameViewModel viewModel ||
            !e.GetCurrentPoint(BoardGrid).Properties.IsLeftButtonPressed)
        {
            return;
        }

        TileViewModel? tile = GetTileAtPointer(e);
        if (tile is null)
        {
            return;
        }

        viewModel.BeginDragSelection(tile);
        UpdateDragLine();
        e.Pointer.Capture(BoardGrid);
        e.Handled = true;
    }

    private void OnBoardPointerMoved(object? sender, PointerEventArgs e)
    {
        if (DataContext is not GameViewModel viewModel || !viewModel.IsDragging)
        {
            return;
        }

        if (!e.GetCurrentPoint(BoardGrid).Properties.IsLeftButtonPressed)
        {
            EndDragSelection(e);
            return;
        }

        IInputElement? hitElement = BoardGrid.InputHitTest(e.GetPosition(BoardGrid));
        Border? tileBorder = GetTileBorder(hitElement);
        TileViewModel? tile = GetTileFromElement(hitElement);

        if (tileBorder is not null && tile is not null && IsPointerInsideTileCenter(tileBorder, e))
        {
            viewModel.ExtendDragSelection(tile);
            UpdateDragLine();
        }
    }

    private void OnBoardPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        EndDragSelection(e);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        EndDragSelection(e);
    }

    private void EndDragSelection(PointerEventArgs e)
    {
        if (DataContext is GameViewModel viewModel)
        {
            viewModel.EndDragSelection();
            ClearDragLine();
            e.Pointer.Capture(null);
            e.Handled = true;
        }
    }

    private void UpdateDragLine()
    {
        DragLineCanvas.Children.Clear();

        if (DataContext is not GameViewModel viewModel || !viewModel.IsDragging)
        {
            return;
        }

        List<TileViewModel> selectedTiles = viewModel.Tiles
            .Where(tile => tile.IsSelected)
            .OrderBy(tile => tile.SelectionOrder)
            .ToList();

        if (selectedTiles.Count < 2)
        {
            return;
        }

        var polyline = new Polyline
        {
            Stroke = new SolidColorBrush(Color.FromRgb(66, 133, 244)),
            StrokeThickness = 4,
            StrokeLineCap = PenLineCap.Round,
            Opacity = 0.7,
        };

        foreach (TileViewModel tile in selectedTiles)
        {
            Border? border = BoardGrid
                .GetVisualDescendants()
                .OfType<Border>()
                .FirstOrDefault(candidate => ReferenceEquals(candidate.Tag, tile));

            Point? center = border?.TranslatePoint(
                new Point(border.Bounds.Width / 2, border.Bounds.Height / 2),
                DragLineCanvas);

            if (center.HasValue)
            {
                polyline.Points.Add(center.Value);
            }
        }

        DragLineCanvas.Children.Add(polyline);
    }

    private void ClearDragLine()
    {
        DragLineCanvas.Children.Clear();
    }
}
