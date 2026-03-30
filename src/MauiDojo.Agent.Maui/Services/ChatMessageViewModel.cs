// Copyright (c) Microsoft. All rights reserved.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.AI;

namespace MauiDojo.Agent.Maui.Services;

/// <summary>
/// Observable wrapper around <see cref="ChatMessage"/> for MAUI data binding.
/// Accumulates streamed content and exposes computed role helpers.
/// </summary>
public partial class ChatMessageViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUser))]
    [NotifyPropertyChangedFor(nameof(IsAssistant))]
    private ChatRole _role;

    [ObservableProperty]
    private string _text = string.Empty;

    [ObservableProperty]
    private string _authorName = string.Empty;

    /// <summary>All content items received for this message.</summary>
    public ObservableCollection<AIContent> Contents { get; } = [];

    /// <summary>True when the message role is <see cref="ChatRole.User"/>.</summary>
    public bool IsUser => Role == ChatRole.User;

    /// <summary>True when the message role is <see cref="ChatRole.Assistant"/>.</summary>
    public bool IsAssistant => Role == ChatRole.Assistant;

    public ChatMessageViewModel()
    {
    }

    public ChatMessageViewModel(ChatRole role, string text = "")
    {
        _role = role;
        _text = text;
    }

    /// <summary>Creates a view model from an existing <see cref="ChatMessage"/>.</summary>
    public static ChatMessageViewModel FromChatMessage(ChatMessage message)
    {
        var vm = new ChatMessageViewModel(message.Role, message.Text ?? string.Empty)
        {
            AuthorName = message.AuthorName ?? string.Empty
        };

        foreach (var content in message.Contents)
        {
            vm.Contents.Add(content);
        }

        return vm;
    }
}
