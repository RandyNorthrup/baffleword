// <copyright file="NavigationService.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.Navigation;

using Boggle.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// DI-backed navigation service that resolves ViewModels from the service provider.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NavigationService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for ViewModel resolution.</param>
    /// <param name="logger">The logger instance.</param>
    public NavigationService(IServiceProvider serviceProvider, ILogger<NavigationService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public event EventHandler? NavigationChanged;

    /// <inheritdoc/>
    public ViewModelBase? CurrentViewModel { get; private set; }

    /// <inheritdoc/>
    public void NavigateTo<TViewModel>()
        where TViewModel : ViewModelBase
    {
        _logger.LogDebug("Navigating to {ViewModelType}", typeof(TViewModel).Name);
        CurrentViewModel = _serviceProvider.GetRequiredService<TViewModel>();
        NavigationChanged?.Invoke(this, EventArgs.Empty);
    }
}
