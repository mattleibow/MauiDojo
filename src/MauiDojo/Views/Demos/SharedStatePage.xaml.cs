// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.Models;
using MauiDojo.ViewModels;
using Microsoft.Extensions.AI;

namespace MauiDojo.Views.Demos;

public partial class SharedStatePage : ContentPage
{
    public SharedStatePage()
    {
        InitializeComponent();
        try
        {
            BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<SharedStateViewModel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SharedStatePage: {ex.Message}");
        }
    }

    private async void OnSendClicked(object? sender, EventArgs e)
    {
        var session = (BindingContext as dynamic)?.Session as MauiDojo.Agent.Maui.Services.IAgentSession;
        if (session is null) return;
        var text = InputEntry.Text?.Trim();
        if (string.IsNullOrEmpty(text)) return;
        InputEntry.Text = string.Empty;
        await session.SendAsync(new ChatMessage(ChatRole.User, text));
    }

    private void OnPreferenceCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox cb && cb.BindingContext is string pref)
        {
            (BindingContext as SharedStateViewModel)?.TogglePreferenceCommand.Execute(pref);
        }
    }

    private void OnRemoveIngredientClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is Ingredient ingredient)
        {
            (BindingContext as SharedStateViewModel)?.RemoveIngredientCommand.Execute(ingredient);
        }
    }

    private void OnRemoveInstructionClicked(object? sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is string instruction)
        {
            (BindingContext as SharedStateViewModel)?.RemoveInstructionCommand.Execute(instruction);
        }
    }
}