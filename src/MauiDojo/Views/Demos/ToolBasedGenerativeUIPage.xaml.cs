// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class ToolBasedGenerativeUIPage : ContentPage
{
    public ToolBasedGenerativeUIPage(ToolBasedGenerativeUIViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
