// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class SharedStatePage : ContentPage
{
    public SharedStatePage(SharedStateViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
