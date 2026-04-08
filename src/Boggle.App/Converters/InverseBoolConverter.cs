// <copyright file="InverseBoolConverter.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Converters;

using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Converts a boolean value to its inverse.
/// </summary>
[ValueConversion(typeof(bool), typeof(bool))]
public sealed class InverseBoolConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? !b : value;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b ? !b : value;
    }
}
