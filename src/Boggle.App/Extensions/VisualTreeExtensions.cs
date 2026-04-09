// <copyright file="VisualTreeExtensions.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Extensions;

using System.Windows;
using System.Windows.Media;

/// <summary>
/// Extension methods for WPF visual tree traversal.
/// </summary>
internal static class VisualTreeExtensions
{
    /// <summary>
    /// Finds all visual children of the specified type within a dependency object.
    /// </summary>
    /// <typeparam name="T">The type of children to find.</typeparam>
    /// <param name="parent">The parent dependency object.</param>
    /// <returns>An enumerable of matching children.</returns>
    public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject parent)
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

            foreach (T grandchild in child.FindVisualChildren<T>())
            {
                yield return grandchild;
            }
        }
    }
}
