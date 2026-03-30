// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class PredictiveStateUpdatesPage : ContentPage
{
    public PredictiveStateUpdatesPage(PredictiveStateUpdatesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
