// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Specialized;
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

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is HumanInTheLoopViewModel vm)
        {
            // Toggle empty view based on message count
            vm.Session.Messages.CollectionChanged += OnMessagesChanged;
            vm.Session.PendingMessages.CollectionChanged += OnMessagesChanged;
            UpdateEmptyView(vm);
        }
    }

    private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (BindingContext is HumanInTheLoopViewModel vm)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateEmptyView(vm);
                // Auto-scroll to bottom
                MessagesScrollView.ScrollToAsync(0, MessagesScrollView.ContentSize.Height, true);
            });
        }
    }

    private void UpdateEmptyView(HumanInTheLoopViewModel vm)
    {
        EmptyView.IsVisible = vm.Session.Messages.Count == 0 && vm.Session.PendingMessages.Count == 0;
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