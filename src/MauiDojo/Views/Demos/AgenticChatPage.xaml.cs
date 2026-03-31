// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.ViewModels;

namespace MauiDojo.Views.Demos;

public partial class AgenticChatPage : ContentPage
{
    public AgenticChatPage()
    {
        InitializeComponent();

        try
        {
            BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<AgenticChatViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AgenticChatPage: VM error: {ex.Message}");
        }
    }
}