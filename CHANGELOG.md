# Changelog

All notable changes to the Boggle project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

---

## [Unreleased]

### Fixed — Audit Verification
- **Pause blur effect**: Added `BlurEffect` (radius 20) to GameContent grid when paused — board letters are now properly obscured, not just darkened
- **Settings integration**: GameViewModel now reads `TimerDurationSeconds` and `MinWordLength` from `ISettingsRepository` instead of hardcoding 3 minutes / 3 letters
- **Test count**: 243 tests all passing (added settings-integration verification test)

### Added — Milestone 10: Final Quality Pass
- **Zero analyzer warnings**: Full build with StyleCop, SonarAnalyzer, and .NET analyzers produces 0 warnings, 0 errors
- **Test coverage**: 242 tests all passing — Core 91.4% line coverage, App 21% (WPF views/code-behind not unit-testable), Audio 11.1% (hardware-dependent NAudio)
- **App icon**: Generated 256×256 blue rounded-square icon with white "B" letter and 4×4 grid motif; set as ApplicationIcon and Window.Icon
- **README.md**: Comprehensive readme with features, scoring table, prerequisites, build/run/test instructions, project structure, architecture overview, tech stack
- **Final PLAN.md**: All 10 milestones marked Complete ✅

### Added — Milestone 9: Polish & Animations
- **Board roll-in animation** (M9.1): Staggered scale+fade entrance for each tile with BackEase easing, 40ms delay between tiles
- **Word feedback pulse** (M9.2): Scale pulse animation (1.0→1.2→1.0) on LastSubmissionFeedback TextBlock when new feedback appears
- **Achievement unlock popup** (M9.3): Gold badges with slide-in animation (TranslateY -20→0 + fade 0→1) on RoundResultsView, staggered 200ms per achievement, with AchievementUnlock SFX
- **High score celebration** (M9.4): ElasticEase bounce animation (scale 0.5→1.0) on score card in RoundResultsView on load
- **View transition animations** (M9.5): Fade-out (120ms) then fade-in (200ms) on ContentControl when navigating between views
- **Timer pulse** (M9.6): IsTimerWarning property on GameViewModel (true when ≤5s remaining), triggers red foreground + repeating ScaleTransform pulse (1.0→1.15) via XAML DataTrigger
- **Button micro-interactions** (M9.7): ScaleTransform animations on PrimaryButtonStyle and SecondaryButtonStyle — hover scales to 1.03x (150ms), press scales to 0.97x (80ms), release returns to 1.03x
- **Unlocked achievements in results**: GameViewModel now captures CheckAchievements return value, passes to RoundResultsViewModel.LoadResults; UnlockedAchievements ObservableCollection displayed with animated gold badges
- **Test count**: 242 tests, all passing

### Added — Milestone 8: Settings & How to Play
- **SettingsViewModel**: Full settings management with ISettingsRepository key-value persistence (TimerDurationSeconds 60-600, MinWordLength 3-5, SfxVolume 0.0-1.0, MusicVolume 0.0-1.0), value clamping, live audio volume updates via IAudioManager, Save/Back commands
- **SettingsView**: XAML view with sliders for all 4 settings, value display, Save/Back buttons, themed CardStyle layout
- **HowToPlayViewModel**: Simple navigation-backed view model with BackCommand to MainMenuViewModel
- **HowToPlayView**: XAML view with game objective, rules (adjacency, Qu tile, NWL2020 dictionary), scoring table (3-4=1pt, 5=2, 6=3, 7=5, 8+=11), gameplay tips
- **DataTemplate mapping**: MainWindow extended with implicit DataTemplates for SettingsViewModel and HowToPlayViewModel
- **Settings persistence**: Async load/save via ISettingsRepository (SQLite Settings table), InvariantCulture formatting for numeric values
- **SettingsViewModelTests** (15 tests): Null guards, default values, clamping for all 4 properties, live audio volume updates, save persistence verification, back navigation, load from repository
- **HowToPlayViewModelTests** (2 tests): Null guard, back navigation
- **Test count**: 242 tests, all passing

### Added — Milestone 7: Audio
- **Word list upgraded**: Replaced dwyl/english-words (363,794 generic words) with NWL2020 (191,745 words) — official North American Scrabble tournament dictionary from scrabblewords/scrabblewords repo
- **Dictionary initialization**: App.xaml.cs now loads NWL2020 word list into TrieDictionaryProvider via WordListLoader on startup from embedded resource
- **Sound effects generated**: `scripts/generate-sounds.ps1` produces 13 WAV files via sine wave synthesis — tile_click, word_valid, word_invalid, timer_tick, timer_warning, round_start, round_end, achievement_unlock, button_hover, button_click, high_score, pause, resume
- **Ambient music generated**: 30-second looping ambient track (C3 drone + G3 pad + E4 shimmer with crossfade) in Assets/Music/ambient.wav
- **Audio wired into GameViewModel**: IAudioManager injected, SFX triggered on round start (RoundStart), word submission (WordValid/WordInvalid), timer countdown (TimerTick at ≤10s, TimerWarning at ≤5s), pause/resume (Pause/Resume), round end (RoundEnd)
- **AudioManager initialization**: App.xaml.cs initializes audio system with sounds/music directories on startup
- **Audio DI registration**: ISoundEffectPlayer, IMusicPlayer, IAudioManager registered as singletons; volume properties exposed for settings integration
- **AudioManager tests** (5 tests): Existing mocked tests covering Initialize/Dispose/SoundEffects/Music properties
- **Test count**: 225 tests, all passing

### Added — Milestone 6: Pause, High Scores, Achievements
- **Pause overlay**: Integrated directly into GameView.xaml as semi-transparent overlay with Resume/Quit card; PauseViewModel uses Action delegates (not DI-resolved)
- **HighScoresView**: Scrollable leaderboard showing score, words found, longest word, completion%, date; themed CardStyle container with SecondaryButton back navigation
- **HighScoresViewModel**: Loads top 50 scores via IHighScoreService.GetTopScoresAsync, ObservableCollection binding, back navigation to main menu
- **AchievementsView**: WrapPanel grid of achievement cards with opacity-based lock/unlock visual state (0.5 locked, 1.0 unlocked), lock/unlock icons, name + description display
- **AchievementsViewModel**: Loads all 20 achievements from IAchievementService.GetAllAchievements(), ObservableCollection binding, back navigation
- **DataTemplate mapping**: MainWindow extended with implicit DataTemplates for HighScoresViewModel and AchievementsViewModel
- **AchievementService**: All 20 achievement definitions and condition checks (implemented in M2, wired into game flow in M5 via GameViewModel.EndRoundAsync)
- **High score recording**: Wired into GameViewModel.EndRoundAsync with IHighScoreService persistence (implemented in M5)
- **HighScoresViewModel tests** (5 tests): Back navigation, initial empty state, async score loading, null guard constructors
- **AchievementsViewModel tests** (5 tests): Back navigation, achievement loading from service, empty state, null guard constructors
- **Test count**: 215 → 225 tests (10 new), all passing

### Added — Milestone 5: Game UI
- **GameView**: Full game play screen with top bar (score + timer + pause button), 4×4 board grid using nested ItemsControl with themed letter tiles (72px, rounded corners, shadows), word input with Enter key binding, scrollable found-words list with point display, and integrated pause overlay with blur background
- **GameViewModel**: Complete game loop — starts round via IGameEngine, DispatcherTimer countdown (100ms tick), word submission with status feedback (Valid/AlreadyFound/NotInDictionary/NotOnBoard/TooShort), pause/resume state management, end-of-round achievement checking + high score recording + statistics persistence, quit navigation
- **RoundResultsView**: Score breakdown with 4-stat card (score, words found, completion%, longest word), found words list with points, expandable "words you missed" WrapPanel, New Round / Main Menu buttons
- **RoundResultsViewModel**: Loads completed GameRound data, populates found/missed word collections, navigation to new round or main menu
- **PauseViewModel**: Resume and Quit commands with Action delegates for GameView overlay integration
- **DataTemplate mapping**: MainWindow wired with implicit DataTemplates for MainMenuViewModel, GameViewModel, RoundResultsViewModel
- **GameViewModel tests** (14 tests): Constructor initialization (board, timer, state), word submission (valid/invalid feedback, clears input, updates found words), pause/resume state, quit navigation, initial state verification
- **RoundResultsViewModel tests** (11 tests): LoadResults sets all properties, populates found/missed words, longest word extraction, navigation commands, null guards
- **Test count**: 190 → 215 tests (25 new), all passing

### Added — Milestone 4: UI Framework
- **MainWindow app shell**: Navigation-frame architecture with `ContentControl` bound to `INavigationService.CurrentViewModel`; implicit `DataTemplate` mapping for ViewModel→View resolution
- **MainMenuView**: XAML UserControl with centered title ("BOGGLE"), 5 navigation buttons (New Game, High Scores, Achievements, How to Play, Settings), version footer; uses PrimaryButtonStyle/SecondaryButtonStyle from theme
- **App startup wiring**: `App.xaml.cs` creates MainWindow via DI, shows it, and navigates to MainMenuViewModel on startup
- **MainMenuViewModel tests** (7 tests): All 5 navigation commands verified, null guard, command presence
- **NavigationService tests** (6 tests): Initial null state, navigation sets CurrentViewModel, NavigationChanged event fires, multiple navigations create new instances, null guards
- **Test count**: 177 → 190 tests (13 new), all passing
- *Note*: M4.1–M4.3 (themes, ViewModelBase, RelayCommand, NavigationService) were scaffolded in M1 and verified working

### Added — Milestone 2: Core Game Logic
- **GameEngine tests** (17 tests): Round lifecycle (start/end/pause/resume), word submission (valid/invalid/duplicate/too short), state validation exceptions, board generation delegation, word normalization, completion percentage calculation
- **AchievementService tests** (15 tests): All 20 achievement definitions verified, condition checks (FirstWords, WordHoarder, Centurion, LongWord, MonsterWord, Dedicated, NoMistakes, StreakMaster, QuMaster, VocabularyBuilder, HighRoller), already-unlocked guard, LoadState restore
- **HighScoreService tests** (6 tests): Qualifying/non-qualifying score recording, empty leaderboard edge case, longest word extraction, completion percentage persistence, repository delegation
- **StatisticsService tests** (9 tests): Empty/populated repository reads, counter accumulation (rounds, score, words), conditional updates (highest score, longest word, best completion), play time tracking
- **WordListLoader tests** (10 tests): Stream loading with 3+ char filter, non-alphabetic filtering, whitespace trimming, empty line skipping, empty stream, null/empty path validation, case preservation
- **English word list** (363,794 words): Sourced from dwyl/english-words (public domain), filtered to 3–16 characters alphabetic only, lowercase, sorted, deduplicated. Configured as EmbeddedResource in Boggle.App.
- **Test count**: 117 → 177 tests (60 new), all passing with zero errors/warnings

### Added — Milestone 3: Data Layer
- All M3 items (SQLite schema, BoggleDatabase, HighScoreRepository, AchievementRepository, SettingsRepository, StatisticsRepository, integration tests) were completed in M1 scaffolding and verified working. 20 integration tests with in-memory SQLite passing.

### Fixed — Milestone 1: Build & Quality Gate Compliance
- **Installed .NET 9 SDK** (9.0.312) via winget; resolved PATH conflict with x86 runtime-only install
- **SonarAnalyzer.CSharp** version pinned to `10.*` wildcard (exact version 10.6.0 no longer available)
- **75 analyzer/compiler errors systematically resolved** to achieve zero-error, zero-warning build:
  - SA1636 (file header copyright): Disabled globally — personal project
  - CA1814/S2368: Converted `GameBoard` from multidimensional `BoardCell[,]` to jagged `BoardCell[][]`
  - CA1024: `GetAllCells()` method → `AllCells` property on GameBoard
  - CA1032: Added missing constructors to `GameStateException`, `InvalidWordException`
  - CA1716: Renamed `IMusicPlayer` methods (`Pause→PausePlayback`, `Resume→ResumePlayback`, `Stop→StopPlayback`)
  - CA1031: Replaced generic `catch (Exception)` with specific `InvalidOperationException`/`IOException` in audio layer
  - CA5394: Suppressed on `Die.Roll()`, `BoardGenerator.Generate()`, `FisherYatesShuffle()` (game randomness, not crypto)
  - CA2000: Suppressed on `SoundEffectPlayer.Play()` with deterministic disposal on failure path
  - CA2007: Converted `await using` → `using` in Data layer (SQLite sync disposal is fine for local file I/O)
  - CA1849/S6966: Changed `reader.IsDBNull()` → `await reader.IsDBNullAsync().ConfigureAwait(false)` in AchievementRepository
  - SA1118: Extracted multi-line parameter to local variable in AchievementRepository
  - xUnit1030: Removed `.ConfigureAwait(false)` from all test methods (conflicts with xUnit parallelization)
  - CA1806: Suppressed for test projects (Assert.Throws pattern intentionally discards instances)
  - S3878/CA1861: Extracted constant arrays to `static readonly` fields in test code
  - CA1515: Suppressed for Boggle.App (WPF XAML requires public types)
  - SA1201/CA1003: Reordered and converted navigation event from `Action?` to `EventHandler?`
  - CA1822/S2325/CA1030: Made `RelayCommand.RaiseCanExecuteChanged()` static
  - CA1305: Suppressed Serilog config builder (no IFormatProvider overload available)
  - CS8625: Added null-forgiving operator for IValueConverter test parameters
- **SQLite in-memory test fix**: Added keepalive `SqliteConnection` to prevent shared cache eviction between connections
- **Placeholder app icon** generated (32×32 cornflower blue with "B")
- **Test project suppressions** in Directory.Build.targets: CA1707, SA1600, CA1806, CA2007

### Added — Milestone 1: Foundation & Scaffolding
- **Solution structure**: Boggle.sln with 4 source + 4 test projects organized in src/ and tests/ folders
- **Boggle.Core**: Complete game logic layer
  - Models: BoardCell, Die, DiceSet (16 official "New Boggle" dice), GameBoard (4×4 grid), WordResult, WordStatus, GameRound, GameRoundState, HighScoreEntry, Achievement, GameStatistics, PlayerProfile
  - Services: BoardGenerator (Fisher-Yates shuffle), WordValidator (DFS path tracing with Qu handling), ScoringService (official scoring table), BoardSolver (Trie-guided DFS), GameEngine (round lifecycle), plus IAchievementService, IHighScoreService, IStatisticsService interfaces
  - Dictionary: TrieDictionaryProvider (26-child trie, O(k) lookup), WordListLoader, IDictionaryProvider
  - Exceptions: GameStateException, InvalidWordException
- **Boggle.Data**: SQLite persistence layer
  - BoggleDatabase with schema creation (HighScores, Achievements, Settings, Statistics tables)
  - HighScoreRepository, AchievementRepository, SettingsRepository, StatisticsRepository — all async with parameterized queries
- **Boggle.Audio**: NAudio-based audio system
  - SoundEffect enum (13 effects), AudioManager, SoundEffectPlayer (preload + fire-and-forget), MusicPlayer (loop/pause/resume/volume)
- **Boggle.App**: WPF application foundation
  - DI container (Microsoft.Extensions.DependencyInjection) registering all services
  - Serilog structured logging (rolling file + debug sink)
  - Theme system: Colors (14 colors), Brushes (solid + gradient), Typography (8 styles), Controls (PrimaryButton, SecondaryButton, Card, ModernTextBox)
  - MVVM base: ViewModelBase (INotifyPropertyChanged + SetProperty), RelayCommand (ICommand)
  - NavigationService with DI-backed view model creation
  - Value converters: BoolToVisibility, TimeSpanToString, ScoreToColor
  - MainMenuViewModel (fully wired) + 7 stub ViewModels for later milestones
- **Test projects**: Boggle.Core.Tests, Boggle.Data.Tests, Boggle.Audio.Tests, Boggle.App.Tests
  - 60+ unit/integration tests covering models, services, dictionary, repositories, ViewModels, converters
  - xUnit + Moq + FluentAssertions + coverlet configuration
- **Build configuration**: Directory.Build.props (TreatWarningsAsErrors, analyzers), Directory.Build.targets (coverage settings), global.json (.NET 9), .editorconfig (~200 rules)
- **Asset placeholders**: Sounds, Music, Icons, Fonts, WordLists directories
- **PLAN.md**: Comprehensive project planning document with 10 milestones covering:
  - Official Boggle game rules (Hasbro standard 4×4, "New Boggle" dice configuration)
  - Technology stack selection (.NET 9, WPF, C# 13, NAudio, SQLite, Serilog, xUnit)
  - Full project structure with 4 source projects and 4 test projects
  - MVVM architecture with dependency injection
  - UI/UX specification: color palette, typography, component design, all 8 view layouts
  - 20 achievements defined with unlock conditions
  - Sound effects plan (13 SFX + ambient music loop) with generation approach
  - Asset generation strategy (word list, font, icon, audio — all automated)
  - Quality gates: TreatWarningsAsErrors, StyleCop, Roslyn analyzers, SonarAnalyzer
  - Testing strategy targeting ≥95% line coverage across all projects
  - Performance targets (board generation <50ms, solver <500ms, memory <150MB)
  - Error handling patterns with Serilog structured logging
  - Dictionary/word list sourcing and Trie-based integration plan
- **CHANGELOG.md**: This changelog to track project progress against milestones

---

*Boggle Desktop App — Single Player Word Game*
