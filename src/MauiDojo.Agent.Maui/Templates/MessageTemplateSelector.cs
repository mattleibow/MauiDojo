// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.Agent.Maui.Services;
using Microsoft.Extensions.AI;

namespace MauiDojo.Agent.Maui.Templates;

/// <summary>
/// Selects a <see cref="DataTemplate"/> based on the <see cref="ChatMessageViewModel.Role"/>.
/// </summary>
public class MessageTemplateSelector : DataTemplateSelector
{
    /// <summary>Template used for <see cref="ChatRole.User"/> messages.</summary>
    public DataTemplate? UserMessageTemplate { get; set; }

    /// <summary>Template used for <see cref="ChatRole.Assistant"/> messages.</summary>
    public DataTemplate? AssistantMessageTemplate { get; set; }

    /// <summary>Template used for <see cref="ChatRole.System"/> messages.</summary>
    public DataTemplate? SystemMessageTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is not ChatMessageViewModel vm)
            return AssistantMessageTemplate ?? new DataTemplate();

        if (vm.Role == ChatRole.User)
            return UserMessageTemplate ?? new DataTemplate();

        if (vm.Role == ChatRole.System)
            return SystemMessageTemplate ?? AssistantMessageTemplate ?? new DataTemplate();

        return AssistantMessageTemplate ?? new DataTemplate();
    }
}
