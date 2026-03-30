// Copyright (c) Microsoft. All rights reserved.

namespace MauiDojo.Models;

/// <summary>
/// Describes a demo scenario in the MAUI Dojo.
/// </summary>
public record DemoScenario(
    string Id,
    string Title,
    string Description,
    IReadOnlyList<string> Tags,
    string Endpoint,
    string Icon = "💬");
