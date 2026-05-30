// <copyright file="BoardGenerator.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Services;

using Baffleword.Core.Models;
using Microsoft.Extensions.Logging;

/// <summary>
/// Generates Baffleword game boards using Fisher-Yates shuffle.
/// </summary>
public sealed class BoardGenerator : IBoardGenerator
{
    private readonly Random _random;
    private readonly ILogger<BoardGenerator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoardGenerator"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public BoardGenerator(ILogger<BoardGenerator> logger)
        : this(logger, new Random())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoardGenerator"/> class with a specific random source.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="random">The random number generator to use.</param>
    public BoardGenerator(ILogger<BoardGenerator> logger, Random random)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Game board generation does not require cryptographic randomness.")]
    public GameBoard Generate(GameMode mode = GameMode.Standard)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Generating new {Mode} game board", mode);
        }

        GameModeConfig config = GameModeConfig.ForMode(mode);
        int gridSize = config.GridSize;

        // Copy dice list and shuffle using Fisher-Yates
        Die[] dice = config.Dice.ToArray();
        FisherYatesShuffle(dice);

        // Roll each die and place in grid
        BoardCell[][] cells = new BoardCell[gridSize][];
        for (int row = 0; row < gridSize; row++)
        {
            cells[row] = new BoardCell[gridSize];
        }

        for (int i = 0; i < gridSize * gridSize; i++)
        {
            int row = i / gridSize;
            int col = i % gridSize;
            string face = dice[i].Roll(_random);
            bool isBlocked = face.Length == 0;
            cells[row][col] = new BoardCell(face, row, col, isBlocked);
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Board generated successfully ({GridSize}x{GridSize2})", gridSize, gridSize);
        }

        return new GameBoard(cells);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Game board generation does not require cryptographic randomness.")]
    private void FisherYatesShuffle(Die[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
