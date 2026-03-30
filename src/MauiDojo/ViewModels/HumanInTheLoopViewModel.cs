// Copyright (c) Microsoft. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDojo.ViewModels;

public partial class HumanInTheLoopViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Human in the Loop";
}
