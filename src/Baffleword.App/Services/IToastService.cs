// <copyright file="IToastService.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.Services;

using System.Collections.ObjectModel;
using Baffleword.Core.Models;

/// <summary>
/// Provides app-wide achievement toast notifications.
/// </summary>
public interface IToastService
{
    /// <summary>
    /// Gets the collection of active achievement toasts.
    /// </summary>
    ObservableCollection<Achievement> Toasts { get; }

    /// <summary>
    /// Shows an achievement toast notification.
    /// </summary>
    /// <param name="achievement">The achievement to display.</param>
    void Show(Achievement achievement);
}
