// Copyright (c) Microsoft. All rights reserved.

namespace MauiDojo.Services;

/// <summary>
/// Service that manages the background color of the chat interface,
/// allowing frontend tools to change the UI appearance.
/// </summary>
public interface IBackgroundColorService
{
    /// <summary>
    /// Sets the background color and raises <see cref="ColorChanged"/>.
    /// </summary>
    /// <param name="color">A color name (e.g., "blue") or hex value (e.g., "#FF5733").</param>
    void SetColor(string color);

    /// <summary>
    /// Raised when the background color changes. The event arg is the new color string.
    /// </summary>
    event EventHandler<string>? ColorChanged;
}
