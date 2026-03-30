// Copyright (c) Microsoft. All rights reserved.

namespace MauiDojo.Services;

/// <inheritdoc />
public class BackgroundColorService : IBackgroundColorService
{
    /// <inheritdoc />
    public event EventHandler<string>? ColorChanged;

    /// <inheritdoc />
    public void SetColor(string color)
    {
        ColorChanged?.Invoke(this, color);
    }
}
