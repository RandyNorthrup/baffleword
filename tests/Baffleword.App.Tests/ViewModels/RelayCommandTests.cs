// <copyright file="RelayCommandTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.Tests.ViewModels;

using Baffleword.App.ViewModels;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the <see cref="RelayCommand"/> class.
/// </summary>
public sealed class RelayCommandTests
{
    [Fact]
    public void Execute_InvokesAction()
    {
        bool executed = false;
        var command = new RelayCommand(() => executed = true);

        command.Execute(null);

        executed.Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WhenNoPredicateProvided_ReturnsTrue()
    {
        var command = new RelayCommand(() => { });

        command.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void CanExecute_WhenPredicateReturnsFalse_ReturnsFalse()
    {
        var command = new RelayCommand(() => { }, () => false);

        command.CanExecute(null).Should().BeFalse();
    }

    [Fact]
    public void CanExecute_WhenPredicateReturnsTrue_ReturnsTrue()
    {
        var command = new RelayCommand(() => { }, () => true);

        command.CanExecute(null).Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithNullAction_ThrowsArgumentNullException()
    {
        Action act = () => new RelayCommand(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void CanExecuteChanged_FiresWhenCommandInvalidated()
    {
        int count = 0;
        var command = new RelayCommand(() => { });
        command.CanExecuteChanged += (_, _) => count++;

        command.RaiseCanExecuteChanged();

        count.Should().Be(1);
    }
}
