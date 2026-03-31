// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;
using Microsoft.Extensions.AI;

namespace MauiDojo.Views.Demos;

public partial class AgenticChatPage : ContentPage
{
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
        if (BindingContext is not AgenticChatViewModel vm) return;
        var text = InputEntry.Text?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        InputEntry.Text = string.Empty;
        await vm.Session.SendAsync(new ChatMessage(ChatRole.User, text));
    }

    private async void OnSuggestionClicked(object? sender, EventArgs e)
    {
        if (BindingContext is not AgenticChatViewModel vm) return;
        if (sender is Button btn)
        {
            // Strip emoji prefix for the message
            var text = btn.Text;
            if (text.Length > 2 && text[1] == ' ') text = text[2..];
            await vm.Session.SendAsync(new ChatMessage(ChatRole.User, text));
        }
    }
}