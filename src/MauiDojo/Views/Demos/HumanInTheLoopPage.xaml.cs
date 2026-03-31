// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;
using Microsoft.Extensions.AI;

namespace MauiDojo.Views.Demos;

public partial class HumanInTheLoopPage : ContentPage
{
    private static readonly Dictionary<string, string> SuggestionMessages = new()
    {
        ["Simple plan"] = "Create a simple 5-step plan for organizing a birthday party",
        ["Complex plan"] = "Create a detailed 10-step plan for launching a new product",
    };

    public HumanInTheLoopPage()
    {
        InitializeComponent();
        try
        {
            BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<HumanInTheLoopViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"HumanInTheLoopPage: {ex.Message}");
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