// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace MauiDojo.Models;

/// <summary>
/// State model for document content in the Predictive State Updates demo.
/// </summary>
public sealed class DocumentState
{
    [JsonPropertyName("document")]
    public string Document { get; set; } = string.Empty;
}
