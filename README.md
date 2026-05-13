# MAUI Dojo

A native .NET MAUI port of the [AGUIDojo](https://github.com/microsoft/agents) Blazor Server sample,
demonstrating the Microsoft Agent Framework's AG-UI protocol with native MAUI controls.

## What Is This?

The original **AGUIDojo** is a Blazor Server app that showcases 7 interactive AI demo scenarios using
the AG-UI (Agent User Interface) protocol. This project recreates those same 7 demos as a native
.NET MAUI application, replacing Blazor components with XAML views, MVVM ViewModels, and native controls.

The MAUI app communicates with the **same AG-UI server** as the Blazor app — no server changes required.

## Architecture

```
┌─────────────────────────────────────────┐
│  MauiDojo (MAUI App)                    │
│  ├── Views/Demos/ (7 XAML pages)        │
│  ├── ViewModels/ (MVVM with toolkit)    │
│  └── Models/ (shared data types)        │
├─────────────────────────────────────────┤
│  MauiDojo.Agent.Maui (Class Library)    │
│  ├── Services/ (IAgentSession, etc.)    │
│  ├── Controls/ (ChatControl, etc.)      │
│  └── Templates/ (message rendering)     │
├─────────────────────────────────────────┤
│  AGUIChatClient (NuGet package)         │
│  HTTP/SSE → AG-UI Server (localhost:5018)│
└─────────────────────────────────────────┘
```

## Quick Start

1. Start the AG-UI server: `cd AGUIDojoServer && dotnet run` (port 5018)
2. Build the MAUI app: `dotnet build src/MauiDojo -f net10.0-windows10.0.19041.0`
3. Run: `src/MauiDojo/bin/Debug/net10.0-windows10.0.19041.0/win-x64/MauiDojo.exe`

## Documentation

- [Architecture & Design](docs/architecture.md) — Solution structure, component design, key abstractions
- [Decisions Log](docs/decisions.md) — Every architectural decision and why
- [Demo Status](docs/demo-status.md) — Current state of each demo, what works, what doesn't
- [Blazor-to-MAUI Mapping](docs/blazor-mapping.md) — How Blazor patterns map to MAUI
- [Known Issues & Future Work](docs/future-work.md) — What still needs doing
- [Development Guide](docs/dev-guide.md) — How to build, test, debug with MauiDevFlow

## Key Dependencies

| Package | Purpose |
|---------|---------|
| `Microsoft.Agents.AI` | Core agent abstractions (AIAgent, ChatClientAgent) |
| `Microsoft.Agents.AI.AGUI` | AG-UI protocol client (AGUIChatClient, SSE streaming) |
| `Microsoft.Extensions.AI` | IChatClient, AITool, AIFunctionFactory |
| `CommunityToolkit.Mvvm` | MVVM source generators ([ObservableProperty], [RelayCommand]) |
| `CommunityToolkit.Maui` | MAUI toolkit (converters, behaviors) |
| `Redth.MauiDevFlow.Agent` | Runtime UI inspection and debugging |