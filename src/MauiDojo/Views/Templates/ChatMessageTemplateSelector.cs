// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.Agent.Maui.Services;
using Microsoft.Extensions.AI;

namespace MauiDojo.Views.Templates;

/// <summary>
/// Selects a DataTemplate based on the content of a <see cref="ChatMessageViewModel"/>.
/// Inspects <see cref="ChatMessageViewModel.Contents"/> for known function calls
/// to render inline cards (weather, plan) instead of plain text bubbles.
/// </summary>
public class ChatMessageTemplateSelector : DataTemplateSelector
{
    public DataTemplate? UserMessageTemplate { get; set; }
    public DataTemplate? AssistantTextTemplate { get; set; }
    public DataTemplate? WeatherCardTemplate { get; set; }
    public DataTemplate? PlanCardTemplate { get; set; }
    public DataTemplate? DefaultTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is not ChatMessageViewModel vm)
            return DefaultTemplate ?? new DataTemplate();

        if (vm.IsUser)
            return UserMessageTemplate ?? DefaultTemplate ?? new DataTemplate();

        // Check for known function calls to render as inline cards
        foreach (var content in vm.Contents)
        {
            if (content is FunctionCallContent fcc)
            {
                switch (fcc.Name)
                {
                    case "get_weather":
                        if (WeatherCardTemplate is not null) return WeatherCardTemplate;
                        break;
                    case "create_plan":
                        if (PlanCardTemplate is not null) return PlanCardTemplate;
                        break;
                }
            }
        }

        if (!string.IsNullOrEmpty(vm.Text))
            return AssistantTextTemplate ?? DefaultTemplate ?? new DataTemplate();

        return DefaultTemplate ?? new DataTemplate();
    }
}
