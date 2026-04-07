// <copyright file="INavigationService.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.Navigation;

using Boggle.App.ViewModels;

/// <summary>
/// Provides view navigation capabilities.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Occurs when navigation changes the current view.
    /// </summary>
    event EventHandler? NavigationChanged;

    /// <summary>
    /// Gets the current ViewModel.
    /// </summary>
    ViewModelBase? CurrentViewModel { get; }

    /// <summary>
    /// Navigates to the specified ViewModel type.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to navigate to.</typeparam>
    void NavigateTo<TViewModel>()
        where TViewModel : ViewModelBase;
}
