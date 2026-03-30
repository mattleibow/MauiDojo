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

public partial class ToolBasedGenerativeUIViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Tool-Based Generative UI";

    [ObservableProperty]
    private string userMessage = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentHaiku))]
    [NotifyPropertyChangedFor(nameof(HasHaikus))]
    [NotifyPropertyChangedFor(nameof(HasPreviousHaiku))]
    [NotifyPropertyChangedFor(nameof(HasNextHaiku))]
    [NotifyPropertyChangedFor(nameof(HaikuPosition))]
    private int currentHaikuIndex;

    public IAgentSession Session { get; }

    public ObservableCollection<Haiku> Haikus { get; } = [];

    public Haiku? CurrentHaiku =>
        Haikus.Count > 0 && CurrentHaikuIndex >= 0 && CurrentHaikuIndex < Haikus.Count
            ? Haikus[CurrentHaikuIndex]
            : null;

    public bool HasHaikus => Haikus.Count > 0;

    public bool HasPreviousHaiku => CurrentHaikuIndex > 0;

    public bool HasNextHaiku => CurrentHaikuIndex < Haikus.Count - 1;

    public string HaikuPosition => HasHaikus ? $"{CurrentHaikuIndex + 1} of {Haikus.Count}" : string.Empty;

    public ObservableCollection<Suggestion> Suggestions { get; } =
    [
        new("Write a nature haiku"),
        new("Haiku about the ocean"),
        new("Mountain haiku"),
    ];

    public ToolBasedGenerativeUIViewModel(
        [FromKeyedServices("tool-based-generative-ui")] AIAgent agent,
        IAgentSessionFactory factory)
    {
        Session = factory.Create(agent);

        [Description("Generate and display a haiku with Japanese text and English translation.")]
        string generate_haiku(
            [Description("Array of Japanese text lines for the haiku")] string[] japanese,
            [Description("Array of English translation lines for the haiku")] string[] english,
            [Description("Optional image name for the haiku background")] string? image_name = null,
            [Description("Optional CSS gradient for the haiku background")] string? gradient = null)
        {
            var haiku = new Haiku
            {
                Japanese = japanese,
                English = english,
                ImageName = image_name,
                Gradient = gradient ?? "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
            };

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Haikus.Add(haiku);
                CurrentHaikuIndex = Haikus.Count - 1;
                OnPropertyChanged(nameof(HasHaikus));
                OnPropertyChanged(nameof(CurrentHaiku));
                OnPropertyChanged(nameof(HasPreviousHaiku));
                OnPropertyChanged(nameof(HasNextHaiku));
                OnPropertyChanged(nameof(HaikuPosition));
            });

            return JsonSerializer.Serialize(haiku, MauiDojoSerializerContext.Default.Haiku);
        }

        Session.RegisterTool(AIFunctionFactory.Create(generate_haiku));
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

    [RelayCommand]
    private void NextHaiku()
    {
        if (CurrentHaikuIndex < Haikus.Count - 1)
            CurrentHaikuIndex++;
    }

    [RelayCommand]
    private void PreviousHaiku()
    {
        if (CurrentHaikuIndex > 0)
            CurrentHaikuIndex--;
    }
}
