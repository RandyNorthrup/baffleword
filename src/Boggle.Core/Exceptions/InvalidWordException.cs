// <copyright file="InvalidWordException.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Exceptions;

/// <summary>
/// Thrown when a word fails validation checks.
/// </summary>
public sealed class InvalidWordException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidWordException"/> class.
    /// </summary>
    public InvalidWordException()
    {
        Word = string.Empty;
        Reason = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidWordException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public InvalidWordException(string message)
        : base(message)
    {
        Word = string.Empty;
        Reason = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidWordException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidWordException(string message, Exception innerException)
        : base(message, innerException)
    {
        Word = string.Empty;
        Reason = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidWordException"/> class.
    /// </summary>
    /// <param name="word">The invalid word.</param>
    /// <param name="reason">The reason the word is invalid.</param>
    public InvalidWordException(string word, string reason)
        : base($"Invalid word '{word}': {reason}")
    {
        Word = word;
        Reason = reason;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidWordException"/> class.
    /// </summary>
    /// <param name="word">The invalid word.</param>
    /// <param name="reason">The reason the word is invalid.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidWordException(string word, string reason, Exception innerException)
        : base($"Invalid word '{word}': {reason}", innerException)
    {
        Word = word;
        Reason = reason;
    }

    /// <summary>
    /// Gets the invalid word.
    /// </summary>
    public string Word { get; }

    /// <summary>
    /// Gets the reason the word is invalid.
    /// </summary>
    public string Reason { get; }
}
