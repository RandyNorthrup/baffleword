// <copyright file="MainWindow.xaml.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App;

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Boggle.App.Extensions;
using Boggle.App.Navigation;
using Boggle.App.Services;
using Boggle.Core.Models;

/// <summary>
/// Main application window that hosts the navigation frame.
/// </summary>
public partial class MainWindow : Window
{
    private readonly INavigationService _navigation;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="toastService">The toast notification service.</param>
    public MainWindow(INavigationService navigation, IToastService toastService)
    {
        InitializeComponent();

        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        ArgumentNullException.ThrowIfNull(toastService);

        _navigation.NavigationChanged += OnNavigationChanged;
        toastService.Toasts.CollectionChanged += OnToastsChanged;

        ToastContainer.ItemsSource = toastService.Toasts;
        DataContext = this;
    }

    private void OnNavigationChanged(object? sender, EventArgs e)
    {
        var fadeOut = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(120));
        fadeOut.Completed += (_, _) =>
        {
            ContentArea.Content = _navigation.CurrentViewModel;
            var fadeIn = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(200));
            ContentArea.BeginAnimation(OpacityProperty, fadeIn);
        };

        ContentArea.BeginAnimation(OpacityProperty, fadeOut);
    }

    private void OnToastsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add)
        {
            return;
        }

        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, AnimateToasts);
    }

    private void AnimateToasts()
    {
        int delay = 0;
        foreach (Border toast in ToastContainer.FindVisualChildren<Border>().Where(b => b.RenderTransform is TranslateTransform))
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
            if (ToastContainer.ItemsSource is IList<Achievement> toasts &&
                toast.DataContext is Achievement achievement)
            {
                toasts.Remove(achievement);
            }
        };

        toast.BeginAnimation(OpacityProperty, fadeOut);
        toast.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideOut);
    }
}
