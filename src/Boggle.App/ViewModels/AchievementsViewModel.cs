// <copyright file="AchievementsViewModel.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using Boggle.App.Navigation;
using Boggle.Core.Models;
using Boggle.Core.Services;

/// <summary>
/// ViewModel for the achievements view.
/// </summary>
public sealed class AchievementsViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;

    /// <summary>
    /// Initializes a new instance of the <see cref="AchievementsViewModel"/> class.
    /// </summary>
    /// <param name="achievementService">The achievement service.</param>
    /// <param name="navigation">The navigation service.</param>
    public AchievementsViewModel(IAchievementService achievementService, INavigationService navigation)
    {
        ArgumentNullException.ThrowIfNull(achievementService);
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

        BackCommand = new RelayCommand(OnBack);
        Achievements = new ObservableCollection<Achievement>(achievementService.GetAllAchievements());
    }

    /// <summary>
    /// Gets the list of all achievements.
    /// </summary>
    public ObservableCollection<Achievement> Achievements { get; }

    /// <summary>
    /// Gets the command to go back to the main menu.
    /// </summary>
    public ICommand BackCommand { get; }

    private void OnBack()
    {
        _navigation.NavigateTo<MainMenuViewModel>();
    }
}
