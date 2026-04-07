// <copyright file="MainMenuViewModel.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.ViewModels;

using System.Windows.Input;
using Boggle.App.Navigation;

/// <summary>
/// ViewModel for the main menu view.
/// </summary>
public sealed class MainMenuViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainMenuViewModel"/> class.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    public MainMenuViewModel(INavigationService navigation)
    {
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

        NewGameCommand = new RelayCommand(OnNewGame);
        HighScoresCommand = new RelayCommand(OnHighScores);
        AchievementsCommand = new RelayCommand(OnAchievements);
        HowToPlayCommand = new RelayCommand(OnHowToPlay);
        SettingsCommand = new RelayCommand(OnSettings);
    }

    /// <summary>
    /// Gets the command to start a new game.
    /// </summary>
    public ICommand NewGameCommand { get; }

    /// <summary>
    /// Gets the command to view high scores.
    /// </summary>
    public ICommand HighScoresCommand { get; }

    /// <summary>
    /// Gets the command to view achievements.
    /// </summary>
    public ICommand AchievementsCommand { get; }

    /// <summary>
    /// Gets the command to view How to Play.
    /// </summary>
    public ICommand HowToPlayCommand { get; }

    /// <summary>
    /// Gets the command to open settings.
    /// </summary>
    public ICommand SettingsCommand { get; }

    private void OnNewGame()
    {
        _navigation.NavigateTo<GameViewModel>();
    }

    private void OnHighScores()
    {
        _navigation.NavigateTo<HighScoresViewModel>();
    }

    private void OnAchievements()
    {
        _navigation.NavigateTo<AchievementsViewModel>();
    }

    private void OnHowToPlay()
    {
        _navigation.NavigateTo<HowToPlayViewModel>();
    }

    private void OnSettings()
    {
        _navigation.NavigateTo<SettingsViewModel>();
    }
}
