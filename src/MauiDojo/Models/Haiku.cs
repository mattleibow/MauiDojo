// Copyright (c) Microsoft. All rights reserved.

namespace MauiDojo.Models;

/// <summary>
/// A haiku with Japanese text, English translation, optional image, and gradient background.
/// </summary>
public sealed class Haiku
{
    public IReadOnlyList<string> Japanese { get; set; } = [];
    public IReadOnlyList<string> English { get; set; } = [];
    public string? ImageName { get; set; }
    public string Gradient { get; set; } = string.Empty;

    /// <summary>Valid image names for haiku backgrounds.</summary>
    public static readonly string[] ValidImageNames =
    [
        "osaka_castle.jpg",
        "tokyo_skyline.jpg",
        "itsukushima_shrine.jpg",
        "takachiho_gorge.jpg",
        "bonsai_tree.jpg",
        "shirakawa_go.jpg",
        "ginkaku_ji.jpg",
        "senso_ji.jpg",
        "cherry_blossoms.jpg",
        "mount_fuji.jpg",
    ];

    /// <summary>Creates a placeholder haiku.</summary>
    public static Haiku CreatePlaceholder() => new()
    {
        Japanese = ["古池や", "蛙飛び込む", "水の音"],
        English = ["An old silent pond", "A frog jumps into the pond", "Splash! Silence again"],
        Gradient = "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
    };
}
