// <copyright file="RoundResultsView.xaml.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Views;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

/// <summary>
/// Round results view showing score breakdown and missed words.
/// </summary>
public partial class RoundResultsView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoundResultsView"/> class.
    /// </summary>
    public RoundResultsView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AnimateScoreCard();
    }

    private void AnimateScoreCard()
    {
        var ease = new ElasticEase { Oscillations = 1, Springiness = 5, EasingMode = EasingMode.EaseOut };

        var bounceX = new DoubleAnimation(0.5, 1.0, TimeSpan.FromMilliseconds(600))
        {
            BeginTime = TimeSpan.FromMilliseconds(150),
            EasingFunction = ease,
        };

        var bounceY = new DoubleAnimation(0.5, 1.0, TimeSpan.FromMilliseconds(600))
        {
            BeginTime = TimeSpan.FromMilliseconds(150),
            EasingFunction = ease,
        };

        var transform = new ScaleTransform(1, 1);
        ScoreCard.RenderTransform = transform;
        transform.BeginAnimation(ScaleTransform.ScaleXProperty, bounceX);
        transform.BeginAnimation(ScaleTransform.ScaleYProperty, bounceY);
    }
}
