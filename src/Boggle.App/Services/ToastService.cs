// <copyright file="ToastService.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Services;

using System.Collections.ObjectModel;
using Boggle.Core.Models;

/// <summary>
/// Manages app-wide achievement toast notifications.
/// </summary>
public sealed class ToastService : IToastService
{
    /// <inheritdoc/>
    public ObservableCollection<Achievement> Toasts { get; } = [];

    /// <inheritdoc/>
    public void Show(Achievement achievement)
    {
        ArgumentNullException.ThrowIfNull(achievement);
        Toasts.Add(achievement);
    }
}
