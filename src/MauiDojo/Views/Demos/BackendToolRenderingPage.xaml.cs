// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class BackendToolRenderingPage : ContentPage
{
    public BackendToolRenderingPage()
    {
        InitializeComponent();

        try
        {
            BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<BackendToolRenderingViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BackendToolRenderingPage: VM error: {ex.Message}");
        }
    }
}