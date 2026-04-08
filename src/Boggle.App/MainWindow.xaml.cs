// <copyright file="MainWindow.xaml.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App;

using System.Windows;
using System.Windows.Media.Animation;
using Boggle.App.Navigation;

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
    public MainWindow(INavigationService navigation)
    {
        InitializeComponent();

        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        _navigation.NavigationChanged += OnNavigationChanged;

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
}
