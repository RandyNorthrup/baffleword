# generate-sounds.ps1
# Generates simple WAV sound effects for the Boggle game using sine wave synthesis.
# Output: src/Boggle.App/Assets/Sounds/*.wav and src/Boggle.App/Assets/Music/ambient.wav

param(
    [string]$OutputSoundsDir = "$PSScriptRoot\..\src\Boggle.App\Assets\Sounds",
    [string]$OutputMusicDir  = "$PSScriptRoot\..\src\Boggle.App\Assets\Music"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function New-WavFile {
    param(
        [string]$Path,
        [int]$SampleRate = 44100,
        [int16[]]$Samples
    )

    $dataSize = $Samples.Length * 2  # 16-bit = 2 bytes per sample
    $fileSize = 36 + $dataSize

    $stream = [System.IO.File]::Create($Path)
    $writer = [System.IO.BinaryWriter]::new($stream)

    try {
        # RIFF header
        $writer.Write([System.Text.Encoding]::ASCII.GetBytes("RIFF"))
        $writer.Write([int]$fileSize)
        $writer.Write([System.Text.Encoding]::ASCII.GetBytes("WAVE"))

        # fmt chunk
        $writer.Write([System.Text.Encoding]::ASCII.GetBytes("fmt "))
        $writer.Write([int]16)         # chunk size
        $writer.Write([int16]1)        # PCM format
        $writer.Write([int16]1)        # mono
        $writer.Write([int]$SampleRate)
        $writer.Write([int]($SampleRate * 2))  # byte rate
        $writer.Write([int16]2)        # block align
        $writer.Write([int16]16)       # bits per sample

        # data chunk
        $writer.Write([System.Text.Encoding]::ASCII.GetBytes("data"))
        $writer.Write([int]$dataSize)

        foreach ($sample in $Samples) {
            $writer.Write($sample)
        }
    }
    finally {
        $writer.Close()
        $stream.Close()
    }
}

function Get-SineSamples {
    param(
        [double]$Frequency,
        [double]$DurationMs,
        [double]$Amplitude = 0.5,
        [int]$SampleRate = 44100,
        [switch]$FadeOut,
        [switch]$FadeIn
    )

    $numSamples = [int]($SampleRate * $DurationMs / 1000.0)
    $samples = [int16[]]::new($numSamples)

    for ($i = 0; $i -lt $numSamples; $i++) {
        $t = $i / [double]$SampleRate
        $value = [Math]::Sin(2.0 * [Math]::PI * $Frequency * $t) * $Amplitude

        if ($FadeOut) {
            $value *= (1.0 - ($i / [double]$numSamples))
        }
        if ($FadeIn) {
            $fadeLen = [Math]::Min($i / [double]$numSamples * 10.0, 1.0)
            $value *= $fadeLen
        }

        $samples[$i] = [int16]([Math]::Max(-32767, [Math]::Min(32767, $value * 32767)))
    }

    return $samples
}

function Get-FrequencySweep {
    param(
        [double]$StartFreq,
        [double]$EndFreq,
        [double]$DurationMs,
        [double]$Amplitude = 0.4,
        [int]$SampleRate = 44100
    )

    $numSamples = [int]($SampleRate * $DurationMs / 1000.0)
    $samples = [int16[]]::new($numSamples)

    for ($i = 0; $i -lt $numSamples; $i++) {
        $progress = $i / [double]$numSamples
        $freq = $StartFreq + ($EndFreq - $StartFreq) * $progress
        $t = $i / [double]$SampleRate
        $fade = 1.0 - ($progress * 0.5)  # gentle fade
        $value = [Math]::Sin(2.0 * [Math]::PI * $freq * $t) * $Amplitude * $fade
        $samples[$i] = [int16]([Math]::Max(-32767, [Math]::Min(32767, $value * 32767)))
    }

    return $samples
}

function Join-Samples {
    param([int16[][]]$Parts)

    $total = 0
    foreach ($p in $Parts) { $total += $p.Length }
    $result = [int16[]]::new($total)
    $offset = 0
    foreach ($p in $Parts) {
        [Array]::Copy($p, 0, $result, $offset, $p.Length)
        $offset += $p.Length
    }
    return $result
}

# Ensure output directories exist
New-Item -ItemType Directory -Path $OutputSoundsDir -Force | Out-Null
New-Item -ItemType Directory -Path $OutputMusicDir  -Force | Out-Null

Write-Host "Generating sound effects..."

# 1. tile_click.wav - Short tick (10ms, 1000Hz)
$s = Get-SineSamples -Frequency 1000 -DurationMs 10 -Amplitude 0.3 -FadeOut
New-WavFile -Path "$OutputSoundsDir\tile_click.wav" -Samples $s
Write-Host "  tile_click.wav"

# 2. word_valid.wav - Pleasant ascending chime (250ms, two tones)
$s1 = Get-SineSamples -Frequency 880 -DurationMs 120 -Amplitude 0.4 -FadeOut
$s2 = Get-SineSamples -Frequency 1320 -DurationMs 130 -Amplitude 0.3 -FadeOut
$s = Join-Samples @($s1, $s2)
New-WavFile -Path "$OutputSoundsDir\word_valid.wav" -Samples $s
Write-Host "  word_valid.wav"

# 3. word_invalid.wav - Low buzz (150ms, 200Hz)
$s = Get-SineSamples -Frequency 200 -DurationMs 150 -Amplitude 0.35 -FadeOut
New-WavFile -Path "$OutputSoundsDir\word_invalid.wav" -Samples $s
Write-Host "  word_invalid.wav"

# 4. timer_tick.wav - Soft tick (50ms, 600Hz)
$s = Get-SineSamples -Frequency 600 -DurationMs 50 -Amplitude 0.2 -FadeOut
New-WavFile -Path "$OutputSoundsDir\timer_tick.wav" -Samples $s
Write-Host "  timer_tick.wav"

# 5. timer_warning.wav - Urgent tick (80ms, 900Hz)
$s = Get-SineSamples -Frequency 900 -DurationMs 80 -Amplitude 0.4 -FadeOut
New-WavFile -Path "$OutputSoundsDir\timer_warning.wav" -Samples $s
Write-Host "  timer_warning.wav"

# 6. round_start.wav - Ascending arpeggio (C5-E5-G5-C6 = 523-659-784-1047)
$s1 = Get-SineSamples -Frequency 523 -DurationMs 120 -Amplitude 0.35 -FadeOut
$s2 = Get-SineSamples -Frequency 659 -DurationMs 120 -Amplitude 0.35 -FadeOut
$s3 = Get-SineSamples -Frequency 784 -DurationMs 120 -Amplitude 0.35 -FadeOut
$s4 = Get-SineSamples -Frequency 1047 -DurationMs 200 -Amplitude 0.4 -FadeOut
$s = Join-Samples @($s1, $s2, $s3, $s4)
New-WavFile -Path "$OutputSoundsDir\round_start.wav" -Samples $s
Write-Host "  round_start.wav"

# 7. round_end.wav - Descending tone (G5-E5-C5)
$s1 = Get-SineSamples -Frequency 784 -DurationMs 150 -Amplitude 0.35 -FadeOut
$s2 = Get-SineSamples -Frequency 659 -DurationMs 150 -Amplitude 0.3 -FadeOut
$s3 = Get-SineSamples -Frequency 523 -DurationMs 250 -Amplitude 0.3 -FadeOut
$s = Join-Samples @($s1, $s2, $s3)
New-WavFile -Path "$OutputSoundsDir\round_end.wav" -Samples $s
Write-Host "  round_end.wav"

# 8. achievement_unlock.wav - Triumphant fanfare (C5-E5-G5 chord then C6)
$s1 = Get-SineSamples -Frequency 523 -DurationMs 200 -Amplitude 0.3 -FadeOut
$s2 = Get-SineSamples -Frequency 659 -DurationMs 200 -Amplitude 0.3 -FadeOut
$s3 = Get-SineSamples -Frequency 784 -DurationMs 200 -Amplitude 0.35 -FadeOut
$s4 = Get-SineSamples -Frequency 1047 -DurationMs 350 -Amplitude 0.45 -FadeOut
$s = Join-Samples @($s1, $s2, $s3, $s4)
New-WavFile -Path "$OutputSoundsDir\achievement_unlock.wav" -Samples $s
Write-Host "  achievement_unlock.wav"

# 9. button_hover.wav - Very soft click (8ms, 800Hz)
$s = Get-SineSamples -Frequency 800 -DurationMs 8 -Amplitude 0.15 -FadeOut
New-WavFile -Path "$OutputSoundsDir\button_hover.wav" -Samples $s
Write-Host "  button_hover.wav"

# 10. button_click.wav - Crisp click (25ms, 1200Hz)
$s = Get-SineSamples -Frequency 1200 -DurationMs 25 -Amplitude 0.3 -FadeOut
New-WavFile -Path "$OutputSoundsDir\button_click.wav" -Samples $s
Write-Host "  button_click.wav"

# 11. high_score.wav - Celebratory ascending (C5-E5-G5-C6-E6)
$s1 = Get-SineSamples -Frequency 523 -DurationMs 100 -Amplitude 0.3 -FadeOut
$s2 = Get-SineSamples -Frequency 659 -DurationMs 100 -Amplitude 0.3 -FadeOut
$s3 = Get-SineSamples -Frequency 784 -DurationMs 100 -Amplitude 0.35 -FadeOut
$s4 = Get-SineSamples -Frequency 1047 -DurationMs 100 -Amplitude 0.4 -FadeOut
$s5 = Get-SineSamples -Frequency 1319 -DurationMs 300 -Amplitude 0.45 -FadeOut
$s = Join-Samples @($s1, $s2, $s3, $s4, $s5)
New-WavFile -Path "$OutputSoundsDir\high_score.wav" -Samples $s
Write-Host "  high_score.wav"

# 12. pause.wav - Descending whoosh (300ms, 800→200Hz)
$s = Get-FrequencySweep -StartFreq 800 -EndFreq 200 -DurationMs 300 -Amplitude 0.3
New-WavFile -Path "$OutputSoundsDir\pause.wav" -Samples $s
Write-Host "  pause.wav"

# 13. resume.wav - Ascending whoosh (300ms, 200→800Hz)
$s = Get-FrequencySweep -StartFreq 200 -EndFreq 800 -DurationMs 300 -Amplitude 0.3
New-WavFile -Path "$OutputSoundsDir\resume.wav" -Samples $s
Write-Host "  resume.wav"

Write-Host ""
Write-Host "Generating background music..."

# Ambient music - Simple calming loop (~30 seconds)
# Low drone (C3=131Hz) + gentle melody fragments
$sampleRate = 44100
$duration = 30000  # 30 seconds
$numSamples = [int]($sampleRate * $duration / 1000.0)
$musicSamples = [int16[]]::new($numSamples)

for ($i = 0; $i -lt $numSamples; $i++) {
    $t = $i / [double]$sampleRate
    $progress = $i / [double]$numSamples

    # Base drone (C3, very soft)
    $drone = [Math]::Sin(2.0 * [Math]::PI * 131 * $t) * 0.08

    # Slow oscillating pad (G3=196Hz, volume swells)
    $padVol = (1.0 + [Math]::Sin(2.0 * [Math]::PI * 0.1 * $t)) * 0.04
    $pad = [Math]::Sin(2.0 * [Math]::PI * 196 * $t) * $padVol

    # Higher shimmer (E4=330Hz, very gentle)
    $shimmerVol = (1.0 + [Math]::Sin(2.0 * [Math]::PI * 0.07 * $t)) * 0.025
    $shimmer = [Math]::Sin(2.0 * [Math]::PI * 330 * $t) * $shimmerVol

    $value = $drone + $pad + $shimmer

    # Crossfade for seamless loop (fade last 2 seconds)
    $fadeLen = $sampleRate * 2
    if ($i -gt ($numSamples - $fadeLen)) {
        $fadeProgress = ($numSamples - $i) / [double]$fadeLen
        $value *= $fadeProgress
    }
    if ($i -lt $fadeLen) {
        $fadeProgress = $i / [double]$fadeLen
        $value *= $fadeProgress
    }

    $musicSamples[$i] = [int16]([Math]::Max(-32767, [Math]::Min(32767, $value * 32767)))
}

New-WavFile -Path "$OutputMusicDir\ambient.wav" -Samples $musicSamples -SampleRate $sampleRate
Write-Host "  ambient.wav (30s loop)"

Write-Host ""
Write-Host "Done! Generated 13 sound effects + 1 ambient music track."
