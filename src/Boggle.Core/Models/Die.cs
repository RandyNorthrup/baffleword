// <copyright file="Die.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.Core.Models;

/// <summary>
/// Represents a single Boggle die with 6 faces.
/// </summary>
public sealed class Die
{
    private readonly string[] _faces;

    /// <summary>
    /// Initializes a new instance of the <see cref="Die"/> class.
    /// </summary>
    /// <param name="faces">The 6 face values of this die.</param>
    public Die(params string[] faces)
    {
        ArgumentNullException.ThrowIfNull(faces);

        if (faces.Length != 6)
        {
            throw new ArgumentException("A die must have exactly 6 faces.", nameof(faces));
        }

        _faces = faces.Select(f => f.ToUpperInvariant()).ToArray();
    }

    /// <summary>
    /// Gets the face values of this die.
    /// </summary>
    public IReadOnlyList<string> Faces => _faces;

    /// <summary>
    /// Rolls this die using the provided random number generator and returns the selected face.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    /// <returns>The randomly selected face value.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5394:Do not use insecure randomness", Justification = "Game dice roll does not require cryptographic randomness.")]
    public string Roll(Random random)
    {
        ArgumentNullException.ThrowIfNull(random);
        return _faces[random.Next(_faces.Length)];
    }
}
