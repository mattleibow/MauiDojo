// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.Agent.Maui.Services;
using MauiDojo.ViewModels;
using MauiDojo.Views.Templates;
using Microsoft.Extensions.AI;

namespace MauiDojo.Views.Demos;

public partial class BackendToolRenderingPage : ContentPage
{
    private readonly BackendToolRenderingViewModel _viewModel;

    public BackendToolRenderingPage(BackendToolRenderingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        // Listen for new messages to inject weather cards
        _viewModel.Session.ResponseUpdated += OnResponseUpdated;
    }

    private void OnResponseUpdated()
    {
        // Weather cards are rendered inline by the code-behind when we detect
        // FunctionCallContent with name "get_weather" in the messages.
        // The XAML CollectionView handles basic text; weather cards are added
        // by the WeatherCardTemplate binding to InvocationContext.
    }
}
