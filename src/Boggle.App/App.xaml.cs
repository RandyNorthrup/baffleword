// <copyright file="App.xaml.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App;

using System.IO;
using System.Windows;
using Boggle.Audio;
using Boggle.Core.Dictionary;
using Boggle.Core.Repositories;
using Boggle.Core.Services;
using Boggle.Data;
using Boggle.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

/// <summary>
/// Application entry point and DI container configuration.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    /// <inheritdoc/>
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Configure Serilog
        string logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDirectory);

#pragma warning disable CA1305 // Serilog builder API does not accept IFormatProvider
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Debug()
            .WriteTo.File(
                Path.Combine(logDirectory, "boggle-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10,
                fileSizeLimitBytes: 10 * 1024 * 1024)
            .CreateLogger();
#pragma warning restore CA1305

        // Build DI container
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Initialize database
        IBoggleDatabase database = _serviceProvider.GetRequiredService<IBoggleDatabase>();
        await database.InitializeAsync().ConfigureAwait(true);

        // Initialize audio system
        IAudioManager audioManager = _serviceProvider.GetRequiredService<IAudioManager>();
        string soundsDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds");
        string musicDir = Path.Combine(AppContext.BaseDirectory, "Assets", "Music");
        audioManager.Initialize(soundsDir, musicDir);

        // Initialize dictionary
        TrieDictionaryProvider trie = (TrieDictionaryProvider)_serviceProvider.GetRequiredService<IDictionaryProvider>();
        using Stream wordStream = GetType().Assembly.GetManifestResourceStream("Boggle.App.Assets.WordLists.english.txt")!;
        WordListLoader loader = _serviceProvider.GetRequiredService<WordListLoader>();
        trie.LoadWords(loader.LoadFromStream(wordStream));

        // Create and show main window
        MainWindow mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow = mainWindow;
        mainWindow.Show();

        // Navigate to main menu
        Navigation.INavigationService navigation = _serviceProvider.GetRequiredService<Navigation.INavigationService>();
        navigation.NavigateTo<ViewModels.MainMenuViewModel>();

        // Set up global exception handlers
        bool isHandlingException = false;
        DispatcherUnhandledException += (_, args) =>
        {
            args.Handled = true;
            if (isHandlingException)
            {
                return;
            }

            isHandlingException = true;
            Log.Fatal(args.Exception, "Unhandled dispatcher exception");
            MessageBox.Show(
                "An unexpected error occurred. The application will close.\n\n" + args.Exception.Message,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
        };

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

        Log.Information("Boggle application started");
    }

    /// <inheritdoc/>
    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Boggle application shutting down");

        _serviceProvider?.Dispose();
        Log.CloseAndFlush();

        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Logging
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(dispose: true);
        });

        // Database
        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Boggle",
            "boggle.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        string connectionString = $"Data Source={dbPath}";

        services.AddSingleton<BoggleDatabase>(sp =>
            new BoggleDatabase(connectionString, sp.GetRequiredService<ILogger<BoggleDatabase>>()));
        services.AddSingleton<IBoggleDatabase>(sp => sp.GetRequiredService<BoggleDatabase>());

        // Repositories
        services.AddSingleton<IHighScoreRepository, HighScoreRepository>();
        services.AddSingleton<IAchievementRepository, AchievementRepository>();
        services.AddSingleton<ISettingsRepository, SettingsRepository>();
        services.AddSingleton<IStatisticsRepository, StatisticsRepository>();

        // Core services
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

        // Audio
        services.AddSingleton<ISoundEffectPlayer, SoundEffectPlayer>();
        services.AddSingleton<IMusicPlayer, MusicPlayer>();
        services.AddSingleton<IAudioManager, AudioManager>();

        // Navigation
        services.AddSingleton<Navigation.INavigationService, Navigation.NavigationService>();

        // Main Window
        services.AddSingleton<MainWindow>();

        // ViewModels
        services.AddTransient<ViewModels.MainMenuViewModel>();
        services.AddTransient<ViewModels.GameViewModel>();
        services.AddTransient<ViewModels.RoundResultsViewModel>();
        services.AddTransient<ViewModels.HighScoresViewModel>();
        services.AddTransient<ViewModels.AchievementsViewModel>();
        services.AddTransient<ViewModels.SettingsViewModel>();
        services.AddTransient<ViewModels.HowToPlayViewModel>();
    }
}
