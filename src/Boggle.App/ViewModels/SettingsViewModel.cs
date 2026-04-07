// <copyright file="SettingsViewModel.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.ViewModels;

using System.Globalization;
using System.Windows.Input;
using Boggle.App.Navigation;
using Boggle.Audio;
using Boggle.Core.Repositories;

/// <summary>
/// ViewModel for the settings view.
/// </summary>
public sealed class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly INavigationService _navigation;
    private readonly IAudioManager _audioManager;

    private int _timerDurationSeconds = 180;
    private int _minWordLength = 3;
    private double _sfxVolume = 0.8;
    private double _musicVolume = 0.5;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    /// <param name="settingsRepository">The settings repository.</param>
    /// <param name="navigation">The navigation service.</param>
    /// <param name="audioManager">The audio manager.</param>
    public SettingsViewModel(ISettingsRepository settingsRepository, INavigationService navigation, IAudioManager audioManager)
    {
        _settingsRepository = settingsRepository ?? throw new ArgumentNullException(nameof(settingsRepository));
        _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        _audioManager = audioManager ?? throw new ArgumentNullException(nameof(audioManager));

        SaveCommand = new RelayCommand(() => _ = SaveAsync());
        BackCommand = new RelayCommand(OnBack);

        _ = LoadSettingsAsync();
    }

    /// <summary>
    /// Gets or sets the timer duration in seconds (60–600).
    /// </summary>
    public int TimerDurationSeconds
    {
        get => _timerDurationSeconds;
        set => SetProperty(ref _timerDurationSeconds, Math.Clamp(value, 60, 600));
    }

    /// <summary>
    /// Gets or sets the minimum word length (3–5).
    /// </summary>
    public int MinWordLength
    {
        get => _minWordLength;
        set => SetProperty(ref _minWordLength, Math.Clamp(value, 3, 5));
    }

    /// <summary>
    /// Gets or sets the SFX volume (0.0–1.0).
    /// </summary>
    public double SfxVolume
    {
        get => _sfxVolume;
        set
        {
            if (SetProperty(ref _sfxVolume, Math.Clamp(value, 0.0, 1.0)))
            {
                _audioManager.SoundEffects.Volume = (float)_sfxVolume;
            }
        }
    }

    /// <summary>
    /// Gets or sets the music volume (0.0–1.0).
    /// </summary>
    public double MusicVolume
    {
        get => _musicVolume;
        set
        {
            if (SetProperty(ref _musicVolume, Math.Clamp(value, 0.0, 1.0)))
            {
                _audioManager.Music.Volume = (float)_musicVolume;
            }
        }
    }

    /// <summary>
    /// Gets the command to save settings.
    /// </summary>
    public ICommand SaveCommand { get; }

    /// <summary>
    /// Gets the command to go back.
    /// </summary>
    public ICommand BackCommand { get; }

    private async Task LoadSettingsAsync()
    {
        string? timer = await _settingsRepository.GetAsync("TimerDurationSeconds").ConfigureAwait(true);
        if (timer != null && int.TryParse(timer, CultureInfo.InvariantCulture, out int t))
        {
            TimerDurationSeconds = t;
        }

        string? minWord = await _settingsRepository.GetAsync("MinWordLength").ConfigureAwait(true);
        if (minWord != null && int.TryParse(minWord, CultureInfo.InvariantCulture, out int mw))
        {
            MinWordLength = mw;
        }

        string? sfx = await _settingsRepository.GetAsync("SfxVolume").ConfigureAwait(true);
        if (sfx != null && double.TryParse(sfx, CultureInfo.InvariantCulture, out double sv))
        {
            SfxVolume = sv;
        }

        string? music = await _settingsRepository.GetAsync("MusicVolume").ConfigureAwait(true);
        if (music != null && double.TryParse(music, CultureInfo.InvariantCulture, out double mv))
        {
            MusicVolume = mv;
        }
    }

    private async Task SaveAsync()
    {
        await _settingsRepository.SetAsync("TimerDurationSeconds", TimerDurationSeconds.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(true);
        await _settingsRepository.SetAsync("MinWordLength", MinWordLength.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(true);
        await _settingsRepository.SetAsync("SfxVolume", SfxVolume.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(true);
        await _settingsRepository.SetAsync("MusicVolume", MusicVolume.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(true);

        _navigation.NavigateTo<MainMenuViewModel>();
    }

    private void OnBack()
    {
        _navigation.NavigateTo<MainMenuViewModel>();
    }
}
