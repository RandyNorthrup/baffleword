// <copyright file="AppPathsTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.Tests;

using System.IO;
using Baffleword.App;
using FluentAssertions;
using Xunit;

public sealed class AppPathsTests
{
    [Fact]
    public void GetApplicationDataDirectory_UsesWritableLocalAppDataFolder()
    {
        string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        string path = AppPaths.GetApplicationDataDirectory();

        path.Should().Be(Path.Combine(localApplicationData, "Baffleword"));
    }

    [Fact]
    public void GetDatabasePath_UsesApplicationDataDirectory()
    {
        string path = AppPaths.GetDatabasePath();

        path.Should().Be(Path.Combine(AppPaths.GetApplicationDataDirectory(), "baffleword.db"));
    }

    [Fact]
    public void GetLogDirectory_UsesApplicationDataDirectory()
    {
        string path = AppPaths.GetLogDirectory();

        path.Should().Be(Path.Combine(AppPaths.GetApplicationDataDirectory(), "Logs"));
    }
}
