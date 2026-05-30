# Changelog

All notable changes to Baffleword are documented here.

## 1.5.7 - 2026-05-30

- Made the release workflow cross-platform: it now publishes self-contained Linux x64, macOS x64, and macOS arm64 archives alongside the signed Velopack Windows installer.
- Clarified the README description as a parody clone of Boggle® and added a trademark disclaimer.
- Removed a stale `scripts/generate-sounds.ps1` reference from the README project structure.

## 1.5.6

- Renamed public app identity, projects, namespaces, release artifacts, and docs to Baffleword.
- Renamed larger game modes to Big Board and Super Board.
- Added regression guard coverage to prevent legacy brand text from returning to source files.
- Kept cross-platform Avalonia UI, SoundFlow audio, .NET 10 build, strict analyzers, package audit, and Velopack Windows installer flow.
