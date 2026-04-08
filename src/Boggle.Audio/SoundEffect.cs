// <copyright file="SoundEffect.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Audio;

/// <summary>
/// Enumeration of all available sound effects.
/// </summary>
public enum SoundEffect
{
    /// <summary>Soft click when hovering/selecting a tile.</summary>
    TileClick,

    /// <summary>Pleasant chime for valid word.</summary>
    WordValid,

    /// <summary>Soft buzz for invalid word.</summary>
    WordInvalid,

    /// <summary>Subtle tick for last 10 seconds.</summary>
    TimerTick,

    /// <summary>Urgent tone for last 5 seconds.</summary>
    TimerWarning,

    /// <summary>Bright ascending arpeggio.</summary>
    RoundStart,

    /// <summary>Gentle descending tone.</summary>
    RoundEnd,

    /// <summary>Triumphant fanfare.</summary>
    AchievementUnlock,

    /// <summary>Barely audible soft tick on button hover.</summary>
    ButtonHover,

    /// <summary>Crisp click on button press.</summary>
    ButtonClick,

    /// <summary>Soft whoosh down on pause.</summary>
    Pause,

    /// <summary>Soft whoosh up on resume.</summary>
    Resume,
}
