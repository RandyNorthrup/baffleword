// <copyright file="BoolToVisibilityConverter.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Converters;

using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// Converts a boolean value to a <see cref="Visibility"/> value.
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public sealed class BoolToVisibilityConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility visibility && visibility == Visibility.Visible;
    }
}
