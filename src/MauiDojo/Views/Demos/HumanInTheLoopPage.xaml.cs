// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class HumanInTheLoopPage : ContentPage
{
    public HumanInTheLoopPage()
    {
        InitializeComponent();

        try
        {
            BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<HumanInTheLoopViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"HumanInTheLoopPage: VM error: {ex.Message}");
        }
    }
}