// Copyright (c) Microsoft. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDojo.Agent.Maui;
using MauiDojo.Agent.Maui.Services;
using MauiDojo.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MauiDojo.ViewModels;

public partial class PredictiveStateUpdatesViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Predictive State Updates";

    [ObservableProperty]
    private string userMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDocument))]
    private DocumentState? currentDocument;

    [ObservableProperty]
    private DocumentState? previousDocument;

    public bool HasDocument => CurrentDocument is not null && !string.IsNullOrEmpty(CurrentDocument.Document);

    [ObservableProperty]
    private bool isStreaming;

    [ObservableProperty]
    private bool isAwaitingConfirmation;

    public IAgentSession Session { get; }

    public ObservableCollection<Suggestion> Suggestions { get; } =
    [
        new("Write a pirate story"),
        new("Write about a space adventure"),
        new("Add a plot twist"),
    ];

    public PredictiveStateUpdatesViewModel(
        [FromKeyedServices("predictive-state-updates")] AIAgent agent,
        IAgentSessionFactory factory)
    {
        Session = factory.Create(agent);

        [Description("Wait for the user to confirm or reject the proposed document changes.")]
        async Task<ConfirmChangesResult> confirm_changes()
        {
            await MainThread.InvokeOnMainThreadAsync(() => IsAwaitingConfirmation = true);
            var response = await Session.WaitForResponse("confirm_changes");
            await MainThread.InvokeOnMainThreadAsync(() => IsAwaitingConfirmation = false);
            return response as ConfirmChangesResult ?? new ConfirmChangesResult { Confirmed = false };
        }

        Session.RegisterTool(AIFunctionFactory.Create(confirm_changes));

        Session.StateSnapshotReceived += OnStateSnapshotReceived;
        Session.PropertyChanged += OnSessionPropertyChanged;
    }

    private void OnStateSnapshotReceived(ReadOnlyMemory<byte> data)
    {
        DocumentState? state;
        try
        {
            state = JsonSerializer.Deserialize(data.Span, MauiDojoSerializerContext.Default.DocumentState);
        }
        catch
        {
            return;
        }

        if (state is null)
            return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            PreviousDocument = CurrentDocument;
            CurrentDocument = state;
            IsStreaming = Session.IsProcessing;
        });
    }

    private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IAgentSession.IsProcessing) && !Session.IsProcessing)
        {
            MainThread.BeginInvokeOnMainThread(() => IsStreaming = false);
        }
    }

    [RelayCommand]
    private void AcceptChanges()
    {
        IsAwaitingConfirmation = false;
        Session.ProvideResponse("confirm_changes", new ConfirmChangesResult { Confirmed = true });
    }

    [RelayCommand]
    private void RejectChanges()
    {
        if (PreviousDocument is not null)
            CurrentDocument = PreviousDocument;
        IsAwaitingConfirmation = false;
        Session.ProvideResponse("confirm_changes", new ConfirmChangesResult { Confirmed = false });
    }

    [RelayCommand]
    private async Task SendAsync()
    {
        var text = UserMessage?.Trim();
        if (string.IsNullOrEmpty(text))
            return;

        UserMessage = string.Empty;
        await Session.SendAsync(new ChatMessage(ChatRole.User, text));
    }

    [RelayCommand]
    private async Task SendSuggestionAsync(Suggestion suggestion)
    {
        var text = suggestion.Message ?? suggestion.Text;
        UserMessage = string.Empty;
        await Session.SendAsync(new ChatMessage(ChatRole.User, text));
    }
}
