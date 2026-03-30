// Copyright (c) Microsoft. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDojo.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "MAUI Dojo";
}
