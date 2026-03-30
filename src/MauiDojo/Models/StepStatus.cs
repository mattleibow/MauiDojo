// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;

namespace MauiDojo.Models;

/// <summary>
/// Status of a plan step.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<StepStatus>))]
public enum StepStatus
{
    Pending = 0,
    Completed = 1,
}
