// <copyright file="BrandingGuardTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.Core.Tests.Quality;

using FluentAssertions;
using Xunit;

public sealed class BrandingGuardTests
{
    private static readonly string _legacyBrand = string.Concat('B', 'o', 'g', 'g', 'l', 'e');

    // Documentation references the legacy-brand trademark nominatively to describe the
    // genre this parody clone imitates. Those references are intentional and legally
    // necessary, so these files are exempt from the legacy-brand guard. The guard
    // still protects all code, project, and identity files.
    private static readonly string[] _allowedBrandReferences =
    [
        "README.md",
        "CHANGELOG.md",
    ];

    private static readonly string[] _textExtensions =
    [
        ".axaml",
        ".cs",
        ".csproj",
        ".editorconfig",
        ".gitignore",
        ".json",
        ".md",
        ".props",
        ".sln",
        ".targets",
        ".yaml",
        ".yml",
    ];

    [Fact]
    public void RepositoryText_DoesNotContainLegacyBrand()
    {
        string repositoryRoot = FindRepositoryRoot();

        string[] offenders = Directory
            .EnumerateFiles(repositoryRoot, "*", SearchOption.AllDirectories)
            .Where(path => IsTextFile(path) && !IsIgnoredPath(repositoryRoot, path) && !IsAllowedBrandReference(repositoryRoot, path))
            .Where(path => File.ReadAllText(path).Contains(_legacyBrand, StringComparison.OrdinalIgnoreCase))
            .Select(path => Path.GetRelativePath(repositoryRoot, path))
            .Order(StringComparer.Ordinal)
            .ToArray();

        offenders.Should().BeEmpty();
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? current = new(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Baffleword.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not locate repository root.");
    }

    private static bool IsTextFile(string path)
    {
        string extension = Path.GetExtension(path);
        return _textExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsAllowedBrandReference(string repositoryRoot, string path)
    {
        string relativePath = Path.GetRelativePath(repositoryRoot, path);
        return _allowedBrandReferences.Contains(relativePath, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsIgnoredPath(string repositoryRoot, string path)
    {
        string relativePath = Path.GetRelativePath(repositoryRoot, path);
        char[] separators = [Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar];
        string[] parts = relativePath.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        return parts.Contains(".git", StringComparer.Ordinal)
            || parts.Contains("bin", StringComparer.Ordinal)
            || parts.Contains("obj", StringComparer.Ordinal);
    }
}
