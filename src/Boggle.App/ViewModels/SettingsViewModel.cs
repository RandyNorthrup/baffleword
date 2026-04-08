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
public sealed class SettingsViewModel : ViewModelBase, IDisposable
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly INavigationService _navigation;
    private readonly IAudioManager _audioManager;

    private double _sfxVolume;
    private double _musicVolume;
    private bool _disposed;

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

        _sfxVolume = audioManager.SoundEffects.Volume;
        _musicVolume = audioManager.Music.Volume;

        BackCommand = new RelayCommand(OnBack);

        _ = LoadSettingsAsync();
    }

    /// <summary>
    /// Gets or sets the SFX volume (0.0–1.0). Returns 0 when muted.
    /// </summary>
    public double SfxVolume
    {
        get => _audioManager.SoundEffects.IsMuted ? 0.0 : _sfxVolume;
        set
        {
            if (!_disposed && !_audioManager.SoundEffects.IsMuted)
            {
                double clamped = Math.Clamp(value, 0.0, 1.0);
                if (SetProperty(ref _sfxVolume, clamped))
                {
                    _audioManager.SoundEffects.Volume = (float)_sfxVolume;
                    _ = SaveSettingAsync("SfxVolume", _sfxVolume);
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the music volume (0.0–1.0). Returns 0 when muted.
    /// </summary>
    public double MusicVolume
    {
        get => _audioManager.Music.IsMuted ? 0.0 : _musicVolume;
        set
        {
            if (!_disposed && !_audioManager.Music.IsMuted)
            {
                double clamped = Math.Clamp(value, 0.0, 1.0);
                if (SetProperty(ref _musicVolume, clamped))
                {
                    _audioManager.Music.Volume = (float)_musicVolume;
                    _ = SaveSettingAsync("MusicVolume", _musicVolume);
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether sound effects are muted. Directly controls the audio player.
    /// </summary>
    public bool IsSfxMuted
    {
        get => _audioManager.SoundEffects.IsMuted;
        set
        {
            if (!_disposed && _audioManager.SoundEffects.IsMuted != value)
            {
                _audioManager.SoundEffects.IsMuted = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SfxVolume));
                _ = SaveMuteAsync("SfxMuted", value);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether music is muted. Directly controls the audio player.
    /// </summary>
    public bool IsMusicMuted
    {
        get => _audioManager.Music.IsMuted;
        set
        {
            if (!_disposed && _audioManager.Music.IsMuted != value)
            {
                _audioManager.Music.IsMuted = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MusicVolume));
                _ = SaveMuteAsync("MusicMuted", value);
            }
        }
    }

    /// <summary>
    /// Gets the command to go back.
    /// </summary>
    public ICommand BackCommand { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        _disposed = true;
    }

    private async Task LoadSettingsAsync()
    {
        string? sfx = await _settingsRepository.GetAsync("SfxVolume").ConfigureAwait(true);
        if (sfx != null && double.TryParse(sfx, CultureInfo.InvariantCulture, out double sv))
        {
            _sfxVolume = Math.Clamp(sv, 0.0, 1.0);
            OnPropertyChanged(nameof(SfxVolume));
        }

        string? music = await _settingsRepository.GetAsync("MusicVolume").ConfigureAwait(true);
        if (music != null && double.TryParse(music, CultureInfo.InvariantCulture, out double mv))
        {
            _musicVolume = Math.Clamp(mv, 0.0, 1.0);
            OnPropertyChanged(nameof(MusicVolume));
        }

        // Mute state is read from the singleton audio players in the constructor.
        // No need to load from DB here — the singleton is the source of truth.
    }

    private async Task SaveSettingAsync(string key, double value)
    {
        await _settingsRepository.SetAsync(key, value.ToString(CultureInfo.InvariantCulture)).ConfigureAwait(true);
    }

    private async Task SaveMuteAsync(string key, bool value)
    {
        await _settingsRepository.SetAsync(key, value.ToString()).ConfigureAwait(true);
    }

    private void OnBack()
    {
        if (!_navigation.GoBack())
        {
            _navigation.NavigateTo<MainMenuViewModel>();
        }
    }
}
