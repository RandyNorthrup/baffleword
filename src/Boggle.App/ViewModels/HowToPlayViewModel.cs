// <copyright file="HowToPlayViewModel.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.ViewModels;

using System.Windows.Input;
using Boggle.App.Navigation;

/// <summary>
/// ViewModel for the How to Play view.
/// </summary>
public sealed class HowToPlayViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;

    /// <summary>
    /// Initializes a new instance of the <see cref="HowToPlayViewModel"/> class.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    public HowToPlayViewModel(INavigationService navigation)
    {
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        BackCommand = new RelayCommand(OnBack);
    }

    /// <summary>
    /// Gets the command to go back.
    /// </summary>
    public ICommand BackCommand { get; }

    private void OnBack()
    {
        _navigation.NavigateTo<MainMenuViewModel>();
    }
}
