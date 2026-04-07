// <copyright file="NavigationServiceTests.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.Tests.Navigation;

using Boggle.App.Navigation;
using Boggle.App.ViewModels;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public sealed class NavigationServiceTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly NavigationService _sut;

    public NavigationServiceTests()
    {
        var services = new ServiceCollection();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton(typeof(Microsoft.Extensions.Logging.ILogger<>), typeof(NullLogger<>));
        services.AddTransient<TestableViewModel>();
        _serviceProvider = services.BuildServiceProvider();

        _sut = new NavigationService(
            _serviceProvider,
            NullLogger<NavigationService>.Instance);
    }

    [Fact]
    public void CurrentViewModel_InitiallyNull()
    {
        _sut.CurrentViewModel.Should().BeNull();
    }

    [Fact]
    public void NavigateTo_SetsCurrentViewModel()
    {
        _sut.NavigateTo<TestableViewModel>();

        _sut.CurrentViewModel.Should().NotBeNull();
        _sut.CurrentViewModel.Should().BeOfType<TestableViewModel>();
    }

    [Fact]
    public void NavigateTo_RaisesNavigationChanged()
    {
        bool eventFired = false;
        _sut.NavigationChanged += (_, _) => eventFired = true;

        _sut.NavigateTo<TestableViewModel>();

        eventFired.Should().BeTrue();
    }

    [Fact]
    public void NavigateTo_MultipleTimes_UpdatesCurrentViewModel()
    {
        _sut.NavigateTo<TestableViewModel>();
        ViewModelBase? first = _sut.CurrentViewModel;

        _sut.NavigateTo<TestableViewModel>();
        ViewModelBase? second = _sut.CurrentViewModel;

        first.Should().NotBeSameAs(second);
    }

    [Fact]
    public void Constructor_WithNullServiceProvider_ThrowsArgumentNullException()
    {
        Action act = () => new NavigationService(null!, NullLogger<NavigationService>.Instance);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Action act = () => new NavigationService(_serviceProvider, null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
    private sealed class TestableViewModel : ViewModelBase
    {
    }
}
