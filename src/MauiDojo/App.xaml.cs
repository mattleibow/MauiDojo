// Copyright (c) Microsoft. All rights reserved.

namespace MauiDojo;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Catch unhandled exceptions for diagnostics
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            var ex = e.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine($"UNHANDLED: {ex}");
            File.WriteAllText(
                Path.Combine(FileSystem.AppDataDirectory, "crash.log"),
                $"{DateTime.Now}\n{ex}");
        };
    }

    protected override Window CreateWindow(IActivationState? activationState) =>
        new(new AppShell());
}
