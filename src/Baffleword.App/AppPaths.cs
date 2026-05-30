// <copyright file="AppPaths.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App;

using System.IO;

/// <summary>
/// Resolves writable application paths for installed desktop builds.
/// </summary>
public static class AppPaths
{
    private const string AppDirectoryName = "Baffleword";
    private const string DatabaseFileName = "baffleword.db";
    private const string LogsDirectoryName = "Logs";

    /// <summary>
    /// Gets the per-user writable application data directory.
    /// </summary>
    /// <returns>The local application data directory for Baffleword.</returns>
    public static string GetApplicationDataDirectory()
    {
        return Path.Combine(GetRequiredLocalApplicationDataPath(), AppDirectoryName);
    }

    /// <summary>
    /// Gets the SQLite database path.
    /// </summary>
    /// <returns>The full path to the Baffleword SQLite database file.</returns>
    public static string GetDatabasePath()
    {
        return Path.Combine(GetApplicationDataDirectory(), DatabaseFileName);
    }

    /// <summary>
    /// Gets the log directory path.
    /// </summary>
    /// <returns>The per-user writable log directory.</returns>
    public static string GetLogDirectory()
    {
        return Path.Combine(GetApplicationDataDirectory(), LogsDirectoryName);
    }

    private static string GetRequiredLocalApplicationDataPath()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException("Local application data folder is not available.");
        }

        return path;
    }
}
