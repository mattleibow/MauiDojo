// Copyright (c) Microsoft. All rights reserved.

using System.Globalization;

namespace MauiDojo.Agent.Maui.Converters;

/// <summary>
/// Inverts a <see cref="bool"/> value: <c>true</c> → <c>false</c>, <c>false</c> → <c>true</c>.
/// </summary>
public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not true;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is not true;
}
