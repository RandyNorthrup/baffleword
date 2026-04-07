# generate-wordlist.ps1
# Downloads and processes a comprehensive English word list for Boggle.
# Filters to words 3-16 characters, alphabetic only, lowercase.
# Source: dwyl/english-words (public domain, ~370K words).

param(
    [string]$OutputPath = "$PSScriptRoot\src\Boggle.App\Assets\WordLists\english.txt",
    [int]$MinLength = 3,
    [int]$MaxLength = 16
)

$ErrorActionPreference = 'Stop'

$url = 'https://raw.githubusercontent.com/dwyl/english-words/master/words_alpha.txt'

Write-Host "Downloading word list from $url ..."
$raw = Invoke-WebRequest -Uri $url -UseBasicParsing
$words = $raw.Content -split "`r?`n"

Write-Host "Filtering $($words.Count) words (length $MinLength-$MaxLength, alphabetic only) ..."
$filtered = $words |
    ForEach-Object { $_.Trim().ToLowerInvariant() } |
    Where-Object { $_.Length -ge $MinLength -and $_.Length -le $MaxLength -and $_ -match '^[a-z]+$' } |
    Sort-Object -Unique

$dir = Split-Path $OutputPath -Parent
if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }

$filtered | Set-Content -Path $OutputPath -Encoding UTF8

Write-Host "Wrote $($filtered.Count) words to $OutputPath"
