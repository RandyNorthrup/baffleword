// <copyright file="WordStatus.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// The validation status of a submitted word.
/// </summary>
public enum WordStatus
{
    /// <summary>
    /// The word is valid and scores points.
    /// </summary>
    Valid,

    /// <summary>
    /// The word is not in the dictionary.
    /// </summary>
    NotInDictionary,

    /// <summary>
    /// The word cannot be traced on the board.
    /// </summary>
    NotOnBoard,

    /// <summary>
    /// The word is too short (less than minimum length).
    /// </summary>
    TooShort,

    /// <summary>
    /// The word has already been submitted this round.
    /// </summary>
    AlreadyFound,
}
