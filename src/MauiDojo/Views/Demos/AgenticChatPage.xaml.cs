// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;
using Microsoft.Extensions.AI;

namespace MauiDojo.Views.Demos;

public partial class AgenticChatPage : ContentPage
{
    // Maps suggestion button text to the full message sent to the agent (matches Blazor exactly)
    private static readonly Dictionary<string, string> SuggestionMessages = new()
    {
        ["Change background"] = "Change background to light blue",
        ["Generate sonnet"] = "Generate sonnet",
    };

    public AgenticChatPage()
    {
        InitializeComponent();
        try
        {
            BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<AgenticChatViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AgenticChatPage: {ex.Message}");
        }
    }

    private async void OnSendClicked(object? sender, EventArgs e)
    {
        var session = (BindingContext as dynamic)?.Session as MauiDojo.Agent.Maui.Services.IAgentSession;
        if (session is null) return;
        var text = InputEntry.Text?.Trim();
        if (string.IsNullOrEmpty(text)) return;
        InputEntry.Text = string.Empty;
        await session.SendAsync(new ChatMessage(ChatRole.User, text));
    }

    private async void OnSuggestionClicked(object? sender, EventArgs e)
    {
        var session = (BindingContext as dynamic)?.Session as MauiDojo.Agent.Maui.Services.IAgentSession;
        if (session is null || sender is not Button btn) return;
        if (SuggestionMessages.TryGetValue(btn.Text, out var message))
            await session.SendAsync(new ChatMessage(ChatRole.User, message));
    }
}