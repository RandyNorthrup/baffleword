// <copyright file="TimeSpanToStringConverter.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Converters;

using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Converts a <see cref="TimeSpan"/> to a formatted string (M:SS).
/// </summary>
[ValueConversion(typeof(TimeSpan), typeof(string))]
public sealed class TimeSpanToStringConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            return $"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:D2}";
        }

        return "0:00";
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("TimeSpanToStringConverter does not support ConvertBack.");
    }
}
