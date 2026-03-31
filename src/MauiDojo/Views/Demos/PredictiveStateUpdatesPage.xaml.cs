// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class PredictiveStateUpdatesPage : ContentPage
{
    public PredictiveStateUpdatesPage()
    {
        InitializeComponent();

        try
        {
            BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<PredictiveStateUpdatesViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PredictiveStateUpdatesPage: VM error: {ex.Message}");
        }
    }
}