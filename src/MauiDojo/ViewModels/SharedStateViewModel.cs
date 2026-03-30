// Copyright (c) Microsoft. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDojo.ViewModels;

public partial class SharedStateViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Shared State";
}
