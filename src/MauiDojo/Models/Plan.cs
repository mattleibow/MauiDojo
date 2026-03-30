// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace MauiDojo.Models;

/// <summary>
/// A plan consisting of steps, used in the Agentic Generative UI and Human-in-the-Loop demos.
/// </summary>
public sealed class Plan
{
    [JsonPropertyName("steps")]
    public List<Step> Steps { get; set; } = [];

    /// <summary>Number of completed steps.</summary>
    [JsonIgnore]
    public int CompletedCount => Steps.Count(s => s.Status == StepStatus.Completed);

    /// <summary>Whether all steps are completed.</summary>
    [JsonIgnore]
    public bool IsComplete => Steps.Count > 0 && Steps.All(s => s.Status == StepStatus.Completed);
}
