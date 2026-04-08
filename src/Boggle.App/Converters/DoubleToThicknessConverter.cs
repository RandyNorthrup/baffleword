// <copyright file="DoubleToThicknessConverter.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Converters;

using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// Converts a double value to a uniform Thickness.
/// </summary>
[ValueConversion(typeof(double), typeof(Thickness))]
public sealed class DoubleToThicknessConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return new Thickness(d);
        }

        return new Thickness(0);
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
