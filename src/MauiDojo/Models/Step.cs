// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDojo.Models;

/// <summary>
/// A single step in a plan with a description and completion status.
/// </summary>
public sealed partial class Step : ObservableObject
{
    [ObservableProperty]
    [property: JsonPropertyName("description")]
    private string _description = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("status")]
    private StepStatus _status = StepStatus.Pending;

    /// <summary>
    /// UI-only: whether this step is selected for confirmation.
    /// Not serialized to JSON.
    /// </summary>
    [ObservableProperty]
    [property: JsonIgnore]
    private bool _isSelected = true;
}
