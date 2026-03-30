// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class HumanInTheLoopPage : ContentPage
{
    public HumanInTheLoopPage(HumanInTheLoopViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
