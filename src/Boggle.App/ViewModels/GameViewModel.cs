// <copyright file="GameViewModel.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Boggle.App.Navigation;
using Boggle.Audio;
using Boggle.Core.Models;
using Boggle.Core.Repositories;
using Boggle.Core.Services;

/// <summary>
/// ViewModel for the game view â€” manages round lifecycle and word submission.
/// </summary>
public sealed class GameViewModel : ViewModelBase, IDisposable
{
    private readonly IGameEngine _gameEngine;
    private readonly INavigationService _navigation;
    private readonly IAchievementService _achievementService;
    private readonly IHighScoreService _highScoreService;
    private readonly IStatisticsService _statisticsService;
    private readonly IAudioManager _audioManager;
    private readonly ISettingsRepository _settingsRepository;
    private readonly DispatcherTimer _timer;
    private readonly List<TileViewModel> _selectedPath = [];

    private string _currentWord = string.Empty;
    private int _score;
    private TimeSpan _timeRemaining;
    private TimeSpan _timerDuration;
    private bool _isPaused;
    private string _lastSubmissionFeedback = string.Empty;
    private string[][] _boardLetters = [];
    private GameRound? _currentRound;
    private bool _isDragging;

    private bool _isTimerWarning;
    private int _boardRows = 4;
    private int _boardColumns = 4;
    private double _tileSize = 72;
    private double _tileMargin = 4;
    private double _tileFontSize = 24;
    private double _boardContainerSize = 344;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameViewModel"/> class.
    /// </summary>
    /// <param name="gameEngine">The game engine.</param>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="achievementService">The achievement service.</param>
    /// <param name="highScoreService">The high score service.</param>
    /// <param name="statisticsService">The statistics service.</param>
    /// <param name="audioManager">The audio manager.</param>
    /// <param name="settingsRepository">The settings repository.</param>
    public GameViewModel(
        IGameEngine gameEngine,
        INavigationService navigation,
        IAchievementService achievementService,
        IHighScoreService highScoreService,
        IStatisticsService statisticsService,
        IAudioManager audioManager,
        ISettingsRepository settingsRepository)
    {
        _gameEngine = gameEngine ?? throw new ArgumentNullException(nameof(gameEngine));
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        _achievementService = achievementService ?? throw new ArgumentNullException(nameof(achievementService));
        _highScoreService = highScoreService ?? throw new ArgumentNullException(nameof(highScoreService));
        _statisticsService = statisticsService ?? throw new ArgumentNullException(nameof(statisticsService));
        _audioManager = audioManager ?? throw new ArgumentNullException(nameof(audioManager));
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));

        SubmitWordCommand = new RelayCommand(OnSubmitWord, () => !string.IsNullOrWhiteSpace(CurrentWord) && !IsPaused);
        PauseCommand = new RelayCommand(OnPause, () => !IsPaused);
        ResumeCommand = new RelayCommand(OnResume, () => IsPaused);
        QuitCommand = new RelayCommand(OnQuit);

        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
        _timer.Tick += OnTimerTick;

        FoundWords = [];
        GroupedFoundWords = CollectionViewSource.GetDefaultView(FoundWords);
        GroupedFoundWords.GroupDescriptions.Add(new PropertyGroupDescription(nameof(WordResult.WordLength)));
        GroupedFoundWords.SortDescriptions.Add(new SortDescription(nameof(WordResult.WordLength), ListSortDirection.Descending));
        GroupedFoundWords.SortDescriptions.Add(new SortDescription(nameof(WordResult.Word), ListSortDirection.Ascending));
        _ = InitializeAsync();
    }

    /// <summary>
    /// Gets or sets the current word being typed.
    /// </summary>
    public string CurrentWord
    {
        get => _currentWord;
        set
        {
            if (SetProperty(ref _currentWord, value))
            {
                RelayCommand.RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets the current score.
    /// </summary>
    public int Score
    {
        get => _score;
        private set => SetProperty(ref _score, value);
    }

    /// <summary>
    /// Gets the time remaining.
    /// </summary>
    public TimeSpan TimeRemaining
    {
        get => _timeRemaining;
        private set
        {
            if (SetProperty(ref _timeRemaining, value))
            {
                IsTimerWarning = value.TotalSeconds <= 5 && value > TimeSpan.Zero;
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether the timer is in warning state (last 5 seconds).
    /// </summary>
    public bool IsTimerWarning
    {
        get => _isTimerWarning;
        private set => SetProperty(ref _isTimerWarning, value);
    }

    /// <summary>
    /// Gets the timer duration for progress calculation.
    /// </summary>
    public TimeSpan TimerDuration
    {
        get => _timerDuration;
        private set => SetProperty(ref _timerDuration, value);
    }

    /// <summary>
    /// Gets a value indicating whether the game is paused.
    /// </summary>
    public bool IsPaused
    {
        get => _isPaused;
        private set
        {
            if (SetProperty(ref _isPaused, value))
            {
                RelayCommand.RaiseCanExecuteChanged();
            }
        }
    }

    /// <summary>
    /// Gets the last submission feedback message.
    /// </summary>
    public string LastSubmissionFeedback
    {
        get => _lastSubmissionFeedback;
        private set => SetProperty(ref _lastSubmissionFeedback, value);
    }

    /// <summary>
    /// Gets the board letters for display.
    /// </summary>
    public string[][] BoardLetters
    {
        get => _boardLetters;
        private set => SetProperty(ref _boardLetters, value);
    }

    /// <summary>
    /// Gets the collection of found words.
    /// </summary>
    public ObservableCollection<WordResult> FoundWords { get; }

    /// <summary>
    /// Gets the grouped and sorted view of found words.
    /// </summary>
    public ICollectionView GroupedFoundWords { get; }

    /// <summary>
    /// Gets the flat collection of 16 tiles for the board.
    /// </summary>
    public ObservableCollection<TileViewModel> Tiles { get; } = [];

    /// <summary>
    /// Gets a value indicating whether a drag selection is in progress.
    /// </summary>
    public bool IsDragging
    {
        get => _isDragging;
        private set => SetProperty(ref _isDragging, value);
    }

    /// <summary>
    /// Gets the number of board rows.
    /// </summary>
    public int BoardRows
    {
        get => _boardRows;
        private set => SetProperty(ref _boardRows, value);
    }

    /// <summary>
    /// Gets the number of board columns.
    /// </summary>
    public int BoardColumns
    {
        get => _boardColumns;
        private set => SetProperty(ref _boardColumns, value);
    }

    /// <summary>
    /// Gets the tile size in pixels.
    /// </summary>
    public double TileSize
    {
        get => _tileSize;
        private set => SetProperty(ref _tileSize, value);
    }

    /// <summary>
    /// Gets the tile margin in pixels.
    /// </summary>
    public double TileMargin
    {
        get => _tileMargin;
        private set => SetProperty(ref _tileMargin, value);
    }

    /// <summary>
    /// Gets the tile font size.
    /// </summary>
    public double TileFontSize
    {
        get => _tileFontSize;
        private set => SetProperty(ref _tileFontSize, value);
    }

    /// <summary>
    /// Gets the board container size.
    /// </summary>
    public double BoardContainerSize
    {
        get => _boardContainerSize;
        private set => SetProperty(ref _boardContainerSize, value);
    }

    /// <summary>
    /// Gets the command to submit a word.
    /// </summary>
    public ICommand SubmitWordCommand { get; }

    /// <summary>
    /// Gets the command to pause the game.
    /// </summary>
    public ICommand PauseCommand { get; }

    /// <summary>
    /// Gets the command to resume the game.
    /// </summary>
    public ICommand ResumeCommand { get; }

    /// <summary>
    /// Gets the command to quit to menu.
    /// </summary>
    public ICommand QuitCommand { get; }

    /// <summary>
    /// Begins a drag selection starting from the given tile.
    /// </summary>
    /// <param name="tile">The tile where the drag started.</param>
    public void BeginDragSelection(TileViewModel tile)
    {
        if (IsPaused || tile is null || tile.IsBlocked)
        {
            return;
        }

        ClearSelection();
        IsDragging = true;
        AddTileToPath(tile);
    }

    /// <summary>
    /// Extends the drag selection to include the given tile if it is adjacent.
    /// </summary>
    /// <param name="tile">The tile being dragged over.</param>
    public void ExtendDragSelection(TileViewModel tile)
    {
        if (!IsDragging || tile is null || IsPaused || tile.IsBlocked)
        {
            return;
        }

        if (tile.IsSelected)
        {
            // Allow backtracking: if the tile is the second-to-last in path, pop the last one
            if (_selectedPath.Count >= 2 && _selectedPath[^2] == tile)
            {
                TileViewModel removed = _selectedPath[^1];
                removed.IsSelected = false;
                removed.SelectionOrder = 0;
                _selectedPath.RemoveAt(_selectedPath.Count - 1);
                UpdateCurrentWordFromPath();
            }

            return;
        }

        TileViewModel last = _selectedPath[^1];
        if (last.IsAdjacentTo(tile))
        {
            AddTileToPath(tile);
        }
    }

    /// <summary>
    /// Ends the drag selection and submits the selected word.
    /// </summary>
    public void EndDragSelection()
    {
        if (!IsDragging)
        {
            return;
        }

        IsDragging = false;

        if (_selectedPath.Count > 0)
        {
            string word = string.Concat(_selectedPath.Select(t => t.Letter));
            if (!string.IsNullOrWhiteSpace(word))
            {
                SubmitDraggedWord(word);
            }
        }

        ClearSelection();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _timer.Stop();
    }

    private async Task InitializeAsync()
    {
        GameMode mode = GameMode.Standard;

        string? modeSetting = await _settingsRepository.GetAsync("GameMode").ConfigureAwait(true);
        if (modeSetting != null && Enum.TryParse<GameMode>(modeSetting, out GameMode gm))
        {
            mode = gm;
        }

        GameModeConfig config = GameModeConfig.ForMode(mode);
        int timerSeconds = config.DefaultTimerSeconds;
        int minWordLength = config.MinWordLength;

        StartNewRound(TimeSpan.FromSeconds(timerSeconds), minWordLength, mode);
    }

    private void StartNewRound(TimeSpan duration, int minWordLength, GameMode mode = GameMode.Standard)
    {
        _currentRound = _gameEngine.StartRound(duration, minWordLength, mode);
        TimerDuration = duration;
        TimeRemaining = duration;
        Score = 0;
        FoundWords.Clear();

        GameBoard board = _currentRound.Board;
        BoardRows = board.Rows;
        BoardColumns = board.Columns;

        // Calculate tile sizing based on board dimensions
        UpdateBoardSizing(board.Rows);

        string[][] letters = new string[board.Rows][];
        Tiles.Clear();
        for (int row = 0; row < board.Rows; row++)
        {
            letters[row] = new string[board.Columns];
            for (int col = 0; col < board.Columns; col++)
            {
                BoardCell cell = board[row, col];
                letters[row][col] = cell.Letter;
                Tiles.Add(new TileViewModel(cell.Letter, row, col, cell.IsBlocked));
            }
        }

        BoardLetters = letters;
        _audioManager.SoundEffects.Play(SoundEffect.RoundStart);
        _timer.Start();
    }

    private void UpdateBoardSizing(int gridSize)
    {
        switch (gridSize)
        {
            case 5:
                TileSize = 56;
                TileMargin = 3;
                TileFontSize = 20;
                BoardContainerSize = 348;
                break;
            case 6:
                TileSize = 46;
                TileMargin = 3;
                TileFontSize = 16;
                BoardContainerSize = 352;
                break;
            default:
                TileSize = 72;
                TileMargin = 4;
                TileFontSize = 24;
                BoardContainerSize = 344;
                break;
        }
    }

    private void AddTileToPath(TileViewModel tile)
    {
        _selectedPath.Add(tile);
        tile.IsSelected = true;
        tile.SelectionOrder = _selectedPath.Count;
        UpdateCurrentWordFromPath();
    }

    private void UpdateCurrentWordFromPath()
    {
        CurrentWord = string.Concat(_selectedPath.Select(t => t.Letter));
    }

    private void ClearSelection()
    {
        foreach (TileViewModel tile in _selectedPath)
        {
            tile.IsSelected = false;
            tile.SelectionOrder = 0;
        }

        _selectedPath.Clear();
        CurrentWord = string.Empty;
    }

    private void SubmitDraggedWord(string word)
    {
        ProcessWordResult(_gameEngine.SubmitWord(word));
    }

    private async void OnTimerTick(object? sender, EventArgs e)
    {
        TimeRemaining -= TimeSpan.FromMilliseconds(100);
        if (TimeRemaining <= TimeSpan.Zero)
        {
            _timer.Stop();
            TimeRemaining = TimeSpan.Zero;
            await EndRoundAsync().ConfigureAwait(true);
        }
        else if (TimeRemaining.TotalSeconds <= 5 && TimeRemaining.Milliseconds < 100)
        {
            _audioManager.SoundEffects.Play(SoundEffect.TimerWarning);
        }
        else if (TimeRemaining.TotalSeconds <= 10 && TimeRemaining.Milliseconds < 100)
        {
            _audioManager.SoundEffects.Play(SoundEffect.TimerTick);
        }
    }

    private void OnSubmitWord()
    {
        string word = CurrentWord.Trim();
        if (string.IsNullOrWhiteSpace(word))
        {
            return;
        }

        ProcessWordResult(_gameEngine.SubmitWord(word));
        CurrentWord = string.Empty;
    }

    private void ProcessWordResult(WordResult result)
    {
        if (result.Status == WordStatus.Valid)
        {
            FoundWords.Insert(0, result);
        }

        Score = _currentRound?.Score ?? 0;

        _audioManager.SoundEffects.Play(result.Status == WordStatus.Valid ? SoundEffect.WordValid : SoundEffect.WordInvalid);

        LastSubmissionFeedback = result.Status switch
        {
            WordStatus.Valid => $"+{result.Points} pts!",
            WordStatus.AlreadyFound => "Already found!",
            WordStatus.NotInDictionary => "Not a word!",
            WordStatus.NotOnBoard => "Not on board!",
            WordStatus.TooShort => "Too short!",
            _ => string.Empty,
        };
    }

    private void OnPause()
    {
        _gameEngine.PauseRound();
        _timer.Stop();
        _audioManager.SoundEffects.Play(SoundEffect.Pause);
        IsPaused = true;
    }

    private void OnResume()
    {
        _gameEngine.ResumeRound();
        _audioManager.SoundEffects.Play(SoundEffect.Resume);
        _timer.Start();
        IsPaused = false;
    }

    private async Task EndRoundAsync()
    {
        _timer.Stop();
        IsTimerWarning = false;

        if (_currentRound == null || _currentRound.State == GameRoundState.Ended)
        {
            return;
        }

        try
        {
            _audioManager.SoundEffects.Play(SoundEffect.RoundEnd);
            GameRound completedRound = _gameEngine.EndRound();
            _currentRound = null;

            GameStatistics stats = await _statisticsService.GetStatisticsAsync().ConfigureAwait(true);
            IReadOnlyList<Achievement> unlocked = _achievementService.CheckAchievements(completedRound, stats);
            await _highScoreService.TryRecordScoreAsync(completedRound).ConfigureAwait(true);
            await _statisticsService.UpdateStatisticsAsync(completedRound).ConfigureAwait(true);

            if (unlocked.Count > 0)
            {
                _audioManager.SoundEffects.Play(SoundEffect.AchievementUnlock);
            }

            _navigation.NavigateTo<RoundResultsViewModel>();
            if (_navigation.CurrentViewModel is RoundResultsViewModel resultsVm)
            {
                resultsVm.LoadResults(completedRound, unlocked);
            }
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            System.Diagnostics.Debug.WriteLine($"EndRoundAsync error: {ex}");
            _navigation.NavigateTo<MainMenuViewModel>();
        }
    }

    private void OnQuit()
    {
        _timer.Stop();
        IsTimerWarning = false;

        if (_currentRound != null && _currentRound.State != GameRoundState.Ended)
        {
            _gameEngine.EndRound();
            _currentRound = null;
        }

        _navigation.NavigateTo<MainMenuViewModel>();
    }
}
