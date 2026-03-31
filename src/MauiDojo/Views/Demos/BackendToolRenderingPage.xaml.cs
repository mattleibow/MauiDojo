// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;
using Microsoft.Extensions.AI;

namespace MauiDojo.Views.Demos;

public partial class BackendToolRenderingPage : ContentPage
{
    private static readonly Dictionary<string, string> SuggestionMessages = new()
    {
        ["Weather in San Francisco"] = "What's the weather like in San Francisco?",
        ["Weather in New York"] = "What's the weather like in New York?",
        ["Weather in Tokyo"] = "What's the weather like in Tokyo?",
    };

    public BackendToolRenderingPage()
    {
        InitializeComponent();
        try
        {
            BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<BackendToolRenderingViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BackendToolRenderingPage: {ex.Message}");
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
        {
            if (BindingContext is BackendToolRenderingViewModel vm)
            {
                vm.CurrentWeather = null;
                vm.IsWeatherLoading = true;
            }
            await session.SendAsync(new ChatMessage(ChatRole.User, message));
        }
    }
}