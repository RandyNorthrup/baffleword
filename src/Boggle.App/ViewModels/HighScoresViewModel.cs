// <copyright file="HighScoresViewModel.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using Boggle.App.Navigation;
using Boggle.Core.Models;
using Boggle.Core.Services;

/// <summary>
/// ViewModel for the high scores view.
/// </summary>
public sealed class HighScoresViewModel : ViewModelBase
{
    private readonly IHighScoreService _highScoreService;
    private readonly INavigationService _navigation;
    private GameMode _selectedMode = GameMode.Standard;

    /// <summary>
    /// Initializes a new instance of the <see cref="HighScoresViewModel"/> class.
    /// </summary>
    /// <param name="highScoreService">The high score service.</param>
    /// <param name="navigation">The navigation service.</param>
    public HighScoresViewModel(IHighScoreService highScoreService, INavigationService navigation)
    {
        _highScoreService = highScoreService ?? throw new ArgumentNullException(nameof(highScoreService));
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

        BackCommand = new RelayCommand(OnBack);
        ShowStandardCommand = new RelayCommand(() => _ = SelectModeAsync(GameMode.Standard));
        ShowBigBoggleCommand = new RelayCommand(() => _ = SelectModeAsync(GameMode.BigBoggle));
        ShowSuperBigBoggleCommand = new RelayCommand(() => _ = SelectModeAsync(GameMode.SuperBigBoggle));
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
                OnPropertyChanged(nameof(IsBigBoggleSelected));
                OnPropertyChanged(nameof(IsSuperBigBoggleSelected));
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether Standard mode is selected.
    /// </summary>
    public bool IsStandardSelected => _selectedMode == GameMode.Standard;

    /// <summary>
    /// Gets a value indicating whether Big Boggle mode is selected.
    /// </summary>
    public bool IsBigBoggleSelected => _selectedMode == GameMode.BigBoggle;

    /// <summary>
    /// Gets a value indicating whether Super Big Boggle mode is selected.
    /// </summary>
    public bool IsSuperBigBoggleSelected => _selectedMode == GameMode.SuperBigBoggle;

    /// <summary>
    /// Gets the command to go back to the main menu.
    /// </summary>
    public ICommand BackCommand { get; }

    /// <summary>
    /// Gets the command to show Standard scores.
    /// </summary>
    public ICommand ShowStandardCommand { get; }

    /// <summary>
    /// Gets the command to show Big Boggle scores.
    /// </summary>
    public ICommand ShowBigBoggleCommand { get; }

    /// <summary>
    /// Gets the command to show Super Big Boggle scores.
    /// </summary>
    public ICommand ShowSuperBigBoggleCommand { get; }

    private async Task SelectModeAsync(GameMode mode)
    {
        SelectedMode = mode;
        await LoadScoresAsync().ConfigureAwait(true);
    }

    private async Task LoadScoresAsync()
    {
        IReadOnlyList<HighScoreEntry> scores = await _highScoreService.GetTopScoresAsync(_selectedMode, 50).ConfigureAwait(true);
        Scores.Clear();
        foreach (HighScoreEntry entry in scores)
        {
            Scores.Add(entry);
        }
    }

    private void OnBack()
    {
        _navigation.NavigateTo<MainMenuViewModel>();
    }
}
