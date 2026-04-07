// <copyright file="RoundResultsView.xaml.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
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

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AnimateScoreCard();
        AnimateAchievements();
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

        ScoreCard.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, bounceX);
        ScoreCard.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, bounceY);
    }

    private void AnimateAchievements()
    {
        int delay = 800;
        foreach (Border badge in FindVisualChildren<Border>(AchievementsList).Where(b => b.RenderTransform is TranslateTransform))
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300))
            {
                BeginTime = TimeSpan.FromMilliseconds(delay),
            };

            var slideIn = new DoubleAnimation(-20, 0, TimeSpan.FromMilliseconds(300))
            {
                BeginTime = TimeSpan.FromMilliseconds(delay),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut },
            };

            badge.BeginAnimation(OpacityProperty, fadeIn);
            badge.RenderTransform.BeginAnimation(TranslateTransform.YProperty, slideIn);

            delay += 200;
        }
    }
}
