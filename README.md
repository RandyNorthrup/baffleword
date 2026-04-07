# Boggle — Single Player Word Game

A polished WPF desktop Boggle game built with .NET 9, featuring a clean MVVM architecture, animations, audio, achievements, and persistent high scores.

## Features

- **Classic Boggle gameplay** — Find words on a 4×4 letter grid by connecting adjacent tiles
- **NWL2020 dictionary** — Official North American Scrabble tournament word list (191,745 words)
- **Trie-based word validation** — Fast prefix and full-word lookups
- **Board solver** — Finds all possible words on any board via DFS traversal
- **Configurable settings** — Timer duration (60–600s), minimum word length (3–5), SFX/music volume
- **High scores** — Persistent SQLite leaderboard with top 50 scores
- **20 achievements** — Unlock milestones like First Words, Centurion, Monster Word, Streak Master
- **Audio system** — 13 sound effects + ambient music loop via NAudio
- **Animations** — Board roll-in, timer pulse, word feedback, view transitions, button micro-interactions
- **Pause/resume** — Pause mid-round without losing progress

## Scoring

| Word Length | Points |
|-------------|--------|
| 3–4 letters | 1      |
| 5 letters   | 2      |
| 6 letters   | 3      |
| 7 letters   | 5      |
| 8+ letters  | 11     |

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (9.0.300+)
- Windows 10/11 (WPF)

## Build & Run

```bash
# Clone the repository
git clone https://github.com/your-username/boggle.git
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

242 tests across 4 test projects:
- **Boggle.Core.Tests** (139) — Game engine, board generation, scoring, dictionary, achievements, statistics
- **Boggle.App.Tests** (78) — ViewModels, navigation, settings, commands
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
