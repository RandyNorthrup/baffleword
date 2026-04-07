// <copyright file="PauseViewModel.cs" company="Boggle">
// Copyright (c) Boggle. All rights reserved.
// </copyright>

namespace Boggle.App.ViewModels;

using System.Windows.Input;

/// <summary>
/// ViewModel for the pause overlay view.
/// </summary>
public sealed class PauseViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PauseViewModel"/> class.
    /// </summary>
    /// <param name="resumeAction">Action to resume the game.</param>
    /// <param name="quitAction">Action to quit to main menu.</param>
    public PauseViewModel(Action resumeAction, Action quitAction)
    {
        ResumeCommand = new RelayCommand(resumeAction ?? throw new ArgumentNullException(nameof(resumeAction)));
        QuitCommand = new RelayCommand(quitAction ?? throw new ArgumentNullException(nameof(quitAction)));
    }

    /// <summary>
    /// Gets the command to resume the game.
    /// </summary>
    public ICommand ResumeCommand { get; }

    /// <summary>
    /// Gets the command to quit to the menu.
    /// </summary>
    public ICommand QuitCommand { get; }
}
