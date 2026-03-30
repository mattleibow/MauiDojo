// Copyright (c) Microsoft. All rights reserved.

using System.Globalization;

namespace MauiDojo.Agent.Maui.Converters;

/// <summary>
/// Converts a <see cref="bool"/> to a visibility value.
/// Pass <c>"invert"</c> as <see cref="IValueConverter.Convert"/> parameter to invert the result.
/// </summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var flag = value is true;

        if (parameter is string s && s.Equals("invert", StringComparison.OrdinalIgnoreCase))
            flag = !flag;

        return flag;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var flag = value is true;

        if (parameter is string s && s.Equals("invert", StringComparison.OrdinalIgnoreCase))
            flag = !flag;

        return flag;
    }
}
