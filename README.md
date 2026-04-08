# Boggle — Single Player Word Game

A polished WPF desktop Boggle game built with .NET 9, featuring three game modes, MVVM architecture, drag-to-select, animations, audio, achievements, and persistent high scores.

## Features

- **Three game modes** — Standard (4×4), Big Boggle (5×5), and Super Big Boggle (6×6 with blocked tiles and digraph dice)
- **Drag-to-select** — Click and drag across adjacent tiles to form words, with visual path lines
- **NWL2020 dictionary** — 191,745 words from the official North American Scrabble tournament word list, with Trie-based prefix and full-word lookups
- **Board solver** — Finds all possible words on any board via DFS traversal
- **High scores** — Persistent SQLite leaderboard with top 50 scores per game mode
- **26 achievements** — Unlock milestones across all three game modes
- **Audio system** — 14 sound effects + ambient music loop via NAudio
- **Animations** — Board roll-in, timer pulse, word feedback, view transitions, button micro-interactions
- **Pause/resume** — Pause mid-round without losing progress
- **Settings** — Adjustable SFX and music volume with mute toggle

## Game Modes

| Mode | Grid | Min Word Length | Timer | Notes |
|------|------|----------------|-------|-------|
| Standard | 4×4 | 3 letters | 3 min | Classic Boggle |
| Big Boggle | 5×5 | 4 letters | 3 min | Larger grid, longer words |
| Super Big Boggle | 6×6 | 4 letters | 4 min | Blocked tiles, digraph dice |

## Scoring

### Standard & Big Boggle

| Word Length | Points |
|-------------|--------|
| 3–4 letters | 1      |
| 5 letters   | 2      |
| 6 letters   | 3      |
| 7 letters   | 5      |
| 8+ letters  | 11     |

### Super Big Boggle

| Word Length | Points |
|-------------|--------|
| 4 letters   | 1      |
| 5 letters   | 2      |
| 6 letters   | 3      |
| 7 letters   | 5      |
| 8 letters   | 11     |
| 9+ letters  | 2 per letter |

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (9.0.300+)
- Windows 10/11 (WPF)

## Build & Run

```bash
# Clone the repository
git clone https://github.com/RandyNorthrup/boggle.git
cd boggle

# Build
dotnet build

# Run
dotnet run --project src/Boggle.App
```

## Run Tests

```bash
dotnet test
```

324 tests across 4 test projects:
- **Boggle.Core.Tests** (207) — Game engine, board generation, scoring, dictionary, achievements, statistics
- **Boggle.App.Tests** (92) — ViewModels, navigation, converters, commands
- **Boggle.Data.Tests** (20) — SQLite repositories, settings persistence
- **Boggle.Audio.Tests** (5) — Audio manager interface tests

## Project Structure

```
src/
  Boggle.Core/       Core game logic, models, services, dictionary
  Boggle.App/        WPF application, views, view models, themes
  Boggle.Data/       SQLite data layer, repositories
  Boggle.Audio/      NAudio-based audio system
tests/
  Boggle.Core.Tests/
  Boggle.App.Tests/
  Boggle.Data.Tests/
  Boggle.Audio.Tests/
scripts/
  generate-sounds.ps1   PowerShell script to generate WAV sound effects
```

## Architecture

- **MVVM** — ViewModelBase with INotifyPropertyChanged, RelayCommand, implicit DataTemplates
- **DI** — Microsoft.Extensions.DependencyInjection for all services and ViewModels
- **Navigation** — NavigationService resolves ViewModels from DI, ContentControl-based view switching
- **Data** — SQLite via Microsoft.Data.Sqlite for high scores, achievements, statistics, settings
- **Dictionary** — Trie data structure loaded from embedded NWL2020 word list resource
- **Audio** — ISoundEffectPlayer (preloaded WAV buffers, fire-and-forget) + IMusicPlayer (looping)

## Tech Stack

- .NET 9 / C# 13
- WPF (Windows Presentation Foundation)
- SQLite (Microsoft.Data.Sqlite)
- NAudio (audio playback)
- xUnit + Moq + FluentAssertions (testing)
- StyleCop + SonarAnalyzer + .NET Analyzers (code quality)

## License

MIT
