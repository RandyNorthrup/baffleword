// <copyright file="RoundResultsViewModel.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using Boggle.App.Navigation;
using Boggle.Core.Models;

/// <summary>
/// ViewModel for the round results view.
/// </summary>
public sealed class RoundResultsViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;
    private int _score;
    private int _wordsFound;
    private double _completionPercentage;
    private string _longestWord = string.Empty;
    private int _totalPossibleWords;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoundResultsViewModel"/> class.
    /// </summary>
    /// <param name="navigation">The navigation service.</param>
    public RoundResultsViewModel(INavigationService navigation)
    {
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));

        NewRoundCommand = new RelayCommand(OnNewRound);
        MainMenuCommand = new RelayCommand(OnMainMenu);
        FoundWords = [];
        MissedWords = [];
    }

    /// <summary>
    /// Gets the final score.
    /// </summary>
    public int Score
    {
        get => _score;
        private set => SetProperty(ref _score, value);
    }

    /// <summary>
    /// Gets the number of words found.
    /// </summary>
    public int WordsFound
    {
        get => _wordsFound;
        private set => SetProperty(ref _wordsFound, value);
    }

    /// <summary>
    /// Gets the completion percentage.
    /// </summary>
    public double CompletionPercentage
    {
        get => _completionPercentage;
        private set => SetProperty(ref _completionPercentage, value);
    }

    /// <summary>
    /// Gets the longest word found.
    /// </summary>
    public string LongestWord
    {
        get => _longestWord;
        private set => SetProperty(ref _longestWord, value);
    }

    /// <summary>
    /// Gets the total possible words on the board.
    /// </summary>
    public int TotalPossibleWords
    {
        get => _totalPossibleWords;
        private set => SetProperty(ref _totalPossibleWords, value);
    }

    /// <summary>
    /// Gets the words the player found.
    /// </summary>
    public ObservableCollection<WordResult> FoundWords { get; }

    /// <summary>
    /// Gets the words the player missed.
    /// </summary>
    public ObservableCollection<string> MissedWords { get; }

    /// <summary>
    /// Gets the achievements unlocked this round.
    /// </summary>
    public ObservableCollection<Achievement> UnlockedAchievements { get; } = [];

    /// <summary>
    /// Gets the command to start a new round.
    /// </summary>
    public ICommand NewRoundCommand { get; }

    /// <summary>
    /// Gets the command to return to the main menu.
    /// </summary>
    public ICommand MainMenuCommand { get; }

    /// <summary>
    /// Loads the round results into the view model.
    /// </summary>
    /// <param name="round">The completed game round.</param>
    /// <param name="unlockedAchievements">Achievements unlocked this round.</param>
    public void LoadResults(GameRound round, IReadOnlyList<Achievement>? unlockedAchievements = null)
    {
        ArgumentNullException.ThrowIfNull(round);

        Score = round.Score;
        WordsFound = round.ValidWordCount;
        CompletionPercentage = round.CompletionPercentage;
        TotalPossibleWords = round.TotalPossibleWords;

        var validWords = round.SubmittedWords.Where(w => w.Status == WordStatus.Valid).ToList();
        LongestWord = validWords.Count > 0
            ? validWords.OrderByDescending(w => w.Word.Length).First().Word
            : string.Empty;

        FoundWords.Clear();
        foreach (WordResult word in round.SubmittedWords)
        {
            FoundWords.Add(word);
        }

        MissedWords.Clear();
        HashSet<string> foundWordSet = new(validWords.Select(w => w.Word), StringComparer.OrdinalIgnoreCase);
        if (round.AllPossibleWords != null)
        {
            foreach (string word in round.AllPossibleWords.Where(w => !foundWordSet.Contains(w)))
            {
                MissedWords.Add(word);
            }
        }

        UnlockedAchievements.Clear();
        if (unlockedAchievements != null)
        {
            foreach (Achievement a in unlockedAchievements)
            {
                UnlockedAchievements.Add(a);
            }
        }
    }

    private void OnNewRound()
    {
        _navigation.NavigateTo<GameViewModel>();
    }

    private void OnMainMenu()
    {
        _navigation.NavigateTo<MainMenuViewModel>();
    }
}
