// <copyright file="HighScoresViewModel.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using Baffleword.App.Navigation;
using Baffleword.Core.Models;
using Baffleword.Core.Services;
using Microsoft.Extensions.Logging;

/// <summary>
/// ViewModel for the high scores view.
/// </summary>
public sealed class HighScoresViewModel : ViewModelBase
{
    private readonly IHighScoreService _highScoreService;
    private readonly INavigationService _navigation;
    private readonly ILogger<HighScoresViewModel> _logger;
    private GameMode _selectedMode = GameMode.Standard;

    /// <summary>
    /// Initializes a new instance of the <see cref="HighScoresViewModel"/> class.
    /// </summary>
    /// <param name="highScoreService">The high score service.</param>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="logger">The logger instance.</param>
    public HighScoresViewModel(IHighScoreService highScoreService, INavigationService navigation, ILogger<HighScoresViewModel> logger)
    {
        _highScoreService = highScoreService ?? throw new ArgumentNullException(nameof(highScoreService));
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        BackCommand = new RelayCommand(OnBack);
        ShowStandardCommand = new RelayCommand(() => _ = SelectModeAsync(GameMode.Standard));
        ShowBigBoardCommand = new RelayCommand(() => _ = SelectModeAsync(GameMode.BigBoard));
        ShowSuperBoardCommand = new RelayCommand(() => _ = SelectModeAsync(GameMode.SuperBoard));
        Scores = [];
        _ = LoadScoresAsync();
    }

    /// <summary>
    /// Gets the high score entries.
    /// </summary>
    public ObservableCollection<HighScoreEntry> Scores { get; }

    /// <summary>
    /// Gets the currently selected game mode for filtering.
    /// </summary>
    public GameMode SelectedMode
    {
        get => _selectedMode;
        private set
        {
            if (SetProperty(ref _selectedMode, value))
            {
                OnPropertyChanged(nameof(IsStandardSelected));
                OnPropertyChanged(nameof(IsBigBoardSelected));
                OnPropertyChanged(nameof(IsSuperBoardSelected));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether Standard mode is selected.
    /// </summary>
    public bool IsStandardSelected => _selectedMode == GameMode.Standard;

    /// <summary>
    /// Gets a value indicating whether Big Board mode is selected.
    /// </summary>
    public bool IsBigBoardSelected => _selectedMode == GameMode.BigBoard;

    /// <summary>
    /// Gets a value indicating whether Super Board mode is selected.
    /// </summary>
    public bool IsSuperBoardSelected => _selectedMode == GameMode.SuperBoard;

    /// <summary>
    /// Gets the command to go back to the main menu.
    /// </summary>
    public ICommand BackCommand { get; }

    /// <summary>
    /// Gets the command to show Standard scores.
    /// </summary>
    public ICommand ShowStandardCommand { get; }

    /// <summary>
    /// Gets the command to show Big Board scores.
    /// </summary>
    public ICommand ShowBigBoardCommand { get; }

    /// <summary>
    /// Gets the command to show Super Board scores.
    /// </summary>
    public ICommand ShowSuperBoardCommand { get; }

    private async Task SelectModeAsync(GameMode mode)
    {
        try
        {
            SelectedMode = mode;
            await LoadScoresAsync().ConfigureAwait(true);
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            _logger.LogError(ex, "Error selecting mode {Mode}", mode);
        }
    }

    private async Task LoadScoresAsync()
    {
        try
        {
            IReadOnlyList<HighScoreEntry> scores = await _highScoreService.GetTopScoresAsync(_selectedMode, 50).ConfigureAwait(true);
            Scores.Clear();
            foreach (HighScoreEntry entry in scores)
            {
                Scores.Add(entry);
            }
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            _logger.LogError(ex, "Error loading scores");
        }
    }

    private void OnBack()
    {
        _navigation.NavigateTo<MainMenuViewModel>();
    }
}
