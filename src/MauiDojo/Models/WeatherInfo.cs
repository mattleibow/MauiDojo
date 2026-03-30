// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace MauiDojo.Models;

/// <summary>
/// Weather information returned by the backend get_weather tool.
/// </summary>
public sealed class WeatherInfo
{
    [JsonPropertyName("temperature")]
    public int Temperature { get; init; }

    [JsonPropertyName("conditions")]
    public string Conditions { get; init; } = string.Empty;

    [JsonPropertyName("humidity")]
    public int Humidity { get; init; }

    [JsonPropertyName("wind_speed")]
    public int WindSpeed { get; init; }

    [JsonPropertyName("feelsLike")]
    public int FeelsLike { get; init; }

    /// <summary>Temperature converted to Fahrenheit.</summary>
    public double TemperatureFahrenheit => Temperature * 9.0 / 5.0 + 32;

    /// <summary>Temperature "feels like" converted to Fahrenheit.</summary>
    public double FeelsLikeFahrenheit => FeelsLike * 9.0 / 5.0 + 32;

    /// <summary>Returns an emoji icon based on weather conditions.</summary>
    public string ConditionIcon => Conditions.ToLowerInvariant() switch
    {
        "sunny" or "clear" => "☀️",
        "cloudy" or "overcast" => "☁️",
        "rainy" or "rain" => "🌧️",
        "stormy" or "thunderstorm" => "⛈️",
        "snowy" or "snow" => "❄️",
        "foggy" or "fog" => "🌫️",
        "windy" => "💨",
        "partly cloudy" => "⛅",
        _ => "🌡️",
    };
}
