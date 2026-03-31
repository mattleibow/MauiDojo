// Copyright (c) Microsoft. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDojo.Agent.Maui;
using MauiDojo.Agent.Maui.Services;
using MauiDojo.Services;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MauiDojo.ViewModels;

public partial class AgenticChatViewModel : ObservableObject, IDisposable
{
    private readonly IBackgroundColorService _backgroundColorService;

    [ObservableProperty]
    private string _title = "Agentic Chat";

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private Color _backgroundColor = Colors.White;

    public IAgentSession Session { get; }

    public ObservableCollection<Suggestion> Suggestions { get; } =
    [
        new("Change background", "Change background to light blue"),
        new("Generate sonnet", "Generate sonnet"),
    ];

    public AgenticChatViewModel(
        [FromKeyedServices("agentic-chat")] AIAgent agent,
        IAgentSessionFactory factory,
        IBackgroundColorService backgroundColorService)
    {
        _backgroundColorService = backgroundColorService;
        _backgroundColorService.ColorChanged += OnColorChanged;

        Session = factory.Create(agent);
    }

    private void OnColorChanged(object? sender, string colorString)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (Color.TryParse(colorString, out var parsed))
                BackgroundColor = parsed;
        });
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        var text = InputText?.Trim();
        if (string.IsNullOrEmpty(text) || Session.IsProcessing)
            return;

        InputText = string.Empty;
        await Session.SendAsync(new ChatMessage(ChatRole.User, text));
    }

    [RelayCommand]
    private async Task SendSuggestionAsync(Suggestion suggestion)
    {
        if (Session.IsProcessing)
            return;

        var message = suggestion.Message ?? suggestion.Text;
        InputText = string.Empty;
        await Session.SendAsync(new ChatMessage(ChatRole.User, message));
    }

    public void Dispose()
    {
        _backgroundColorService.ColorChanged -= OnColorChanged;
    }
}
