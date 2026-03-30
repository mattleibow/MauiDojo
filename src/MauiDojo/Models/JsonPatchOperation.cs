// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace MauiDojo.Models;

/// <summary>
/// RFC 6902 JSON Patch operation for incremental state updates.
/// </summary>
public sealed class JsonPatchOperation
{
    [JsonPropertyName("op")]
    public required string Op { get; set; }

    [JsonPropertyName("path")]
    public required string Path { get; set; }

    [JsonPropertyName("value")]
    public object? Value { get; set; }

    [JsonPropertyName("from")]
    public string? From { get; set; }
}
