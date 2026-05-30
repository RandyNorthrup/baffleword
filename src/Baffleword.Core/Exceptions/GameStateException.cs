// <copyright file="GameStateException.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Exceptions;

/// <summary>
/// Thrown when an operation is invalid for the current game state.
/// </summary>
public sealed class GameStateException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameStateException"/> class.
    /// </summary>
    public GameStateException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameStateException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public GameStateException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameStateException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public GameStateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
