// Copyright (c) Microsoft. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDojo.ViewModels;

public partial class PredictiveStateUpdatesViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Predictive State Updates";
}
