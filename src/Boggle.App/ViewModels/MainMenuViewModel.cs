// <copyright file="MainMenuViewModel.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.ViewModels;

using System.Windows.Input;
using Boggle.App.Navigation;
using Boggle.Core.Models;
using Boggle.Core.Repositories;

/// <summary>
/// ViewModel for the main menu view.
/// </summary>
public sealed class MainMenuViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;
    private readonly ISettingsRepository _settingsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainMenuViewModel"/> class.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="settingsRepository">The settings repository.</param>
    public MainMenuViewModel(INavigationService navigation, ISettingsRepository settingsRepository)
    {
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));

        PlayStandardCommand = new RelayCommand(() => _ = LaunchGameAsync(GameMode.Standard));
        PlayBigBoggleCommand = new RelayCommand(() => _ = LaunchGameAsync(GameMode.BigBoggle));
        PlaySuperBigBoggleCommand = new RelayCommand(() => _ = LaunchGameAsync(GameMode.SuperBigBoggle));
        HighScoresCommand = new RelayCommand(OnHighScores);
        AchievementsCommand = new RelayCommand(OnAchievements);
        HowToPlayCommand = new RelayCommand(OnHowToPlay);
        SettingsCommand = new RelayCommand(OnSettings);
    }

    /// <summary>
    /// Gets the command to play Standard Boggle.
    /// </summary>
    public ICommand PlayStandardCommand { get; }

    /// <summary>
    /// Gets the command to play Big Boggle.
    /// </summary>
    public ICommand PlayBigBoggleCommand { get; }

    /// <summary>
    /// Gets the command to play Super Big Boggle.
    /// </summary>
    public ICommand PlaySuperBigBoggleCommand { get; }

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

    private async Task LaunchGameAsync(GameMode mode)
    {
        await _settingsRepository.SetAsync("GameMode", mode.ToString()).ConfigureAwait(true);
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
