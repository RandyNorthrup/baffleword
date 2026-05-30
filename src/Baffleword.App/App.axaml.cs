// <copyright file="App.axaml.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App;

using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Baffleword.Audio;
using Baffleword.Core.Dictionary;
using Baffleword.Core.Models;
using Baffleword.Core.Repositories;
using Baffleword.Core.Services;
using Baffleword.Data;
using Baffleword.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

/// <summary>
/// Application entry point and DI container configuration.
/// </summary>
public sealed partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    /// <summary>
    /// Gets the DI service provider.
    /// </summary>
    public IServiceProvider? Services => _serviceProvider;

    /// <inheritdoc/>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <inheritdoc/>
    public override void OnFrameworkInitializationCompleted()
    {
        ConfigureLogging();

        ServiceCollection services = [];
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                Log.Fatal(ex, "Unhandled domain exception");
            }
        };

        TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            Log.Error(args.Exception, "Unobserved task exception");
            args.SetObserved();
        };

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            desktop.Exit += (_, _) => OnDesktopExit();
            _ = InitializeAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureLogging()
    {
        string logDirectory = AppPaths.GetLogDirectory();
        Directory.CreateDirectory(logDirectory);

#pragma warning disable CA1305 // Serilog builder API does not accept IFormatProvider.
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .WriteTo.File(
                Path.Combine(logDirectory, "baffleword-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10,
                fileSizeLimitBytes: 10 * 1024 * 1024,
                formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();
#pragma warning restore CA1305
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(dispose: true);
        });

        string dbPath = AppPaths.GetDatabasePath();
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        string connectionString = $"Data Source={dbPath}";

        services.AddSingleton<BafflewordDatabase>(sp =>
            new BafflewordDatabase(connectionString, sp.GetRequiredService<ILogger<BafflewordDatabase>>()));
        services.AddSingleton<IBafflewordDatabase>(sp => sp.GetRequiredService<BafflewordDatabase>());

        services.AddSingleton<IHighScoreRepository, HighScoreRepository>();
        services.AddSingleton<IAchievementRepository, AchievementRepository>();
        services.AddSingleton<ISettingsRepository, SettingsRepository>();
        services.AddSingleton<IStatisticsRepository, StatisticsRepository>();

        services.AddSingleton<WordListLoader>();
        services.AddSingleton<IDictionaryProvider, TrieDictionaryProvider>();
        services.AddSingleton<IBoardGenerator, BoardGenerator>();
        services.AddSingleton<IWordValidator, WordValidator>();
        services.AddSingleton<IBoardSolver, BoardSolver>();
        services.AddSingleton<IScoringService, ScoringService>();
        services.AddSingleton<IGameEngine, GameEngine>();
        services.AddSingleton<IAchievementService, AchievementService>();
        services.AddSingleton<IHighScoreService, HighScoreService>();
        services.AddSingleton<IStatisticsService, StatisticsService>();

        services.AddSingleton<SoundFlowAudioDevice>();
        services.AddSingleton<ISoundEffectPlayer, SoundFlowSoundEffectPlayer>();
        services.AddSingleton<IMusicPlayer, SoundFlowMusicPlayer>();
        services.AddSingleton<IAudioManager, AudioManager>();
        services.AddSingleton<Navigation.INavigationService, Navigation.NavigationService>();
        services.AddSingleton<Services.IToastService, Services.ToastService>();
        services.AddSingleton<MainWindow>();

        services.AddTransient<ViewModels.MainMenuViewModel>();
        services.AddTransient<ViewModels.GameViewModel>();
        services.AddTransient<ViewModels.RoundResultsViewModel>();
        services.AddTransient<ViewModels.HighScoresViewModel>();
        services.AddTransient<ViewModels.AchievementsViewModel>();
        services.AddTransient<ViewModels.SettingsViewModel>();
        services.AddTransient<ViewModels.HowToPlayViewModel>();
    }

    private static async Task LoadAudioSettingsAsync(IServiceProvider services)
    {
        IAudioManager audioManager = services.GetRequiredService<IAudioManager>();
        ISettingsRepository settingsRepo = services.GetRequiredService<ISettingsRepository>();

        string? sfxSetting = await settingsRepo.GetAsync("SfxVolume").ConfigureAwait(true);
        if (sfxSetting != null && double.TryParse(sfxSetting, CultureInfo.InvariantCulture, out double sfxVol))
        {
            audioManager.SoundEffects.Volume = (float)sfxVol;
        }

        string? musicSetting = await settingsRepo.GetAsync("MusicVolume").ConfigureAwait(true);
        if (musicSetting != null && double.TryParse(musicSetting, CultureInfo.InvariantCulture, out double musicVol))
        {
            audioManager.Music.Volume = (float)musicVol;
        }

        string? sfxMuted = await settingsRepo.GetAsync("SfxMuted").ConfigureAwait(true);
        if (sfxMuted != null && bool.TryParse(sfxMuted, out bool sfxMute) && sfxMute)
        {
            audioManager.SoundEffects.IsMuted = true;
        }

        string? musicMuted = await settingsRepo.GetAsync("MusicMuted").ConfigureAwait(true);
        if (musicMuted != null && bool.TryParse(musicMuted, out bool musicMute) && musicMute)
        {
            audioManager.Music.IsMuted = true;
        }

        string soundsDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds");
        string musicDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Music");
        audioManager.Initialize(soundsDir, musicDir);
    }

    private static void InitializeDictionary(IServiceProvider services)
    {
        TrieDictionaryProvider trie = (TrieDictionaryProvider)services.GetRequiredService<IDictionaryProvider>();
        using Stream wordStream = typeof(App).Assembly.GetManifestResourceStream("Baffleword.App.Assets.WordLists.english.txt")!;
        WordListLoader loader = services.GetRequiredService<WordListLoader>();
        trie.LoadWords(loader.LoadFromStream(wordStream));
    }

    private static async Task LoadAchievementsAsync(IServiceProvider services)
    {
        IAchievementRepository achievementRepo = services.GetRequiredService<IAchievementRepository>();
        IReadOnlyList<Achievement> savedAchievements = await achievementRepo.GetAllAsync().ConfigureAwait(true);
        AchievementService achievementService = (AchievementService)services.GetRequiredService<IAchievementService>();
        achievementService.LoadState(savedAchievements);
    }

    private async Task InitializeAsync()
    {
        if (_serviceProvider is null)
        {
            return;
        }

        try
        {
            IBafflewordDatabase database = _serviceProvider.GetRequiredService<IBafflewordDatabase>();
            await database.InitializeAsync().ConfigureAwait(true);

            await LoadAudioSettingsAsync(_serviceProvider).ConfigureAwait(true);
            InitializeDictionary(_serviceProvider);
            await LoadAchievementsAsync(_serviceProvider).ConfigureAwait(true);

            Navigation.INavigationService navigation = _serviceProvider.GetRequiredService<Navigation.INavigationService>();
            navigation.NavigateTo<ViewModels.MainMenuViewModel>();

            Log.Information("Baffleword application started");
        }
        catch (Exception ex) when (ex is not OutOfMemoryException)
        {
            Log.Fatal(ex, "Application initialization failed");
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown(1);
            }
        }
    }

    private void OnDesktopExit()
    {
        Log.Information("Baffleword application shutting down");
        _serviceProvider?.Dispose();
        Log.CloseAndFlush();
    }
}
