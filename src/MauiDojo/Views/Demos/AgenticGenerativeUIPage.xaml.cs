// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;
using Microsoft.Extensions.AI;

namespace MauiDojo.Views.Demos;

public partial class AgenticGenerativeUIPage : ContentPage
{
    public AgenticGenerativeUIPage()
    {
        InitializeComponent();
        try
        {
            BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<AgenticGenerativeUIViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AgenticGenerativeUIPage: {ex.Message}");
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
        var text = !string.IsNullOrEmpty(btn.AutomationId) ? btn.AutomationId : btn.Text;
        await session.SendAsync(new ChatMessage(ChatRole.User, text));
    }
}