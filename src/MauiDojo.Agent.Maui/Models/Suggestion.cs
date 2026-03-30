// Copyright (c) Microsoft. All rights reserved.

namespace MauiDojo.Agent.Maui;

/// <summary>
/// A suggestion that can be displayed in the <see cref="Controls.SuggestionBar"/>.
/// If <paramref name="Message"/> is null, <paramref name="Text"/> is used as the message content.
/// </summary>
public record Suggestion(string Text, string? Message = null);
