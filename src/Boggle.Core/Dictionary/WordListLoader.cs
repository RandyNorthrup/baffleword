// <copyright file="WordListLoader.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Dictionary;

using Microsoft.Extensions.Logging;

/// <summary>
/// Loads word lists from embedded resources or files.
/// </summary>
public sealed class WordListLoader
{
    private readonly ILogger<WordListLoader> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WordListLoader"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public WordListLoader(ILogger<WordListLoader> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Loads words from a stream (one word per line).
    /// </summary>
    /// <param name="stream">The stream containing the word list.</param>
    /// <returns>An enumerable of words.</returns>
    public IEnumerable<string> LoadFromStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        _logger.LogDebug("Loading word list from stream");
        var words = new List<string>();

        using var reader = new StreamReader(stream);
        while (reader.ReadLine() is { } line)
        {
            string trimmed = line.Trim();
            if (trimmed.Length >= 3 && trimmed.All(char.IsLetter))
            {
                words.Add(trimmed);
            }
        }

        _logger.LogInformation("Loaded {Count} words from word list", words.Count);
        return words;
    }

    /// <summary>
    /// Loads words from a file path (one word per line).
    /// </summary>
    /// <param name="filePath">The path to the word list file.</param>
    /// <returns>An enumerable of words.</returns>
    public IEnumerable<string> LoadFromFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        _logger.LogDebug("Loading word list from file: {FilePath}", filePath);
        using FileStream stream = File.OpenRead(filePath);
        return LoadFromStream(stream);
    }
}
