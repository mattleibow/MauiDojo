// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class AgenticChatPage : ContentPage
{
    public AgenticChatPage(AgenticChatViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
