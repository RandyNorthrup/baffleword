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
        Scores = [];
        _ = LoadScoresAsync();
    }

    /// <summary>
    /// Gets the high score entries.
    /// </summary>
    public ObservableCollection<HighScoreEntry> Scores { get; }

    /// <summary>
    /// Gets the command to go back to the main menu.
    /// </summary>
    public ICommand BackCommand { get; }

    private async Task LoadScoresAsync()
    {
        IReadOnlyList<HighScoreEntry> scores = await _highScoreService.GetTopScoresAsync(TimeSpan.FromMinutes(3), 50).ConfigureAwait(true);
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
