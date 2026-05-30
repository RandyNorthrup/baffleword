// <copyright file="WordResult.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Models;

/// <summary>
/// Represents the result of validating a submitted word.
/// </summary>
public sealed class WordResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WordResult"/> class.
    /// </summary>
    /// <param name="word">The submitted word.</param>
    /// <param name="status">The validation status.</param>
    /// <param name="points">Points awarded (0 if invalid).</param>
    public WordResult(string word, WordStatus status, int points)
    {
        Word = word;
        Status = status;
        Points = points;
        SubmittedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the submitted word.
    /// </summary>
    public string Word { get; }

    /// <summary>
    /// Gets the validation status.
    /// </summary>
    public WordStatus Status { get; }

    /// <summary>
    /// Gets the points awarded for this word.
    /// </summary>
    public int Points { get; }

    /// <summary>
    /// Gets the UTC time when this word was submitted.
    /// </summary>
    public DateTime SubmittedAt { get; }

    /// <summary>
    /// Gets the length of the submitted word.
    /// </summary>
    public int WordLength => Word.Length;
}
