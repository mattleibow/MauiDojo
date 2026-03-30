// Copyright (c) Microsoft. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDojo.Agent.Maui;
using MauiDojo.Agent.Maui.Services;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MauiDojo.ViewModels;

public partial class BackendToolRenderingViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Backend Tool Rendering";

    [ObservableProperty]
    private string _inputText = string.Empty;

    public IAgentSession Session { get; }

    public ObservableCollection<Suggestion> Suggestions { get; } =
    [
        new("Weather in Tokyo", "What's the weather like in Tokyo?"),
        new("Weather in New York", "What's the weather in New York right now?"),
        new("Weather in London", "Tell me the current weather in London"),
    ];

    public BackendToolRenderingViewModel(
        [FromKeyedServices("backend-tool-rendering")] AIAgent agent,
        IAgentSessionFactory factory)
    {
        Session = factory.Create(agent);
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
}
