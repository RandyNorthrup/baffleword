// <copyright file="RelayCommand.cs" company="Randy Northrup">
// Copyright (c) 2025 Randy Northrup. Licensed under the MIT License.
// </copyright>

namespace Boggle.App.ViewModels;

using System.Windows.Input;

/// <summary>
/// A command implementation that delegates to <see cref="Action"/> and <see cref="Func{Boolean}"/>.
/// </summary>
public sealed class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The action to execute.</param>
    /// <param name="canExecute">An optional predicate that determines whether the command can execute.</param>
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Raises the <see cref="CanExecuteChanged"/> event.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "Standard MVVM pattern for programmatically raising CanExecuteChanged.")]
    public static void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();

    /// <inheritdoc/>
    public bool CanExecute(object? parameter) => _canExecute is null || _canExecute();

    /// <inheritdoc/>
    public void Execute(object? parameter) => _execute();
}
