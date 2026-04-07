// <copyright file="ScoreToColorConverter.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.Converters;

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

/// <summary>
/// Converts a score value to a color (higher scores get warmer colors).
/// </summary>
[ValueConversion(typeof(int), typeof(Brush))]
public sealed class ScoreToColorConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int score)
        {
            return score switch
            {
                >= 11 => new SolidColorBrush(Color.FromRgb(0xF0, 0xC0, 0x40)), // Gold
                >= 5 => new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x82)),  // Green
                >= 2 => new SolidColorBrush(Color.FromRgb(0x4A, 0x9B, 0xD9)),  // Blue
                _ => new SolidColorBrush(Color.FromRgb(0x7A, 0x84, 0x90)),      // Gray
            };
        }

        return new SolidColorBrush(Color.FromRgb(0x7A, 0x84, 0x90));
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("ScoreToColorConverter does not support ConvertBack.");
    }
}
