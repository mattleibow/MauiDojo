// Copyright (c) Microsoft. All rights reserved.

using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiDojo.ViewModels;

public partial class AgenticChatViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Agentic Chat";
}
