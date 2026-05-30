// <copyright file="MainWindow.axaml.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App;

using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Threading;
using Baffleword.App.Navigation;
using Baffleword.App.Services;
using Baffleword.Core.Models;

/// <summary>
/// Main application window that hosts the navigation frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly INavigationService? _navigation;
    private readonly IToastService? _toastService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="toastService">The toast notification service.</param>
    public MainWindow(INavigationService navigation, IToastService toastService)
        : this()
    {
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        _toastService = toastService ?? throw new ArgumentNullException(nameof(toastService));

        _navigation.NavigationChanged += OnNavigationChanged;
        _toastService.Toasts.CollectionChanged += OnToastsChanged;
        ToastContainer.ItemsSource = _toastService.Toasts;
    }

    private void OnNavigationChanged(object? sender, EventArgs e)
    {
        ContentArea.Content = _navigation?.CurrentViewModel;
    }

    private void OnToastsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add || e.NewItems is null)
        {
            return;
        }

        foreach (Achievement achievement in e.NewItems.OfType<Achievement>())
        {
            DispatcherTimer.RunOnce(() => _toastService?.Toasts.Remove(achievement), TimeSpan.FromSeconds(5));
        }
    }
}
