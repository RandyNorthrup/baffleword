// <copyright file="NavigationServiceTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.Tests.Navigation;

using Baffleword.App.Navigation;
using Baffleword.App.ViewModels;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public sealed class NavigationServiceTests : IDisposable
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

    [Fact]
    public void NavigateToPreservingCurrent_PreservesCurrentForGoBack()
    {
        _sut.NavigateTo<TestableViewModel>();
        ViewModelBase? original = _sut.CurrentViewModel;

        _sut.NavigateToPreservingCurrent<TestableViewModel>();
        _sut.CurrentViewModel.Should().NotBeSameAs(original);

        bool result = _sut.GoBack();

        result.Should().BeTrue();
        _sut.CurrentViewModel.Should().BeSameAs(original);
    }

    [Fact]
    public void GoBack_WithNoPreviousVm_ReturnsFalse()
    {
        _sut.NavigateTo<TestableViewModel>();

        bool result = _sut.GoBack();

        result.Should().BeFalse();
    }

    [Fact]
    public void NavigateTo_ClearsPreviousVm()
    {
        _sut.NavigateTo<TestableViewModel>();
        _sut.NavigateToPreservingCurrent<TestableViewModel>();

        // Normal navigate should clear the preserved VM
        _sut.NavigateTo<TestableViewModel>();
        bool result = _sut.GoBack();

        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _sut.Dispose();
        _serviceProvider.Dispose();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI container")]
    private sealed class TestableViewModel : ViewModelBase
    {
    }
}
