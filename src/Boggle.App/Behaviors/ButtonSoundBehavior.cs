// <copyright file="ButtonSoundBehavior.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Behaviors;

using System.Windows;
using System.Windows.Controls;
using Boggle.Audio;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Attached behavior that plays hover and click sounds on buttons.
/// </summary>
public static class ButtonSoundBehavior
{
    /// <summary>
    /// Identifies the EnableSounds attached property.
    /// </summary>
    public static readonly DependencyProperty EnableSoundsProperty =
        DependencyProperty.RegisterAttached(
            "EnableSounds",
            typeof(bool),
            typeof(ButtonSoundBehavior),
            new PropertyMetadata(false, OnEnableSoundsChanged));

    /// <summary>
    /// Gets the EnableSounds value for the given element.
    /// </summary>
    /// <param name="obj">The target element.</param>
    /// <returns>Whether sounds are enabled.</returns>
    public static bool GetEnableSounds(DependencyObject obj) =>
        (bool)obj.GetValue(EnableSoundsProperty);

    /// <summary>
    /// Sets the EnableSounds value for the given element.
    /// </summary>
    /// <param name="obj">The target element.</param>
    /// <param name="value">Whether to enable sounds.</param>
    public static void SetEnableSounds(DependencyObject obj, bool value) =>
        obj.SetValue(EnableSoundsProperty, value);

    private static void OnEnableSoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Button button)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            button.MouseEnter += OnMouseEnter;
            button.Click += OnClick;
        }
        else
        {
            button.MouseEnter -= OnMouseEnter;
            button.Click -= OnClick;
        }
    }

    private static void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (sender is Button button && button.IsEnabled)
        {
            GetAudioManager()?.SoundEffects.Play(SoundEffect.ButtonHover);
        }
    }

    private static void OnClick(object sender, RoutedEventArgs e)
    {
        GetAudioManager()?.SoundEffects.Play(SoundEffect.ButtonClick);
    }

    private static IAudioManager? GetAudioManager()
    {
        return (Application.Current as App)?
            .Services?
            .GetService<IAudioManager>();
    }
}
