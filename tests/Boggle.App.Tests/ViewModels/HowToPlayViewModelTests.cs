// <copyright file="HowToPlayViewModelTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.Tests.ViewModels;

using Boggle.App.Navigation;
using Boggle.App.ViewModels;
using FluentAssertions;
using Moq;
using Xunit;

public sealed class HowToPlayViewModelTests
{
    private readonly Mock<INavigationService> _navigation = new();

    [Fact]
    public void Constructor_ThrowsOnNullNavigation()
    {
        var act = () => new HowToPlayViewModel(null!);
        act.Should().Throw<ArgumentNullException>().WithParameterName("navigation");
    }

    [Fact]
    public void BackCommand_NavigatesToMainMenu()
    {
        var sut = new HowToPlayViewModel(_navigation.Object);

        sut.BackCommand.Execute(null);

        _navigation.Verify(n => n.NavigateTo<MainMenuViewModel>(), Times.Once);
    }
}
