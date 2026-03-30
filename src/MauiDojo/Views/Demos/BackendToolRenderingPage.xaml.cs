// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class BackendToolRenderingPage : ContentPage
{
    public BackendToolRenderingPage(BackendToolRenderingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
