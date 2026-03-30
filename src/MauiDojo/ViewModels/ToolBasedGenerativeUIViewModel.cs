// Copyright (c) Microsoft. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDojo.ViewModels;

public partial class ToolBasedGenerativeUIViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Tool-Based Generative UI";
}
