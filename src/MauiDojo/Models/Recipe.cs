// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace MauiDojo.Models;

/// <summary>
/// A recipe with ingredients and instructions, used in the Shared State demo.
/// </summary>
public sealed class Recipe
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("skill_level")]
    public string SkillLevel { get; set; } = "Beginner";

    [JsonPropertyName("cooking_time")]
    public string CookingTime { get; set; } = "30 min";

    [JsonPropertyName("special_preferences")]
    public List<string> SpecialPreferences { get; set; } = [];

    [JsonPropertyName("ingredients")]
    public List<Ingredient> Ingredients { get; set; } = [];

    [JsonPropertyName("instructions")]
    public List<string> Instructions { get; set; } = [];
}
