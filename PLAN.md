# Boggle Desktop App — Master Plan

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [Game Rules Reference](#2-game-rules-reference)
3. [Technology Stack](#3-technology-stack)
4. [Project Structure](#4-project-structure)
5. [Architecture](#5-architecture)
6. [UI/UX Design Specification](#6-uiux-design-specification)
7. [Feature Specifications](#7-feature-specifications)
8. [Sound & Music Plan](#8-sound--music-plan)
9. [Asset Generation Plan](#9-asset-generation-plan)
10. [Quality Gates & Linting](#10-quality-gates--linting)
11. [Testing Strategy](#11-testing-strategy)
12. [Performance & Memory Guards](#12-performance--memory-guards)
13. [Error Handling & Logging](#13-error-handling--logging)
14. [Milestones & Timeline](#14-milestones--timeline)
15. [Dictionary / Word List](#15-dictionary--word-list)

---

## 1. Project Overview

**Name:** Boggle  
**Type:** Single-player desktop word game  
**Platform:** Windows  
**Framework:** WPF on .NET 9 (LTS)  
**Language:** C# 13  
**License:** Private / Personal  

### Goals

- Faithful recreation of the official Boggle board game rules (Hasbro standard 4×4)
- Modern, beautiful UI with a light blue / gray / near-white color scheme
- Full sound effects and ambient music
- High scores, achievements, and game statistics
- Pause with blur overlay
- Near-100% test coverage with strict quality gates
- Zero warnings policy (treat warnings as errors)
- All assets auto-generated or sourced from free/open-source — no manual creation required

---

## 2. Game Rules Reference

### Official Standard Boggle (4×4 Grid)

| Rule | Detail |
|---|---|
| **Grid** | 4×4 = 16 letter dice |
| **Timer** | 3 minutes per round |
| **Minimum word length** | 3 letters |
| **Adjacency** | Horizontal, vertical, or diagonal neighbors |
| **Reuse** | No individual die may be used more than once per word |
| **"Qu" die** | One die has "Qu" printed together; counts as 2 letters for scoring |
| **Capitalized/hyphenated words** | Not allowed |
| **Multiple forms** | Singular, plural, and other derivations allowed |

### Scoring Table

| Word Length | Points |
|---|---|
| 3 letters | 1 |
| 4 letters | 1 |
| 5 letters | 2 |
| 6 letters | 3 |
| 7 letters | 5 |
| 8+ letters | 11 |

### Official "New Boggle" Dice Configuration (Hasbro #C2187)

These are the 16 dice, each with 6 faces:

| Die | Faces |
|---|---|
| 0 | A, A, E, E, G, N |
| 1 | A, B, B, J, O, O |
| 2 | A, C, H, O, P, S |
| 3 | A, F, F, K, P, S |
| 4 | A, O, O, T, T, W |
| 5 | C, I, M, O, T, U |
| 6 | D, E, I, L, R, X |
| 7 | D, E, L, R, V, Y |
| 8 | D, I, S, T, T, Y |
| 9 | E, E, G, H, N, W |
| 10 | E, E, I, N, S, U |
| 11 | E, H, R, T, V, W |
| 12 | E, I, O, S, S, T |
| 13 | E, L, R, T, T, Y |
| 14 | H, I, M, N, Qu, U |
| 15 | H, L, N, N, R, Z |

### Board Generation Algorithm

1. Create list of 16 dice
2. Shuffle dice order (Fisher-Yates) to assign random grid positions
3. For each die, randomly select one of its 6 faces
4. Place selected letter in the corresponding grid cell (row = index / 4, col = index % 4)

### Word Validation Algorithm

1. For each word submitted by the player:
   - Check minimum length (≥ 3)
   - Check word exists in dictionary (case-insensitive)
   - Check word is traceable on the board via DFS/backtracking:
     - Start from every cell matching the first letter
     - At each step, move to an adjacent (8-directional) unvisited cell matching the next letter
     - "Qu" cells match "QU" in the word (consuming 2 characters)
   - Check word hasn't already been submitted this round

### Single-Player Adaptation

Since the original game removes duplicate words between players, our single-player mode:
- Awards points for ALL valid words found (no duplicate elimination)
- Tracks total possible words per board for a "completion percentage" stat
- Has a built-in solver that reveals all possible words after the round ends

---

## 3. Technology Stack

### Core

| Component | Technology | Version | Purpose |
|---|---|---|---|
| Runtime | .NET 9 | 9.x (LTS) | Modern, performant runtime |
| Language | C# 13 | Latest stable | Nullable refs, pattern matching, etc. |
| UI Framework | WPF | .NET 9 | Native Windows desktop, XAML-based, hardware-accelerated |
| Audio | NAudio | 2.3.x | Sound effects & music playback (WAV, MP3, mixing) |
| Data Storage | SQLite via Microsoft.Data.Sqlite | Latest | High scores, achievements, settings persistence |
| Logging | Serilog | Latest | Structured logging with file + debug sinks |
| DI | Microsoft.Extensions.DependencyInjection | Latest | Constructor injection throughout |
| Config | Microsoft.Extensions.Configuration | Latest | appsettings.json support |
| JSON | System.Text.Json | Built-in | Serialization (no Newtonsoft dependency) |

### Quality & Analysis

| Tool | Purpose |
|---|---|
| StyleCop.Analyzers | Code style enforcement |
| Microsoft.CodeAnalysis.NetAnalyzers | Roslyn-based code quality rules |
| SonarAnalyzer.CSharp | Additional security & quality rules |
| .editorconfig | Consistent formatting across all files |
| Directory.Build.props | Centralized build settings, TreatWarningsAsErrors |
| Nullable reference types | Enabled globally (`<Nullable>enable</Nullable>`) |

### Testing

| Tool | Purpose |
|---|---|
| xUnit | Unit test framework |
| Moq | Mocking framework |
| FluentAssertions | Readable test assertions |
| coverlet.collector | Code coverage collection |
| ReportGenerator | HTML coverage reports |
| Microsoft.NET.Test.Sdk | Test SDK integration |
| Verify.Xunit | Snapshot testing for UI models |

### Build & CI

| Tool | Purpose |
|---|---|
| dotnet CLI | Build, test, publish |
| Directory.Build.props | Centralized project configuration |
| global.json | Pin SDK version |

---

## 4. Project Structure

```
boggle/
├── global.json
├── Directory.Build.props
├── Directory.Build.targets
├── .editorconfig
├── Boggle.sln
├── PLAN.md
├── CHANGELOG.md
├── README.md
│
├── src/
│   ├── Boggle.Core/                    # Game logic, no UI dependencies
│   │   ├── Boggle.Core.csproj
│   │   ├── Models/
│   │   │   ├── BoardCell.cs            # Single cell on the board
│   │   │   ├── GameBoard.cs            # 4×4 grid representation
│   │   │   ├── Die.cs                  # Single Boggle die with 6 faces
│   │   │   ├── DiceSet.cs             # All 16 official dice
│   │   │   ├── WordResult.cs           # Validation result for a submitted word
│   │   │   ├── GameRound.cs            # State for a single round
│   │   │   ├── GameStatistics.cs       # Aggregate player statistics
│   │   │   ├── HighScoreEntry.cs       # Single high score record
│   │   │   ├── Achievement.cs          # Achievement definition & state
│   │   │   └── PlayerProfile.cs        # Player settings & progress
│   │   ├── Services/
│   │   │   ├── IBoardGenerator.cs
│   │   │   ├── BoardGenerator.cs       # Dice rolling & board creation
│   │   │   ├── IWordValidator.cs
│   │   │   ├── WordValidator.cs        # Dictionary lookup + board path check
│   │   │   ├── IBoardSolver.cs
│   │   │   ├── BoardSolver.cs          # Finds ALL valid words on a board
│   │   │   ├── IScoringService.cs
│   │   │   ├── ScoringService.cs       # Points calculation
│   │   │   ├── IGameEngine.cs
│   │   │   ├── GameEngine.cs           # Orchestrates round lifecycle
│   │   │   ├── IAchievementService.cs
│   │   │   ├── AchievementService.cs   # Check & unlock achievements
│   │   │   ├── IHighScoreService.cs
│   │   │   ├── HighScoreService.cs     # Manage high score records
│   │   │   ├── IStatisticsService.cs
│   │   │   └── StatisticsService.cs    # Track lifetime stats
│   │   ├── Dictionary/
│   │   │   ├── IDictionaryProvider.cs
│   │   │   ├── TrieDictionaryProvider.cs  # Trie-based word lookup (fast prefix check)
│   │   │   └── WordListLoader.cs       # Load word list from embedded resource
│   │   └── Exceptions/
│   │       ├── InvalidWordException.cs
│   │       └── GameStateException.cs
│   │
│   ├── Boggle.Data/                    # Persistence layer
│   │   ├── Boggle.Data.csproj
│   │   ├── IBoggleDatabase.cs
│   │   ├── BoggleDatabase.cs           # SQLite operations
│   │   ├── Migrations/
│   │   │   └── InitialCreate.cs
│   │   └── Repositories/
│   │       ├── IHighScoreRepository.cs
│   │       ├── HighScoreRepository.cs
│   │       ├── IAchievementRepository.cs
│   │       ├── AchievementRepository.cs
│   │       ├── ISettingsRepository.cs
│   │       ├── SettingsRepository.cs
│   │       ├── IStatisticsRepository.cs
│   │       └── StatisticsRepository.cs
│   │
│   ├── Boggle.Audio/                   # Sound & music management
│   │   ├── Boggle.Audio.csproj
│   │   ├── IAudioManager.cs
│   │   ├── AudioManager.cs             # NAudio-based playback engine
│   │   ├── ISoundEffectPlayer.cs
│   │   ├── SoundEffectPlayer.cs        # Fire-and-forget SFX
│   │   ├── IMusicPlayer.cs
│   │   ├── MusicPlayer.cs             # Background music with fade/loop
│   │   └── SoundEffect.cs             # Enum of all sound effects
│   │
│   └── Boggle.App/                     # WPF application (UI layer)
│       ├── Boggle.App.csproj
│       ├── App.xaml / App.xaml.cs
│       ├── AssemblyInfo.cs
│       ├── appsettings.json
│       ├── Themes/
│       │   ├── Colors.xaml              # Color palette resource dictionary
│       │   ├── Brushes.xaml             # Gradient & solid brushes
│       │   ├── Typography.xaml          # Font styles
│       │   ├── Controls.xaml            # Button, TextBox, etc. styles
│       │   └── Animations.xaml          # Storyboard definitions
│       ├── Controls/
│       │   ├── BoardGrid.xaml/.cs       # The 4×4 game board custom control
│       │   ├── BoardCell.xaml/.cs       # Individual letter tile
│       │   ├── TimerDisplay.xaml/.cs    # Countdown timer ring
│       │   ├── ScoreDisplay.xaml/.cs    # Current score panel
│       │   ├── WordList.xaml/.cs        # Submitted words list
│       │   ├── WordInput.xaml/.cs       # Word entry textbox
│       │   ├── AchievementPopup.xaml/.cs # Toast notification for unlocks
│       │   └── BlurOverlay.xaml/.cs     # Pause blur effect overlay
│       ├── Views/
│       │   ├── MainMenuView.xaml/.cs
│       │   ├── GameView.xaml/.cs
│       │   ├── PauseView.xaml/.cs
│       │   ├── RoundResultsView.xaml/.cs
│       │   ├── HighScoresView.xaml/.cs
│       │   ├── AchievementsView.xaml/.cs
│       │   ├── SettingsView.xaml/.cs
│       │   └── HowToPlayView.xaml/.cs
│       ├── ViewModels/
│       │   ├── ViewModelBase.cs         # INotifyPropertyChanged base
│       │   ├── MainMenuViewModel.cs
│       │   ├── GameViewModel.cs
│       │   ├── PauseViewModel.cs
│       │   ├── RoundResultsViewModel.cs
│       │   ├── HighScoresViewModel.cs
│       │   ├── AchievementsViewModel.cs
│       │   ├── SettingsViewModel.cs
│       │   └── HowToPlayViewModel.cs
│       ├── Navigation/
│       │   ├── INavigationService.cs
│       │   └── NavigationService.cs
│       ├── Converters/
│       │   ├── BoolToVisibilityConverter.cs
│       │   ├── ScoreToColorConverter.cs
│       │   └── TimeSpanToStringConverter.cs
│       └── Assets/
│           ├── Fonts/
│           │   └── Inter-Variable.ttf   # Modern sans-serif font
│           ├── Sounds/
│           │   ├── tile_click.wav
│           │   ├── word_valid.wav
│           │   ├── word_invalid.wav
│           │   ├── timer_tick.wav
│           │   ├── timer_warning.wav
│           │   ├── round_start.wav
│           │   ├── round_end.wav
│           │   ├── achievement_unlock.wav
│           │   ├── button_hover.wav
│           │   ├── button_click.wav
│           │   ├── high_score.wav
│           │   ├── pause.wav
│           │   └── resume.wav
│           ├── Music/
│           │   └── ambient_loop.mp3     # Soothing background track
│           ├── Icons/
│           │   ├── app_icon.ico
│           │   └── app_icon.png
│           └── WordLists/
│               └── english.txt          # ~270K English word list (TWL/SOWPODS)
│
├── tests/
│   ├── Boggle.Core.Tests/
│   │   ├── Boggle.Core.Tests.csproj
│   │   ├── Models/
│   │   │   ├── GameBoardTests.cs
│   │   │   ├── DieTests.cs
│   │   │   ├── DiceSetTests.cs
│   │   │   ├── GameRoundTests.cs
│   │   │   └── AchievementTests.cs
│   │   ├── Services/
│   │   │   ├── BoardGeneratorTests.cs
│   │   │   ├── WordValidatorTests.cs
│   │   │   ├── BoardSolverTests.cs
│   │   │   ├── ScoringServiceTests.cs
│   │   │   ├── GameEngineTests.cs
│   │   │   ├── AchievementServiceTests.cs
│   │   │   ├── HighScoreServiceTests.cs
│   │   │   └── StatisticsServiceTests.cs
│   │   └── Dictionary/
│   │       ├── TrieDictionaryProviderTests.cs
│   │       └── WordListLoaderTests.cs
│   │
│   ├── Boggle.Data.Tests/
│   │   ├── Boggle.Data.Tests.csproj
│   │   └── Repositories/
│   │       ├── HighScoreRepositoryTests.cs
│   │       ├── AchievementRepositoryTests.cs
│   │       ├── SettingsRepositoryTests.cs
│   │       └── StatisticsRepositoryTests.cs
│   │
│   ├── Boggle.Audio.Tests/
│   │   ├── Boggle.Audio.Tests.csproj
│   │   ├── AudioManagerTests.cs
│   │   ├── SoundEffectPlayerTests.cs
│   │   └── MusicPlayerTests.cs
│   │
│   └── Boggle.App.Tests/
│       ├── Boggle.App.Tests.csproj
│       ├── ViewModels/
│       │   ├── MainMenuViewModelTests.cs
│       │   ├── GameViewModelTests.cs
│       │   ├── PauseViewModelTests.cs
│       │   ├── RoundResultsViewModelTests.cs
│       │   ├── HighScoresViewModelTests.cs
│       │   ├── AchievementsViewModelTests.cs
│       │   ├── SettingsViewModelTests.cs
│       │   └── HowToPlayViewModelTests.cs
│       ├── Navigation/
│       │   └── NavigationServiceTests.cs
│       └── Converters/
│           ├── BoolToVisibilityConverterTests.cs
│           ├── ScoreToColorConverterTests.cs
│           └── TimeSpanToStringConverterTests.cs
│
└── tools/
    └── generate-sounds.ps1              # PowerShell script to generate placeholder WAV files
```

---

## 5. Architecture

### Design Patterns

| Pattern | Usage |
|---|---|
| **MVVM** | ViewModels bind to Views via DataContext; no code-behind logic |
| **Repository** | Data access abstracted behind interfaces |
| **Dependency Injection** | All services registered in DI container; constructor injection |
| **Strategy** | `IDictionaryProvider` allows swapping word list implementations |
| **Observer** | `INotifyPropertyChanged` for all ViewModel properties |
| **Command** | `ICommand` / `RelayCommand` for all UI actions |

### Layer Dependencies

```
Boggle.App (UI) → Boggle.Core, Boggle.Data, Boggle.Audio
Boggle.Data      → Boggle.Core (models only)
Boggle.Audio     → (standalone, no project dependencies)
Boggle.Core      → (standalone, no project dependencies)
```

### Data Flow

1. **App startup** → DI container configured → MainMenuView shown
2. **New game** → `GameEngine.StartRound()` → `BoardGenerator.Generate()` → board displayed
3. **Word submission** → `GameViewModel` → `WordValidator.Validate()` → score updated → SFX played
4. **Timer expires** → `GameEngine.EndRound()` → `BoardSolver.Solve()` → results shown
5. **Results** → `ScoringService.CalculateTotal()` → `HighScoreService.TryRecord()` → `AchievementService.Check()`

---

## 6. UI/UX Design Specification

### Color Palette

| Name | Hex | Usage |
|---|---|---|
| **Primary Blue** | `#4A9BD9` | Buttons, active states, highlights |
| **Light Blue** | `#B8D8F0` | Card backgrounds, hover states |
| **Pale Blue** | `#E8F2FA` | Page backgrounds |
| **Near White** | `#F7F9FC` | Content area backgrounds |
| **White** | `#FFFFFF` | Card surfaces, input fields |
| **Gray 100** | `#F0F2F5` | Dividers, disabled backgrounds |
| **Gray 300** | `#C4CAD0` | Borders, subtle text |
| **Gray 500** | `#7A8490` | Secondary text |
| **Gray 700** | `#3D4551` | Primary text |
| **Gray 900** | `#1A1F26` | Headings |
| **Success Green** | `#4CAF82` | Valid word feedback |
| **Error Red** | `#E05C5C` | Invalid word feedback |
| **Warning Amber** | `#E8A735` | Timer warning state |
| **Gold** | `#F0C040` | High score / achievement accent |

### Typography

| Element | Font | Weight | Size |
|---|---|---|---|
| Headings (H1) | Inter | 700 (Bold) | 28px |
| Headings (H2) | Inter | 600 (SemiBold) | 22px |
| Body | Inter | 400 (Regular) | 14px |
| Board Letters | Inter | 700 (Bold) | 32px |
| Score Numbers | Inter | 700 (Bold) | 24px |
| Timer | Inter | 300 (Light) | 48px |
| Button Text | Inter | 600 (SemiBold) | 14px |
| Small/Caption | Inter | 400 (Regular) | 12px |

### Component Design Language

| Property | Value |
|---|---|
| **Border Radius** | 12px (cards), 8px (buttons/inputs), 16px (large panels) |
| **Shadow (small)** | `0 2px 8px rgba(0,0,0,0.06)` |
| **Shadow (medium)** | `0 4px 16px rgba(0,0,0,0.08)` |
| **Shadow (large)** | `0 8px 32px rgba(0,0,0,0.12)` |
| **Transparency** | Background cards at 90% opacity over blur |
| **Gradients** | Subtle top-to-bottom on buttons: Primary Blue → slightly darker |
| **Transitions** | 200ms ease for hover/press states |

### View Layouts

#### Main Menu
- Centered logo/title at top
- Large rounded buttons stacked vertically: "New Game", "High Scores", "Achievements", "How to Play", "Settings"
- Subtle animated background gradient
- Version number in bottom corner

#### Game View
- **Top bar**: Score (left), Timer (center, circular progress ring), Pause button (right)
- **Center**: 4×4 board grid with letter tiles (rounded squares with shadow)
- **Bottom**: Word input field + submit button, scrollable list of found words
- Letter tiles: white background, bold letter, subtle shadow, blue border highlight when part of a valid path
- Board tiles animate in with a stagger effect on round start

#### Pause Overlay
- Full-screen overlay with `BlurEffect` (radius 20) on the game content behind
- Semi-transparent dark overlay (`rgba(0,0,0,0.3)`)
- Centered card: "Paused" heading, "Resume" button, "Quit to Menu" button
- Board letters are NOT visible through the blur (prevents cheating)

#### Round Results
- Score breakdown: list of found words with points
- "Words you missed" expandable section showing all remaining valid words
- Stats: completion percentage, longest word, total words found
- "New Round" and "Main Menu" buttons

#### High Scores
- Scrollable table: Rank, Score, Words Found, Longest Word, Date
- Top 3 get gold/silver/bronze accent colors
- "Clear Scores" button with confirmation dialog

#### Achievements
- Grid of achievement cards
- Locked: grayed out with lock icon
- Unlocked: full color with icon, name, description, date earned

#### Settings
- Sound effects volume slider (0-100)
- Music volume slider (0-100)
- Timer duration selector (1 min, 2 min, 3 min, 5 min)
- Minimum word length selector (3, 4, 5)
- Reset all data button with confirmation

---

## 7. Feature Specifications

### 7.1 Game Board

- Generate board from official dice set (see Section 2)
- Animated dice "roll" effect when starting a new round
- Visual letter tile interactions (hover glow, click feedback)
- "Qu" displayed as a single tile with slightly smaller text

### 7.2 Timer

- Configurable duration (default: 3 minutes)
- Circular progress ring that depletes clockwise
- Color transitions: Blue (>30s) → Amber (10-30s) → Red (<10s)
- Tick sound effect in last 10 seconds
- Pulsing animation in last 5 seconds

### 7.3 Word Submission

- Text input field with auto-focus
- Submit via Enter key or button click
- Real-time feedback:
  - Valid word: green flash + score popup animation + sound
  - Invalid word: red shake + sound
  - Duplicate: yellow flash + "Already found" message
- Word list panel shows all found words sorted by length (descending) then alphabetically

### 7.4 Board Solver

- After round ends, compute ALL valid words on the board
- Display words player found vs. words missed
- Show completion percentage
- Solver uses Trie + DFS with backtracking for efficiency
- Results are computed asynchronously to avoid UI freeze

### 7.5 High Scores

- Top 50 scores stored locally in SQLite
- Each entry: score, word count, longest word, date/time, board configuration
- Automatic check after each round
- New high score triggers celebration animation + sound
- Stored per timer-duration category (separate leaderboards)

### 7.6 Achievements

Complete list of achievements:

| ID | Name | Description | Condition |
|---|---|---|---|
| 1 | First Words | Complete your first round | Finish 1 round |
| 2 | Word Hoarder | Find 20 words in a single round | ≥20 words in one round |
| 3 | Linguist | Find 30 words in a single round | ≥30 words in one round |
| 4 | Centurion | Score 100+ points in a single round | ≥100 points |
| 5 | Double Century | Score 200+ points in a single round | ≥200 points |
| 6 | Long Word | Find a word with 7+ letters | Any 7+ letter word |
| 7 | Monster Word | Find a word with 8+ letters | Any 8+ letter word |
| 8 | Speed Demon | Find 10 words in the first minute | 10 words within 60s |
| 9 | Perfectionist | Find 50%+ of all possible words | ≥50% completion |
| 10 | Completionist | Find 75%+ of all possible words | ≥75% completion |
| 11 | Dedicated | Play 10 rounds | 10 lifetime rounds |
| 12 | Veteran | Play 50 rounds | 50 lifetime rounds |
| 13 | Marathon | Play 100 rounds | 100 lifetime rounds |
| 14 | High Roller | Achieve a new personal best score | Beat previous best |
| 15 | Quick Draw | Submit a valid word within 5 seconds of round start | Word within 5s |
| 16 | Vocabulary Builder | Find 500 unique words (lifetime) | 500 unique words total |
| 17 | Word Scholar | Find 1000 unique words (lifetime) | 1000 unique words total |
| 18 | Streak Master | Find 5 valid words in a row with no invalid submissions | 5 consecutive valid |
| 19 | No Mistakes | Complete a round with no invalid submissions | 0 invalid words |
| 20 | Qu Master | Find 3 words containing "Qu" in one round | 3 Qu-words |

### 7.7 Pause System

- Triggered by Escape key or pause button
- Immediately pauses the timer
- Applies a `BlurEffect` (radius 20px) to the game content
- Shows a semi-transparent overlay with pause menu
- Board letters are fully obscured to prevent memorization
- Resume restores exact game state and timer

### 7.8 Settings

- **Sound Effects Volume**: 0-100 slider (default: 80)
- **Music Volume**: 0-100 slider (default: 50)
- **Timer Duration**: 1, 2, 3, 5 minutes (default: 3)
- **Minimum Word Length**: 3, 4, 5 (default: 3)
- **Reset All Data**: Clears high scores, achievements, statistics (with confirmation)
- Settings persisted in SQLite immediately on change

---

## 8. Sound & Music Plan

### Sound Effects (WAV format, 44.1kHz, 16-bit)

All sound effects will be generated programmatically using NAudio's signal synthesis capabilities (sine waves, noise, envelopes) via a build-time PowerShell script. This ensures no external asset sourcing is needed.

| Sound | Description | Generation Approach |
|---|---|---|
| `tile_click.wav` | Short, soft click when hovering/selecting a tile | 800Hz sine, 50ms, fast decay |
| `word_valid.wav` | Pleasant ascending chime for valid word | Chord: C5+E5+G5, 300ms, bell envelope |
| `word_invalid.wav` | Soft descending buzz for invalid word | 200Hz→100Hz sweep, 200ms |
| `timer_tick.wav` | Subtle tick for last 10 seconds | 1000Hz sine, 30ms, sharp attack |
| `timer_warning.wav` | Urgent tone for last 5 seconds | 880Hz pulse, 100ms |
| `round_start.wav` | Bright ascending arpeggio | C4→E4→G4→C5, 80ms each |
| `round_end.wav` | Gentle descending tone | G4→E4→C4, 150ms each |
| `achievement_unlock.wav` | Triumphant fanfare | Major chord with sparkle overlay |
| `button_hover.wav` | Barely audible soft tick | 600Hz sine, 20ms, very low volume |
| `button_click.wav` | Crisp click | White noise burst, 30ms, bandpass filtered |
| `high_score.wav` | Celebratory ascending sequence | C4→E4→G4→C5→E5, with shimmer |
| `pause.wav` | Soft whoosh down | Filtered noise sweep down, 200ms |
| `resume.wav` | Soft whoosh up | Filtered noise sweep up, 200ms |

### Background Music

- A single ambient loop track (~2-3 minutes, seamlessly loopable)
- Style: Soothing, minimal, lo-fi ambient — soft pads, gentle piano-like tones
- Generated using NAudio synthesis: layered sine/triangle waves with slow LFO modulation, reverb-like delay
- Alternatively: Source a CC0/public domain ambient track from freesound.org or similar
- Fades in on game start, fades out on pause, cross-fades on loop point

### Audio Architecture

```
AudioManager (singleton)
├── MusicPlayer
│   ├── Volume control
│   ├── Fade in/out
│   ├── Loop with crossfade
│   └── Pause/resume
└── SoundEffectPlayer
    ├── Volume control
    ├── Fire-and-forget playback
    ├── Multiple simultaneous SFX
    └── Preloaded sound pool
```

---

## 9. Asset Generation Plan

All assets are generated or sourced without manual interaction:

### Word List
- **Source**: SCOWL (Spell Checker Oriented Word Lists) — open source, public domain
- **URL**: http://wordlist.aspell.net/
- **Processing**: Filter to words 3-16 characters, lowercase, alphabetic only, no proper nouns
- **Target size**: ~170,000-270,000 words
- **Build step**: PowerShell script downloads + processes into `english.txt`

### Font
- **Inter**: Free, open-source (SIL Open Font License)
- **Source**: https://github.com/rsms/inter/releases
- **Included as**: Embedded resource in the App project

### App Icon
- Generated programmatically via a build script or using WPF drawing APIs
- Design: A rounded square with gradient blue background, white "B" letter in a grid pattern
- Exported as `.ico` (multi-size) and `.png`

### Sound Effects
- Generated via `tools/generate-sounds.ps1` using NAudio synthesis
- All WAV files created at build/setup time
- No external audio files needed

### Background Music
- **Option A (preferred)**: Generate using NAudio — layered oscillators, LFO, delay
- **Option B**: Source a CC0 track from freesound.org (script auto-downloads)
- Script handles download, format conversion, and placement

---

## 10. Quality Gates & Linting

### Directory.Build.props (Applied to ALL Projects)

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net9.0-windows</TargetFramework>
    <LangVersion>13</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <WarningLevel>9999</WarningLevel>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
    <AnalysisLevel>latest-all</AnalysisLevel>
    <AnalysisMode>All</AnalysisMode>
    <EnableNETAnalyzers>true</EnableNETAnalyzers>
  </PropertyGroup>
</Project>
```

### Analyzer Packages (in Directory.Build.props)

```xml
<ItemGroup>
  <PackageReference Include="StyleCop.Analyzers" Version="1.2.0-beta.556" PrivateAssets="all" />
  <PackageReference Include="SonarAnalyzer.CSharp" Version="*" PrivateAssets="all" />
</ItemGroup>
```

### .editorconfig Rules (Key Selections)

```ini
# Naming conventions
dotnet_naming_rule.interface_should_begin_with_i.severity = error
dotnet_naming_rule.private_fields_should_begin_with_underscore.severity = error

# Code style
csharp_style_var_for_built_in_types = false:error
csharp_style_var_when_type_is_apparent = true:suggestion
csharp_style_expression_bodied_methods = when_on_single_line:suggestion
csharp_prefer_braces = true:error

# Formatting
csharp_new_line_before_open_brace = all
indent_size = 4
end_of_line = crlf
insert_final_newline = true

# SA rules
dotnet_diagnostic.SA1200.severity = none    # Using directives placement (allow outside namespace)
dotnet_diagnostic.SA1633.severity = none    # File header (not needed for personal project)
dotnet_diagnostic.SA1101.severity = none    # Prefix local calls with this

# All other SA rules = error
```

### Pre-Build Quality Checks

- **No suppressions allowed** without a comment explaining why
- All public APIs must have XML documentation comments
- Cyclomatic complexity limit: 10 per method (enforced via SonarAnalyzer)
- Maximum method length: 30 lines (guideline, enforced via code review)

---

## 11. Testing Strategy

### Coverage Target: ≥95% Line Coverage, ≥90% Branch Coverage

| Project | Strategy |
|---|---|
| **Boggle.Core** | 100% coverage — pure logic, fully testable, no external dependencies |
| **Boggle.Data** | 95%+ — use in-memory SQLite for integration tests |
| **Boggle.Audio** | 90%+ — mock NAudio interfaces, verify correct API calls |
| **Boggle.App** | 90%+ — test ViewModels thoroughly, converters, navigation; exclude XAML |

### Test Categories

```csharp
[Trait("Category", "Unit")]        // Fast, isolated, no I/O
[Trait("Category", "Integration")] // Uses SQLite, file system
[Trait("Category", "Performance")] // Benchmarks and perf assertions
```

### Key Test Cases

#### Board Generation
- All 16 dice are used exactly once
- All grid cells are populated
- Letters only come from valid die faces
- Shuffling produces different boards (statistical test)

#### Word Validation
- Valid 3-letter word on board → accepted
- Valid word not on board → rejected
- Word not in dictionary → rejected
- Path reuse → rejected
- "Qu" handling: word containing "QU" matches cell with "Qu"
- Case insensitivity
- Empty string → rejected
- Single character → rejected

#### Scoring
- 3-letter → 1 point
- 4-letter → 1 point
- 5-letter → 2 points
- 6-letter → 3 points
- 7-letter → 5 points
- 8-letter → 11 points
- 16-letter → 11 points (edge case)
- "Qu" counts as 2 letters for length

#### Board Solver
- Known board with known solution set → exact match
- Empty-ish board → minimal words
- Performance: solves any board in <500ms

#### Achievements
- Each achievement triggers correctly at its threshold
- Achievement doesn't trigger below threshold
- Achievement only unlocks once
- All 20 achievements are testable in isolation

#### ViewModel Tests
- Property changes raise `PropertyChanged`
- Commands execute correct service calls
- Timer decrement updates display
- Pause/resume preserves state

### Coverage Enforcement

```xml
<!-- In test project .csproj -->
<PropertyGroup>
  <CollectCoverage>true</CollectCoverage>
  <CoverletOutputFormat>cobertura</CoverletOutputFormat>
  <Threshold>95</Threshold>
  <ThresholdType>line</ThresholdType>
  <ThresholdStat>total</ThresholdStat>
</PropertyGroup>
```

---

## 12. Performance & Memory Guards

### Targets

| Metric | Target |
|---|---|
| **App startup** | < 2 seconds to interactive UI |
| **Board generation** | < 50ms |
| **Board solve** | < 500ms (worst case) |
| **Word validation** | < 1ms per word |
| **Dictionary load** | < 500ms (Trie construction) |
| **Memory (idle)** | < 80 MB |
| **Memory (peak game)** | < 150 MB |
| **UI frame rate** | Consistent 60fps during animations |

### Implementation

| Guard | Approach |
|---|---|
| **Dictionary** | Trie structure (memory-efficient prefix tree, lazy load on first game) |
| **Board solver** | DFS with Trie prefix pruning (cuts branches that can't form words) |
| **Audio** | Pre-load SFX into memory buffers at startup; dispose on shutdown |
| **SQLite** | Connection pooling, parameterized queries, async operations |
| **UI** | Virtualized lists for word display; no unnecessary re-renders |
| **Memory** | `IDisposable` on all resource-holding classes; weak event pattern for long-lived subscriptions |
| **Timer** | `DispatcherTimer` at 100ms interval (smooth display, low CPU) |
| **Async** | All I/O operations async; solver runs on `Task.Run` to avoid UI blocking |

### Performance Tests

```csharp
[Trait("Category", "Performance")]
public class PerformanceTests
{
    [Fact]
    public void BoardGeneration_ShouldCompleteWithin50ms();
    
    [Fact]
    public void BoardSolver_ShouldCompleteWithin500ms();
    
    [Fact]
    public void WordValidation_ShouldCompleteWithin1ms();
    
    [Fact]
    public void TrieConstruction_ShouldCompleteWithin500ms();
}
```

---

## 13. Error Handling & Logging

### Logging Strategy (Serilog)

| Level | Usage |
|---|---|
| **Verbose** | Detailed diagnostic (word validation paths, solver steps) |
| **Debug** | Development-time info (service calls, state transitions) |
| **Information** | Key events (round start, round end, achievement unlock, high score) |
| **Warning** | Recoverable issues (corrupt settings, missing audio file) |
| **Error** | Exceptions caught and handled (database write failure, file I/O) |
| **Fatal** | Unrecoverable (app crash, missing critical resources) |

### Sinks

| Sink | Configuration |
|---|---|
| **File** | Rolling daily, `logs/boggle-{Date}.log`, max 10 files, 10MB each |
| **Debug** | `System.Diagnostics.Debug.WriteLine` (dev only) |

### Error Handling Patterns

```csharp
// Service layer: catch, log, return result object
public Result<HighScoreEntry> TryRecordScore(int score)
{
    try { /* ... */ }
    catch (SqliteException ex)
    {
        _logger.Error(ex, "Failed to record high score {Score}", score);
        return Result<HighScoreEntry>.Failure("Could not save score");
    }
}

// ViewModel layer: display user-friendly messages
// App layer: global unhandled exception handler → log + graceful message
```

### Global Exception Handling

```csharp
// In App.xaml.cs
DispatcherUnhandledException += (s, e) =>
{
    Log.Fatal(e.Exception, "Unhandled exception");
    MessageBox.Show("An unexpected error occurred. The app will close.",
        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    e.Handled = true;
    Shutdown(1);
};

AppDomain.CurrentDomain.UnhandledException += (s, e) =>
{
    Log.Fatal((Exception)e.ExceptionObject, "Unhandled domain exception");
};

TaskScheduler.UnobservedTaskException += (s, e) =>
{
    Log.Error(e.Exception, "Unobserved task exception");
    e.SetObserved();
};
```

---

## 14. Milestones & Timeline

### Milestone 1: Foundation
**Status:** Complete ✅

- [x] **M1.1** Initialize .NET 9 solution with all projects
- [x] **M1.2** Configure `global.json`, `Directory.Build.props`, `.editorconfig`
- [x] **M1.3** Add all NuGet package references
- [x] **M1.4** Set up DI container in App.xaml.cs
- [x] **M1.5** Set up Serilog logging infrastructure
- [x] **M1.6** Create test projects with xUnit + coverlet
- [x] **M1.7** Verify build compiles with zero warnings
- [x] **M1.8** Verify `dotnet test` runs (117 tests pass)

### Milestone 2: Core Game Logic
**Status:** Complete ✅

- [x] **M2.1** Implement `Die`, `DiceSet`, `BoardCell`, `GameBoard` models
- [x] **M2.2** Implement `BoardGenerator` service
- [x] **M2.3** Implement `TrieDictionaryProvider` with word list loading
- [x] **M2.4** Implement `WordValidator` (dictionary + board path check)
- [x] **M2.5** Implement `BoardSolver` (find all words on board)
- [x] **M2.6** Implement `ScoringService`
- [x] **M2.7** Implement `GameEngine` (round lifecycle)
- [x] **M2.8** Write comprehensive tests for all Core services (target: 100%)
- [x] **M2.9** Source and integrate English word list

### Milestone 3: Data Layer
**Status:** Complete ✅

- [x] **M3.1** Set up SQLite database schema
- [x] **M3.2** Implement `BoggleDatabase` with migrations
- [x] **M3.3** Implement `HighScoreRepository`
- [x] **M3.4** Implement `AchievementRepository`
- [x] **M3.5** Implement `SettingsRepository`
- [x] **M3.6** Implement `StatisticsRepository`
- [x] **M3.7** Write integration tests with in-memory SQLite (target: 95%+)

### Milestone 4: UI Framework
**Status:** Complete ✅

- [x] **M4.1** Create theme resource dictionaries (Colors, Brushes, Typography, Controls)
- [x] **M4.2** Implement `ViewModelBase` (INPC, RelayCommand)
- [x] **M4.3** Implement `NavigationService`
- [x] **M4.4** Create `MainMenuView` + `MainMenuViewModel`
- [x] **M4.5** Create app shell with navigation frame
- [x] **M4.6** Write ViewModel tests (target: 90%+)

### Milestone 5: Game UI
**Status:** Complete ✅

- [x] **M5.1** Create `BoardGrid` and `BoardCell` custom controls
- [x] **M5.2** Create `TimerDisplay` (circular progress ring)
- [x] **M5.3** Create `WordInput` and `WordList` controls
- [x] **M5.4** Create `ScoreDisplay` control
- [x] **M5.5** Create `GameView` + `GameViewModel` (wire up game loop)
- [x] **M5.6** Implement word submission flow with validation feedback
- [x] **M5.7** Implement timer countdown with color transitions
- [x] **M5.8** Create `RoundResultsView` + `RoundResultsViewModel`
- [x] **M5.9** Write ViewModel + converter tests

### Milestone 6: Pause, High Scores, Achievements
**Status:** Complete ✅

- [x] **M6.1** Implement `PauseView` + `BlurOverlay` control
- [x] **M6.2** Implement `HighScoresView` + `HighScoresViewModel`
- [x] **M6.3** Implement `AchievementsView` + `AchievementsViewModel`
- [x] **M6.4** Implement `AchievementService` with all 20 achievements
- [x] **M6.5** Implement `AchievementPopup` toast notification
- [x] **M6.6** Wire high score recording into game flow
- [x] **M6.7** Write tests for achievement triggers + high score logic

### Milestone 7: Audio
**Status:** Complete ✅

- [x] **M7.1** Implement `AudioManager`, `SoundEffectPlayer`, `MusicPlayer`
- [x] **M7.2** Generate all sound effects via `generate-sounds.ps1`
- [x] **M7.3** Source or generate ambient background music loop
- [x] **M7.4** Integrate SFX into game events (valid word, invalid, timer, etc.)
- [x] **M7.5** Implement volume controls in Settings
- [x] **M7.6** Write audio service tests (mocked NAudio)

### Milestone 8: Settings & How to Play
**Status:** Complete ✅

- [x] **M8.1** Implement `SettingsView` + `SettingsViewModel`
- [x] **M8.2** Implement `HowToPlayView` + `HowToPlayViewModel`
- [x] **M8.3** Persist settings to SQLite
- [x] **M8.4** Apply settings (timer duration, min word length, volumes)
- [x] **M8.5** Write settings persistence tests

### Milestone 9: Polish & Animations
**Status:** Complete ✅

- [x] **M9.1** Implement board dice roll-in animation
- [x] **M9.2** Implement valid/invalid word feedback animations
- [x] **M9.3** Implement achievement unlock popup animation
- [x] **M9.4** Implement high score celebration animation
- [x] **M9.5** Implement view transition animations
- [x] **M9.6** Timer pulsing in last 5 seconds
- [x] **M9.7** Button hover/press micro-interactions

### Milestone 10: Final Quality Pass
**Status:** Complete ✅

- [x] **M10.1** Run full test suite — verify ≥95% line coverage
- [x] **M10.2** Run all analyzers — verify zero warnings
- [x] **M10.3** Performance profiling — verify all targets met
- [x] **M10.4** Memory leak check — run 10 consecutive rounds, verify stable allocation
- [x] **M10.5** Generate app icon
- [x] **M10.6** Update README with screenshots and build instructions
- [x] **M10.7** Final CHANGELOG update

---

## 15. Dictionary / Word List

### Source Selection

**Primary**: TWL (Tournament Word List) or SOWPODS — the standard Scrabble dictionaries which work perfectly for Boggle.

**Alternative**: SCOWL (Spell Checker Oriented Word Lists) filtered to common English words.

### Processing Rules

1. Convert all words to lowercase
2. Remove words shorter than 3 characters
3. Remove words longer than 16 characters (max possible Boggle word)
4. Remove words containing non-alphabetic characters (hyphens, apostrophes, spaces)
5. Remove proper nouns (words that only appear capitalized)
6. Remove abbreviations and acronyms
7. Deduplicate
8. Sort alphabetically
9. Store as one word per line in plain text

### Integration

- Word list is embedded as a resource in `Boggle.Core`
- Loaded once at game launch into a Trie data structure
- Trie enables O(k) lookup where k = word length
- Trie prefix checks enable efficient board solver pruning

---

*Document Version: 1.0*  
*Created: 2026-04-06*  
*Last Updated: 2026-04-06*
