// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Specialized;
using System.Text.Json;
using MauiDojo.Agent.Maui.Services;
using MauiDojo.Models;
using Microsoft.Extensions.AI;

namespace MauiDojo.Views.Templates;

/// <summary>
/// Inline weather card that renders inside the message list.
/// Extracts weather data from the <see cref="ChatMessageViewModel.Contents"/>
/// (FunctionCallContent for location, FunctionResultContent for weather data).
/// </summary>
public partial class InlineWeatherCardView : ContentView
{
    public InlineWeatherCardView()
    {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is not ChatMessageViewModel vm)
            return;

        UpdateDisplay(vm);

        // Watch for new content arriving (streaming updates)
        vm.Contents.CollectionChanged += OnContentsChanged;
    }

    private void OnContentsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (BindingContext is ChatMessageViewModel vm)
        {
            MainThread.BeginInvokeOnMainThread(() => UpdateDisplay(vm));
        }
    }

    private void UpdateDisplay(ChatMessageViewModel vm)
    {
        string? location = null;
        WeatherInfo? weather = null;

        foreach (var content in vm.Contents)
        {
            if (content is FunctionCallContent fcc &&
                string.Equals(fcc.Name, "get_weather", StringComparison.OrdinalIgnoreCase))
            {
                if (fcc.Arguments?.TryGetValue("location", out var loc) == true)
                    location = loc?.ToString();
            }
            else if (content is FunctionResultContent frc && weather is null)
            {
                weather = TryParseWeather(frc.Result);
            }
        }

        if (weather is not null)
        {
            LoadingPanel.IsVisible = false;
            WeatherPanel.IsVisible = true;
            LocationLabel.Text = location ?? "Unknown";
            ConditionIconLabel.Text = weather.ConditionIcon;
            TemperatureLabel.Text = $"{weather.Temperature}°C";
            TemperatureFLabel.Text = $"/ {weather.TemperatureFahrenheit:F0}°F";
            ConditionsLabel.Text = weather.Conditions;
            HumidityLabel.Text = $"{weather.Humidity}%";
            WindLabel.Text = $"{weather.WindSpeed} km/h";
            FeelsLikeLabel.Text = $"{weather.FeelsLike}°C";
        }
        else if (location is not null)
        {
            // Have the function call but no result yet — show loading
            LoadingPanel.IsVisible = true;
            WeatherPanel.IsVisible = false;
        }

        // Show text content below the card if present
        if (!string.IsNullOrEmpty(vm.Text))
        {
            TextFrame.IsVisible = true;
            TextLabel.Text = vm.Text;
        }
        else
        {
            TextFrame.IsVisible = false;
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
                return JsonSerializer.Deserialize(
                    jsonString,
                    MauiDojoSerializerContext.Default.WeatherInfo);
            }
            catch { }
        }

        return null;
    }
}
