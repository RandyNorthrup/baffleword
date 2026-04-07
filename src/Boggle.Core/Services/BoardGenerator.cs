// <copyright file="BoardGenerator.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.Core.Services;

using Boggle.Core.Models;
using Microsoft.Extensions.Logging;

/// <summary>
/// Generates Boggle game boards using Fisher-Yates shuffle and official dice.
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
    public GameBoard Generate()
    {
        _logger.LogDebug("Generating new game board");

        // Copy dice list and shuffle using Fisher-Yates
        Die[] dice = DiceSet.StandardDice.ToArray();
        FisherYatesShuffle(dice);

        // Roll each die and place in grid
        BoardCell[][] cells = new BoardCell[GameBoard.Rows][];
        for (int row = 0; row < GameBoard.Rows; row++)
        {
            cells[row] = new BoardCell[GameBoard.Columns];
        }

        for (int i = 0; i < GameBoard.TotalCells; i++)
        {
            int row = i / GameBoard.Columns;
            int col = i % GameBoard.Columns;
            string letter = dice[i].Roll(_random);
            cells[row][col] = new BoardCell(letter, row, col);
        }

        _logger.LogDebug("Board generated successfully");
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
