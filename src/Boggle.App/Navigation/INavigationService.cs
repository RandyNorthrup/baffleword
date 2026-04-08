// <copyright file="INavigationService.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.Navigation;

using Boggle.App.ViewModels;

/// <summary>
/// Provides view navigation capabilities.
/// </summary>
public interface INavigationService : IDisposable
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

    /// <summary>
    /// Navigates to the specified ViewModel type, preserving the current VM for back-navigation.
    /// </summary>
    /// <typeparam name="TViewModel">The ViewModel type to navigate to.</typeparam>
    void NavigateToPreservingCurrent<TViewModel>()
        where TViewModel : ViewModelBase;

    /// <summary>
    /// Navigates back to the previously preserved ViewModel, if any.
    /// </summary>
    /// <returns>True if back-navigation occurred; false if there was no previous VM.</returns>
    bool GoBack();
}
