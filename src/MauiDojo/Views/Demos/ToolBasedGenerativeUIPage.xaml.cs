// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class ToolBasedGenerativeUIPage : ContentPage
{
    public ToolBasedGenerativeUIPage()
    {
        InitializeComponent();

        try
        {
            BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<ToolBasedGenerativeUIViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ToolBasedGenerativeUIPage: VM error: {ex.Message}");
        }
    }
}