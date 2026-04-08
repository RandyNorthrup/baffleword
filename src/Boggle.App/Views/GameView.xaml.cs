// <copyright file="GameView.xaml.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Views;

using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Boggle.App.ViewModels;
using Boggle.Core.Models;

/// <summary>
/// Game view with board, timer, drag-to-select, and pause overlay.
/// </summary>
public partial class GameView : UserControl
{
    private string _previousFeedback = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameView"/> class.
    /// </summary>
    public GameView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        DataContextChanged += OnDataContextChanged;
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
            {
                yield return match;
            }

            foreach (T grandchild in FindVisualChildren<T>(child))
            {
                yield return grandchild;
            }
        }
    }

    private static TileViewModel? GetTileFromElement(DependencyObject? element)
    {
        while (element != null)
        {
            if (element is FrameworkElement fe && fe.DataContext is TileViewModel tile)
            {
                return tile;
            }

            element = VisualTreeHelper.GetParent(element);
        }

        return null;
    }

    private static Border? GetTileBorder(DependencyObject? element)
    {
        while (element != null)
        {
            if (element is Border b && b.DataContext is TileViewModel)
            {
                return b;
            }

            element = VisualTreeHelper.GetParent(element);
        }

        return null;
    }

    private static bool IsCursorInsideTileCenter(DependencyObject hitElement, MouseEventArgs e)
    {
        Border? border = GetTileBorder(hitElement);
        if (border == null)
        {
            return false;
        }

        double margin = border.ActualWidth * 0.16;
        Point pos = e.GetPosition(border);
        return pos.X >= margin && pos.X <= border.ActualWidth - margin &&
               pos.Y >= margin && pos.Y <= border.ActualHeight - margin;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AnimateBoardEntrance();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is INotifyPropertyChanged oldVm)
        {
            oldVm.PropertyChanged -= OnViewModelPropertyChanged;
        }

        if (e.OldValue is GameViewModel oldGameVm)
        {
            oldGameVm.AchievementToasts.CollectionChanged -= OnToastsChanged;
        }

        if (e.NewValue is INotifyPropertyChanged newVm)
        {
            newVm.PropertyChanged += OnViewModelPropertyChanged;
        }

        if (e.NewValue is GameViewModel newGameVm)
        {
            newGameVm.AchievementToasts.CollectionChanged += OnToastsChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameViewModel.LastSubmissionFeedback))
        {
            string newFeedback = FeedbackText.Text;
            if (!string.IsNullOrEmpty(newFeedback) && newFeedback != _previousFeedback)
            {
                _previousFeedback = newFeedback;
                AnimateFeedbackPulse();
            }
        }
    }

    private void BoardGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not GameViewModel vm)
        {
            return;
        }

        TileViewModel? tile = GetTileFromElement(e.OriginalSource as DependencyObject);
        if (tile != null)
        {
            vm.BeginDragSelection(tile);
            UpdateDragLine();
            BoardGrid.CaptureMouse();
            e.Handled = true;
        }
    }

    private void BoardGrid_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (DataContext is not GameViewModel vm || !vm.IsDragging)
        {
            return;
        }

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            vm.EndDragSelection();
            ClearDragLine();
            BoardGrid.ReleaseMouseCapture();
            return;
        }

        Point pos = e.GetPosition(BoardGrid);
        HitTestResult? hit = VisualTreeHelper.HitTest(BoardGrid, pos);
        if (hit?.VisualHit is DependencyObject hitElement)
        {
            TileViewModel? tile = GetTileFromElement(hitElement);
            if (tile != null && IsCursorInsideTileCenter(hitElement, e))
            {
                vm.ExtendDragSelection(tile);
                UpdateDragLine();
            }
        }
    }

    private void BoardGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not GameViewModel vm)
        {
            return;
        }

        vm.EndDragSelection();
        ClearDragLine();
        BoardGrid.ReleaseMouseCapture();
        e.Handled = true;
    }

    private void UpdateDragLine()
    {
        DragLineCanvas.Children.Clear();

        if (DataContext is not GameViewModel vm || !vm.IsDragging)
        {
            return;
        }

        // Find the UniformGrid inside the ItemsControl
        UniformGrid? grid = FindVisualChildren<UniformGrid>(BoardGrid).FirstOrDefault();
        if (grid == null)
        {
            return;
        }

        var selectedTiles = vm.Tiles.Where(t => t.IsSelected).OrderBy(t => t.SelectionOrder).ToList();
        if (selectedTiles.Count < 2)
        {
            return;
        }

        var polyline = new Polyline
        {
            Stroke = new SolidColorBrush(Color.FromRgb(66, 133, 244)),
            StrokeThickness = 4,
            StrokeLineJoin = PenLineJoin.Round,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            Opacity = 0.7,
        };

        foreach (TileViewModel tile in selectedTiles)
        {
            int index = (tile.Row * vm.BoardColumns) + tile.Column;
            if (index < grid.Children.Count && grid.Children[index] is UIElement element)
            {
                Point center = element.TranslatePoint(
                    new Point(element.RenderSize.Width / 2, element.RenderSize.Height / 2),
                    DragLineCanvas);
                polyline.Points.Add(center);
            }
        }

        DragLineCanvas.Children.Add(polyline);
    }

    private void ClearDragLine()
    {
        DragLineCanvas.Children.Clear();
    }

    private void AnimateBoardEntrance()
    {
        int delay = 0;
        foreach (Border tile in FindVisualChildren<Border>(BoardGrid).Where(t => t.DataContext is TileViewModel))
        {
            tile.RenderTransformOrigin = new Point(0.5, 0.5);
            tile.RenderTransform = new ScaleTransform(0, 0);
            tile.Opacity = 0;

            var scaleX = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250))
            {
                BeginTime = TimeSpan.FromMilliseconds(delay),
                EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut },
            };

            var scaleY = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(250))
            {
                BeginTime = TimeSpan.FromMilliseconds(delay),
                EasingFunction = new BackEase { Amplitude = 0.3, EasingMode = EasingMode.EaseOut },
            };

            var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200))
            {
                BeginTime = TimeSpan.FromMilliseconds(delay),
            };

            tile.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleX);
            tile.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleY);
            tile.BeginAnimation(OpacityProperty, fade);

            delay += 40;
        }
    }

    private void AnimateFeedbackPulse()
    {
        var scaleUp = new DoubleAnimation(1.0, 1.2, TimeSpan.FromMilliseconds(100))
        {
            AutoReverse = true,
        };

        FeedbackText.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleUp);
        FeedbackText.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleUp.Clone());
    }

    private void OnToastsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add)
        {
            return;
        }

        // Wait for the ItemsControl to render the new items, then animate them
        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, AnimateToasts);
    }

    private void AnimateToasts()
    {
        int delay = 0;
        foreach (Border toast in FindVisualChildren<Border>(ToastContainer).Where(b => b.RenderTransform is TranslateTransform))
        {
            if (toast.Opacity > 0)
            {
                continue;
            }

            if (toast.RenderTransform.IsFrozen)
            {
                toast.RenderTransform = new TranslateTransform(300, 0);
            }

            int capturedDelay = delay;

            var slideIn = new DoubleAnimation(300, 0, TimeSpan.FromMilliseconds(400))
            {
                BeginTime = TimeSpan.FromMilliseconds(delay),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
            };

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300))
            {
                BeginTime = TimeSpan.FromMilliseconds(delay),
            };

            toast.BeginAnimation(OpacityProperty, fadeIn);
            toast.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideIn);

            ScheduleToastDismissal(toast, capturedDelay);

            delay += 300;
        }
    }

    private void ScheduleToastDismissal(Border toast, int entranceDelayMs)
    {
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(entranceDelayMs + 5000),
        };

        timer.Tick += (_, _) =>
        {
            timer.Stop();
            FadeOutAndRemoveToast(toast);
        };

        timer.Start();
    }

    private void FadeOutAndRemoveToast(Border toast)
    {
        if (toast.RenderTransform.IsFrozen)
        {
            toast.RenderTransform = new TranslateTransform(0, 0);
        }

        var slideOut = new DoubleAnimation(0, 300, TimeSpan.FromMilliseconds(400))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn },
        };

        var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(400));

        fadeOut.Completed += (_, _) =>
        {
            if (DataContext is GameViewModel vm)
            {
                // Find the Achievement bound to this toast and remove it
                object? item = toast.DataContext;
                if (item is Achievement achievement)
                {
                    vm.AchievementToasts.Remove(achievement);
                }
            }
        };

        toast.BeginAnimation(OpacityProperty, fadeOut);
        toast.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);
    }
}
