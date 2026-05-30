// <copyright file="ViewModelBaseTests.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Baffleword.App.Tests.ViewModels;

using Baffleword.App.ViewModels;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests for the <see cref="ViewModelBase"/> class.
/// </summary>
public sealed class ViewModelBaseTests
{
    [Fact]
    public void SetProperty_RaisesPropertyChangedEvent()
    {
        var vm = new TestViewModel();
        bool eventRaised = false;
        vm.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(TestViewModel.Name))
            {
                eventRaised = true;
            }
        };

        vm.Name = "Test";

        eventRaised.Should().BeTrue();
    }

    [Fact]
    public void SetProperty_DoesNotRaiseWhenValueUnchanged()
    {
        var vm = new TestViewModel { Name = "Test" };
        bool eventRaised = false;
        vm.PropertyChanged += (_, _) => eventRaised = true;

        vm.Name = "Test";

        eventRaised.Should().BeFalse();
    }

    [Fact]
    public void SetProperty_UpdatesBackingField()
    {
        var vm = new TestViewModel();

        vm.Name = "Hello";

        vm.Name.Should().Be("Hello");
    }

    private sealed class TestViewModel : ViewModelBase
    {
        private string _name = string.Empty;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }
}
