// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace MauiDojo.Models;

/// <summary>
/// Wrapper for recipe response from the shared state agent.
/// Required for ChatResponseFormat.ForJsonSchema&lt;T&gt;() on the server.
/// </summary>
public sealed class RecipeResponse
{
    [JsonPropertyName("recipe")]
    public Recipe? Recipe { get; set; }
}
