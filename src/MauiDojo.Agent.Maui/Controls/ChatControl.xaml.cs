// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Specialized;
using System.Windows.Input;
using MauiDojo.Agent.Maui.Services;
using Microsoft.Extensions.AI;

namespace MauiDojo.Agent.Maui.Controls;

/// <summary>
/// A self-contained chat UI control that binds to an <see cref="IAgentSession"/>.
/// Drop into any page to get a complete chat experience with message history,
/// streaming indicators, suggestions, and user input.
/// </summary>
public partial class ChatControl : ContentView
{
    #region Bindable Properties

    public static readonly BindableProperty SessionProperty =
        BindableProperty.Create(
            nameof(Session),
            typeof(IAgentSession),
            typeof(ChatControl),
            null,
            propertyChanged: OnSessionChanged);

    public static readonly BindableProperty SuggestionsProperty =
        BindableProperty.Create(
            nameof(Suggestions),
            typeof(IList<Suggestion>),
            typeof(ChatControl));

    public static readonly BindableProperty AdditionalContentTemplatesProperty =
        BindableProperty.Create(
            nameof(AdditionalContentTemplates),
            typeof(DataTemplate),
            typeof(ChatControl));

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(
            nameof(Placeholder),
            typeof(string),
            typeof(ChatControl),
            "Type a message…");

    /// <summary>The agent session to bind to.</summary>
    public IAgentSession? Session
    {
        get => (IAgentSession?)GetValue(SessionProperty);
        set => SetValue(SessionProperty, value);
    }

    /// <summary>Suggestion items to display in the suggestion bar.</summary>
    public IList<Suggestion>? Suggestions
    {
        get => (IList<Suggestion>?)GetValue(SuggestionsProperty);
        set => SetValue(SuggestionsProperty, value);
    }

    /// <summary>Allows the app to inject custom content templates.</summary>
    public DataTemplate? AdditionalContentTemplates
    {
        get => (DataTemplate?)GetValue(AdditionalContentTemplatesProperty);
        set => SetValue(AdditionalContentTemplatesProperty, value);
    }

    /// <summary>Placeholder text for the input editor.</summary>
    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    #endregion

    /// <summary>Command invoked when a suggestion is tapped.</summary>
    public ICommand SuggestionTappedCommand { get; }

    public ChatControl()
    {
        SuggestionTappedCommand = new Command<Suggestion>(OnSuggestionTapped);
        InitializeComponent();
    }

    #region Session Binding

    private static void OnSessionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not ChatControl control) return;

        control.DetachSession(oldValue as IAgentSession);
        control.AttachSession(newValue as IAgentSession);
    }

    private void AttachSession(IAgentSession? session)
    {
        if (session is null) return;

        MessagesCollection.ItemsSource = session.Messages;
        PendingMessagesCollection.ItemsSource = session.PendingMessages;

        session.Messages.CollectionChanged += OnMessagesCollectionChanged;
        session.PendingMessages.CollectionChanged += OnPendingMessagesCollectionChanged;
        session.PropertyChanged += OnSessionPropertyChanged;

        SyncProcessingState(session.IsProcessing);
    }

    private void DetachSession(IAgentSession? session)
    {
        if (session is null) return;

        session.Messages.CollectionChanged -= OnMessagesCollectionChanged;
        session.PendingMessages.CollectionChanged -= OnPendingMessagesCollectionChanged;
        session.PropertyChanged -= OnSessionPropertyChanged;

        MessagesCollection.ItemsSource = null;
        PendingMessagesCollection.ItemsSource = null;
    }

    private void OnSessionPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IAgentSession.IsProcessing) && sender is IAgentSession session)
        {
            MainThread.BeginInvokeOnMainThread(() => SyncProcessingState(session.IsProcessing));
        }
    }

    private void SyncProcessingState(bool isProcessing)
    {
        LoadingIndicator.IsActive = isProcessing;
        SendButton.IsEnabled = !isProcessing;
        InputEditor.IsEnabled = !isProcessing;
        SuggestionBarControl.IsEnabled = !isProcessing;
    }

    #endregion

    #region Auto-scroll

    private void OnMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
            ScrollToBottom();
    }

    private void OnPendingMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
            ScrollToBottom();
    }

    private void ScrollToBottom()
    {
        if (Session is null) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            // Scroll the main collection to the last message
            if (Session.Messages.Count > 0)
            {
                MessagesCollection.ScrollTo(
                    Session.Messages.Count - 1,
                    position: ScrollToPosition.End,
                    animate: true);
            }
        });
    }

    #endregion

    #region User Input

    private void OnSendClicked(object? sender, EventArgs e) => SendCurrentMessage();

    private void OnInputCompleted(object? sender, EventArgs e) => SendCurrentMessage();

    private void OnNewChatClicked(object? sender, EventArgs e) => Session?.Reset();

    private void SendCurrentMessage()
    {
        var text = InputEditor.Text?.Trim();
        if (string.IsNullOrEmpty(text) || Session is null || Session.IsProcessing)
            return;

        InputEditor.Text = string.Empty;
        _ = Session.SendAsync(new ChatMessage(ChatRole.User, text));
    }

    private void OnSuggestionTapped(Suggestion suggestion)
    {
        if (Session is null || Session.IsProcessing) return;

        var message = suggestion.Message ?? suggestion.Text;
        _ = Session.SendAsync(new ChatMessage(ChatRole.User, message));
    }

    #endregion
}
