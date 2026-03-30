// Copyright (c) Microsoft. All rights reserved.

namespace MauiDojo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState) =>
        new(new AppShell());
}
