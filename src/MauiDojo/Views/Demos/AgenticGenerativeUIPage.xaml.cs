// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class AgenticGenerativeUIPage : ContentPage
{
    public AgenticGenerativeUIPage()
    {
        InitializeComponent();

        try
        {
            BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<AgenticGenerativeUIViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AgenticGenerativeUIPage: VM error: {ex.Message}");
        }
    }
}