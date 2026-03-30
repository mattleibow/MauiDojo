// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class AgenticGenerativeUIPage : ContentPage
{
    public AgenticGenerativeUIPage(AgenticGenerativeUIViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
