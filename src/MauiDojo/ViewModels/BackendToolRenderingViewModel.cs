// Copyright (c) Microsoft. All rights reserved.

using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDojo.Agent.Maui;
using MauiDojo.Agent.Maui.Services;
using MauiDojo.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MauiDojo.ViewModels;

public partial class BackendToolRenderingViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    private string _title = "Backend Tool Rendering";

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasWeather))]
    private WeatherInfo? _currentWeather;

    [ObservableProperty]
    private string _weatherLocation = string.Empty;

    public bool HasWeather => CurrentWeather is not null;

    public IAgentSession Session { get; }

    public ObservableCollection<Suggestion> Suggestions { get; } =
    [
        new("Weather in Tokyo", "What's the weather like in Tokyo?"),
        new("Weather in New York", "What's the weather in New York right now?"),
        new("Weather in London", "Tell me the current weather in London"),
    ];

    public BackendToolRenderingViewModel(
        [FromKeyedServices("backend-tool-rendering")] AIAgent agent,
        IAgentSessionFactory factory)
    {
        Session = factory.Create(agent);
        Session.ResponseUpdated += OnResponseUpdated;
    }

    private void OnResponseUpdated()
    {
        // Scan pending messages for get_weather function results
        foreach (var msg in Session.PendingMessages)
        {
            ScanForWeatherResult(msg);
        }

        foreach (var msg in Session.Messages)
        {
            ScanForWeatherResult(msg);
        }
    }

    private void ScanForWeatherResult(ChatMessageViewModel msg)
    {
        string? location = null;
        WeatherInfo? weather = null;

        foreach (var content in msg.Contents)
        {
            if (content is FunctionCallContent fcc &&
                string.Equals(fcc.Name, "get_weather", StringComparison.OrdinalIgnoreCase))
            {
                // Extract the location argument
                if (fcc.Arguments?.TryGetValue("location", out var loc) == true)
                    location = loc?.ToString();
            }
            else if (content is FunctionResultContent frc && weather is null)
            {
                // Try to parse the result as WeatherInfo
                weather = TryParseWeather(frc.Result);
            }
        }

        if (weather is not null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                WeatherLocation = location ?? "Unknown";
                CurrentWeather = weather;
            });
        }
    }

    private static WeatherInfo? TryParseWeather(object? result)
    {
        if (result is WeatherInfo w)
            return w;

        if (result is JsonElement jsonElement)
        {
            try
            {
                return JsonSerializer.Deserialize(
                    jsonElement.GetRawText(),
                    MauiDojoSerializerContext.Default.WeatherInfo);
            }
            catch { }
        }

        if (result is string jsonString)
        {
            try
            {
                return JsonSerializer.Deserialize(jsonString, MauiDojoSerializerContext.Default.WeatherInfo);
            }
            catch { }
        }

        return null;
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        var text = InputText?.Trim();
        if (string.IsNullOrEmpty(text) || Session.IsProcessing)
            return;

        InputText = string.Empty;
        CurrentWeather = null;
        await Session.SendAsync(new ChatMessage(ChatRole.User, text));
    }

    [RelayCommand]
    private async Task SendSuggestionAsync(Suggestion suggestion)
    {
        if (Session.IsProcessing)
            return;

        var message = suggestion.Message ?? suggestion.Text;
        InputText = string.Empty;
        CurrentWeather = null;
        await Session.SendAsync(new ChatMessage(ChatRole.User, message));
    }

    public void Dispose()
    {
        Session.ResponseUpdated -= OnResponseUpdated;
    }
}
