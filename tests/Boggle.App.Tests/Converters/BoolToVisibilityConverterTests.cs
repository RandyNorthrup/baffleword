// <copyright file="BoolToVisibilityConverterTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Tests.Converters;

using System.Globalization;
using System.Windows;
using Boggle.App.Converters;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the <see cref="BoolToVisibilityConverter"/> class.
/// </summary>
public sealed class BoolToVisibilityConverterTests
{
    private readonly BoolToVisibilityConverter _sut = new();

    [Fact]
    public void Convert_True_ReturnsVisible()
    {
        object result = _sut.Convert(true, typeof(Visibility), null!, CultureInfo.InvariantCulture);

        result.Should().Be(Visibility.Visible);
    }

    [Fact]
    public void Convert_False_ReturnsCollapsed()
    {
        object result = _sut.Convert(false, typeof(Visibility), null!, CultureInfo.InvariantCulture);

        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Convert_Null_ReturnsCollapsed()
    {
        object result = _sut.Convert(null!, typeof(Visibility), null!, CultureInfo.InvariantCulture);

        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void ConvertBack_Visible_ReturnsTrue()
    {
        object result = _sut.ConvertBack(Visibility.Visible, typeof(bool), null!, CultureInfo.InvariantCulture);

        result.Should().Be(true);
    }

    [Fact]
    public void ConvertBack_Collapsed_ReturnsFalse()
    {
        object result = _sut.ConvertBack(Visibility.Collapsed, typeof(bool), null!, CultureInfo.InvariantCulture);

        result.Should().Be(false);
    }
}
