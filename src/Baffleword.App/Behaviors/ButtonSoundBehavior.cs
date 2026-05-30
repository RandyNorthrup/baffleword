// <copyright file="ButtonSoundBehavior.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.Behaviors;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Baffleword.Audio;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Attached behavior that plays hover and click sounds on buttons.
/// </summary>
public static class ButtonSoundBehavior
{
    /// <summary>
    /// Identifies the EnableSounds attached property.
    /// </summary>
    public static readonly AttachedProperty<bool> EnableSoundsProperty =
        AvaloniaProperty.RegisterAttached<Button, bool>("EnableSounds", typeof(ButtonSoundBehavior));

    static ButtonSoundBehavior()
    {
        EnableSoundsProperty.Changed.AddClassHandler<Button>(OnEnableSoundsChanged);
    }

    /// <summary>
    /// Gets the EnableSounds value for the given button.
    /// </summary>
    /// <param name="button">The target button.</param>
    /// <returns>Whether sounds are enabled.</returns>
    public static bool GetEnableSounds(Button button)
    {
        ArgumentNullException.ThrowIfNull(button);
        return button.GetValue(EnableSoundsProperty);
    }

    /// <summary>
    /// Sets the EnableSounds value for the given button.
    /// </summary>
    /// <param name="button">The target button.</param>
    /// <param name="value">Whether to enable sounds.</param>
    public static void SetEnableSounds(Button button, bool value)
    {
        ArgumentNullException.ThrowIfNull(button);
        button.SetValue(EnableSoundsProperty, value);
    }

    private static void OnEnableSoundsChanged(Button button, AvaloniaPropertyChangedEventArgs args)
    {
        bool newValue = args.GetNewValue<bool>();

        if (newValue)
        {
            button.PointerEntered += OnPointerEntered;
            button.Click += OnClick;
        }
        else
        {
            button.PointerEntered -= OnPointerEntered;
            button.Click -= OnClick;
        }
    }

    private static void OnPointerEntered(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        if (sender is Button { IsEnabled: true })
        {
            GetAudioManager()?.SoundEffects.Play(SoundEffect.ButtonHover);
        }
    }

    private static void OnClick(object? sender, RoutedEventArgs e)
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
