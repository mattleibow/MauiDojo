// Copyright (c) Microsoft. All rights reserved.

namespace MauiDojo.Models;

/// <summary>
/// Result of a plan confirmation from the user in the Human-in-the-Loop demo.
/// </summary>
public sealed class PlanConfirmationResult
{
    public bool Confirmed { get; set; }
    public List<int> SelectedStepIndices { get; set; } = [];
}
