// <copyright file="GameRoundState.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Models;

/// <summary>
/// The state of a game round.
/// </summary>
public enum GameRoundState
{
    /// <summary>
    /// The round is currently being played.
    /// </summary>
    Playing,

    /// <summary>
    /// The round is paused.
    /// </summary>
    Paused,

    /// <summary>
    /// The round has ended (timer expired or manually ended).
    /// </summary>
    Ended,
}
