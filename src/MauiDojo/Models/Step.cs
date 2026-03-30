// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace MauiDojo.Models;

/// <summary>
/// A single step in a plan with a description and completion status.
/// </summary>
public sealed class Step
{
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public StepStatus Status { get; set; } = StepStatus.Pending;
}
