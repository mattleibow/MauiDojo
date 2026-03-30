// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace MauiDojo.Models;

/// <summary>
/// An ingredient in a recipe with an emoji icon, name, and amount.
/// </summary>
public sealed class Ingredient
{
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = "🍽️";

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public string Amount { get; set; } = string.Empty;
}
