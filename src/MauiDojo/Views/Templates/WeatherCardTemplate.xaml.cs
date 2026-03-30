// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using MauiDojo.Agent.Maui.Services;
using MauiDojo.Models;

namespace MauiDojo.Views.Templates;

public partial class WeatherCardTemplate : ContentView
{
    public WeatherCardTemplate()
    {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is not InvocationContext ctx)
            return;

        // Set location from arguments
        var location = ctx.GetArgument<string>("location") ?? "Unknown";
        LocationLabel.Text = $"📍 {location}";

        if (ctx.HasResult)
        {
            ShowResult(ctx);
        }
        else
        {
            LoadingPanel.IsVisible = true;
            ResultPanel.IsVisible = false;
            ctx.ResultArrived += () => MainThread.BeginInvokeOnMainThread(() => ShowResult(ctx));
        }
    }

    private void ShowResult(InvocationContext ctx)
    {
        LoadingPanel.IsVisible = false;
        ResultPanel.IsVisible = true;

        WeatherInfo? weather = null;

        if (ctx.Result is JsonElement jsonElement)
        {
            try
            {
                weather = JsonSerializer.Deserialize(
                    jsonElement.GetRawText(),
                    MauiDojoSerializerContext.Default.WeatherInfo);
            }
            catch { }
        }
        else if (ctx.Result is WeatherInfo w)
        {
            weather = w;
        }

        if (weather is null)
        {
            ConditionIconLabel.Text = "❓";
            TemperatureLabel.Text = "N/A";
            ConditionsLabel.Text = "Could not parse weather data";
            return;
        }

        ConditionIconLabel.Text = weather.ConditionIcon;
        TemperatureLabel.Text = $"{weather.Temperature}°C / {weather.TemperatureFahrenheit:F0}°F";
        ConditionsLabel.Text = weather.Conditions;
        HumidityLabel.Text = $"{weather.Humidity}%";
        WindLabel.Text = $"{weather.WindSpeed} km/h";
        FeelsLikeLabel.Text = $"{weather.FeelsLike}°C / {weather.FeelsLikeFahrenheit:F0}°F";
    }
}
