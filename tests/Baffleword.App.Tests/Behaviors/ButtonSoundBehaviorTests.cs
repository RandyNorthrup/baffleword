// <copyright file="ButtonSoundBehaviorTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.Tests.Behaviors;

using Avalonia.Controls;
using Baffleword.App.Behaviors;
using FluentAssertions;
using Xunit;

public sealed class ButtonSoundBehaviorTests
{
    [Fact]
    public void EnableSounds_DefaultsToFalse()
    {
        var button = new Button();

        ButtonSoundBehavior.GetEnableSounds(button).Should().BeFalse();
    }

    [Fact]
    public void EnableSounds_CanBeToggled()
    {
        var button = new Button();

        ButtonSoundBehavior.SetEnableSounds(button, true);
        ButtonSoundBehavior.GetEnableSounds(button).Should().BeTrue();

        ButtonSoundBehavior.SetEnableSounds(button, false);
        ButtonSoundBehavior.GetEnableSounds(button).Should().BeFalse();
    }
}
