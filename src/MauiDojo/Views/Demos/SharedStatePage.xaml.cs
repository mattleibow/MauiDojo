// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;
using Microsoft.Extensions.AI;

namespace MauiDojo.Views.Demos;

public partial class SharedStatePage : ContentPage
{
    public SharedStatePage()
    {
        InitializeComponent();
        try
        {
            BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<SharedStateViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SharedStatePage: {ex.Message}");
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
        var text = btn.Text;
        // Strip leading emoji + space
        if (text.Length > 2 && (char.IsHighSurrogate(text[0]) || text[0] > 127))
        {
            var spaceIdx = text.IndexOf(' ');
            if (spaceIdx > 0 && spaceIdx < 4) text = text[(spaceIdx + 1)..];
        }
        await session.SendAsync(new ChatMessage(ChatRole.User, text));
    }
}