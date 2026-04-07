# Boggle Project Scaffold Script
# Creates the .NET solution, all projects, references, and NuGet packages

$ErrorActionPreference = "Stop"
Set-Location "c:\Users\Randy\Coding\boggle"

Write-Host "=== Creating Solution ===" -ForegroundColor Cyan
dotnet new sln -n Boggle --force

Write-Host "=== Creating Source Projects ===" -ForegroundColor Cyan

# Core library (no WPF)
dotnet new classlib -n Boggle.Core -o src/Boggle.Core --framework net9.0-windows --force
# Data library (no WPF)
dotnet new classlib -n Boggle.Data -o src/Boggle.Data --framework net9.0-windows --force
# Audio library (no WPF)
dotnet new classlib -n Boggle.Audio -o src/Boggle.Audio --framework net9.0-windows --force
# WPF App
dotnet new wpf -n Boggle.App -o src/Boggle.App --framework net9.0-windows --force

Write-Host "=== Creating Test Projects ===" -ForegroundColor Cyan
dotnet new xunit -n Boggle.Core.Tests -o tests/Boggle.Core.Tests --framework net9.0-windows --force
dotnet new xunit -n Boggle.Data.Tests -o tests/Boggle.Data.Tests --framework net9.0-windows --force
dotnet new xunit -n Boggle.Audio.Tests -o tests/Boggle.Audio.Tests --framework net9.0-windows --force
dotnet new xunit -n Boggle.App.Tests -o tests/Boggle.App.Tests --framework net9.0-windows --force

Write-Host "=== Adding Projects to Solution ===" -ForegroundColor Cyan
dotnet sln add src/Boggle.Core/Boggle.Core.csproj
dotnet sln add src/Boggle.Data/Boggle.Data.csproj
dotnet sln add src/Boggle.Audio/Boggle.Audio.csproj
dotnet sln add src/Boggle.App/Boggle.App.csproj
dotnet sln add tests/Boggle.Core.Tests/Boggle.Core.Tests.csproj
dotnet sln add tests/Boggle.Data.Tests/Boggle.Data.Tests.csproj
dotnet sln add tests/Boggle.Audio.Tests/Boggle.Audio.Tests.csproj
dotnet sln add tests/Boggle.App.Tests/Boggle.App.Tests.csproj

Write-Host "=== Adding Project References ===" -ForegroundColor Cyan
# App references Core, Data, Audio
dotnet add src/Boggle.App/Boggle.App.csproj reference src/Boggle.Core/Boggle.Core.csproj
dotnet add src/Boggle.App/Boggle.App.csproj reference src/Boggle.Data/Boggle.Data.csproj
dotnet add src/Boggle.App/Boggle.App.csproj reference src/Boggle.Audio/Boggle.Audio.csproj

# Data references Core
dotnet add src/Boggle.Data/Boggle.Data.csproj reference src/Boggle.Core/Boggle.Core.csproj

# Test project references
dotnet add tests/Boggle.Core.Tests/Boggle.Core.Tests.csproj reference src/Boggle.Core/Boggle.Core.csproj
dotnet add tests/Boggle.Data.Tests/Boggle.Data.Tests.csproj reference src/Boggle.Data/Boggle.Data.csproj
dotnet add tests/Boggle.Data.Tests/Boggle.Data.Tests.csproj reference src/Boggle.Core/Boggle.Core.csproj
dotnet add tests/Boggle.Audio.Tests/Boggle.Audio.Tests.csproj reference src/Boggle.Audio/Boggle.Audio.csproj
dotnet add tests/Boggle.App.Tests/Boggle.App.Tests.csproj reference src/Boggle.App/Boggle.App.csproj
dotnet add tests/Boggle.App.Tests/Boggle.App.Tests.csproj reference src/Boggle.Core/Boggle.Core.csproj

Write-Host "=== Adding NuGet Packages to Source Projects ===" -ForegroundColor Cyan

# Core packages
dotnet add src/Boggle.Core/Boggle.Core.csproj package Microsoft.Extensions.DependencyInjection.Abstractions
dotnet add src/Boggle.Core/Boggle.Core.csproj package Microsoft.Extensions.Logging.Abstractions
dotnet add src/Boggle.Core/Boggle.Core.csproj package System.Text.Json

# Data packages
dotnet add src/Boggle.Data/Boggle.Data.csproj package Microsoft.Data.Sqlite
dotnet add src/Boggle.Data/Boggle.Data.csproj package Microsoft.Extensions.Logging.Abstractions

# Audio packages
dotnet add src/Boggle.Audio/Boggle.Audio.csproj package NAudio
dotnet add src/Boggle.Audio/Boggle.Audio.csproj package Microsoft.Extensions.Logging.Abstractions

# App packages
dotnet add src/Boggle.App/Boggle.App.csproj package Microsoft.Extensions.DependencyInjection
dotnet add src/Boggle.App/Boggle.App.csproj package Microsoft.Extensions.Configuration
dotnet add src/Boggle.App/Boggle.App.csproj package Microsoft.Extensions.Configuration.Json
dotnet add src/Boggle.App/Boggle.App.csproj package Serilog
dotnet add src/Boggle.App/Boggle.App.csproj package Serilog.Sinks.File
dotnet add src/Boggle.App/Boggle.App.csproj package Serilog.Sinks.Debug
dotnet add src/Boggle.App/Boggle.App.csproj package Serilog.Extensions.Logging

Write-Host "=== Adding NuGet Packages to Test Projects ===" -ForegroundColor Cyan

# Test packages for all test projects
$testProjects = @(
    "tests/Boggle.Core.Tests/Boggle.Core.Tests.csproj",
    "tests/Boggle.Data.Tests/Boggle.Data.Tests.csproj",
    "tests/Boggle.Audio.Tests/Boggle.Audio.Tests.csproj",
    "tests/Boggle.App.Tests/Boggle.App.Tests.csproj"
)

foreach ($proj in $testProjects) {
    dotnet add $proj package Moq
    dotnet add $proj package FluentAssertions
    dotnet add $proj package coverlet.collector
}

# Data tests need SQLite
dotnet add tests/Boggle.Data.Tests/Boggle.Data.Tests.csproj package Microsoft.Data.Sqlite

Write-Host "=== Scaffold Complete ===" -ForegroundColor Green
