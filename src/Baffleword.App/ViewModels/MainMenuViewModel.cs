// <copyright file="MainMenuViewModel.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.ViewModels;

using System.Windows.Input;
using Baffleword.App.Navigation;
using Baffleword.Core.Models;
using Baffleword.Core.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// ViewModel for the main menu view.
/// </summary>
public sealed class MainMenuViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;
    private readonly ISettingsRepository _settingsRepository;
    private readonly ILogger<MainMenuViewModel> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainMenuViewModel"/> class.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="settingsRepository">The settings repository.</param>
    /// <param name="logger">The logger instance.</param>
    public MainMenuViewModel(INavigationService navigation, ISettingsRepository settingsRepository, ILogger<MainMenuViewModel> logger)
    {
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        PlayStandardCommand = new RelayCommand(() => _ = LaunchGameAsync(GameMode.Standard));
        PlayBigBoardCommand = new RelayCommand(() => _ = LaunchGameAsync(GameMode.BigBoard));
        PlaySuperBoardCommand = new RelayCommand(() => _ = LaunchGameAsync(GameMode.SuperBoard));
        HighScoresCommand = new RelayCommand(OnHighScores);
        AchievementsCommand = new RelayCommand(OnAchievements);
        HowToPlayCommand = new RelayCommand(OnHowToPlay);
        SettingsCommand = new RelayCommand(OnSettings);
    }

    /// <summary>
    /// Gets the command to play Standard.
    /// </summary>
    public ICommand PlayStandardCommand { get; }

    /// <summary>
    /// Gets the command to play Big Board.
    /// </summary>
    public ICommand PlayBigBoardCommand { get; }

    /// <summary>
    /// Gets the command to play Super Board.
    /// </summary>
    public ICommand PlaySuperBoardCommand { get; }

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
        try
        {
            await _settingsRepository.SetAsync("GameMode", mode.ToString()).ConfigureAwait(true);
            _navigation.NavigateTo<GameViewModel>();
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            _logger.LogError(ex, "Error launching {Mode} game", mode);
        }
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
