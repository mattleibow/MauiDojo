# Development Guide

## Prerequisites

- .NET 10 SDK (preview)
- MAUI workload: `dotnet workload install maui`
- The AGUIDojo server running at `http://localhost:5018`
- Azure OpenAI credentials configured for the server

## Building

```bash
# Restore packages
dotnet restore src/MauiDojo/MauiDojo.csproj

# Build for Windows
dotnet build src/MauiDojo/MauiDojo.csproj -f net10.0-windows10.0.19041.0 -c Debug

# Build for Android (on any OS)
dotnet build src/MauiDojo/MauiDojo.csproj -f net10.0-android -c Debug

# Build for iOS/Mac (macOS only)
dotnet build src/MauiDojo/MauiDojo.csproj -f net10.0-ios -c Debug
dotnet build src/MauiDojo/MauiDojo.csproj -f net10.0-maccatalyst -c Debug
```

## Running

### Windows
```bash
# Build first, then launch the exe directly
dotnet build src/MauiDojo/MauiDojo.csproj -f net10.0-windows10.0.19041.0
src/MauiDojo/bin/Debug/net10.0-windows10.0.19041.0/win-x64/MauiDojo.exe
```

### Server
The MAUI app connects to `http://localhost:5018` by default (hardcoded in `MauiProgram.cs`).
Start the AGUIDojo server first:
```bash
cd /path/to/AGUIDojoServer
dotnet run
```

## Debugging with MauiDevFlow

MauiDevFlow is integrated for runtime UI inspection. Install the CLI:
```bash
dotnet tool install --global Redth.MauiDevFlow.CLI
```

### Common commands
```bash
# Check if agent is connected
maui-devflow list

# Take a screenshot
maui-devflow MAUI screenshot --output screen.png

# Inspect the visual tree
maui-devflow MAUI tree --depth 15 --fields "id,type,text"

# Tap a button by text
maui-devflow MAUI tap --text "Simple plan"

# Tap a flyout item
maui-devflow MAUI tap FlyoutItem_IMPL_HumanInTheLoop

# Check app logs
maui-devflow MAUI logs --limit 20

# Navigate to a demo
maui-devflow MAUI tap FlyoutItem_IMPL_BackendToolRendering
```

### Flyout item IDs
Each demo has a predictable flyout item ID:
- `FlyoutItem_IMPL_AgenticChat`
- `FlyoutItem_IMPL_BackendToolRendering`
- `FlyoutItem_IMPL_HumanInTheLoop`
- `FlyoutItem_IMPL_AgenticGenerativeUI`
- `FlyoutItem_IMPL_ToolBasedGenerativeUI`
- `FlyoutItem_IMPL_SharedState`
- `FlyoutItem_IMPL_PredictiveStateUpdates`

### Debugging tips
- `maui-devflow MAUI logs` captures `ILogger` output and `Console.WriteLine`
- `System.Diagnostics.Debug.WriteLine` is NOT captured — use `Console.WriteLine` or `ILogger` instead
- For detailed debugging, write to a log file:
  ```csharp
  File.AppendAllText(Path.Combine(FileSystem.AppDataDirectory, "debug.log"), $"{DateTime.Now}: message\n");
  ```
  Read it with: `Get-Content "$env:LOCALAPPDATA\User Name\com.microsoft.mauidojo\Data\debug.log"`

## Project Configuration

### NuGet packages (floating versions)
The Agent Framework packages use floating version ranges because they're in active preview:
```xml
<PackageReference Include="Microsoft.Agents.AI" Version="1.0.0-*" />
<PackageReference Include="Microsoft.Agents.AI.AGUI" Version="1.0.0-*" />
<PackageReference Include="Microsoft.Extensions.AI" Version="10.*" />
```

### MauiDevFlow (Debug only)
```csharp
#if DEBUG
builder.AddMauiDevFlowAgent();
#endif
```

### Server URL
Hardcoded in `MauiProgram.cs`:
```csharp
string serverUrl = "http://localhost:5018";
```

## Adding a New Demo

1. Create a ViewModel in `ViewModels/` extending `ObservableObject`
2. Inject `[FromKeyedServices("key")] AIAgent agent` + `IAgentSessionFactory`
3. Create `Session = factory.Create(agent)` and register any tools
4. Create a XAML page in `Views/Demos/` with parameterless constructor
5. Resolve ViewModel: `BindingContext = App.Current?.Handler?.MauiContext?.Services.GetService<MyViewModel>()`
6. Register the keyed agent in `MauiProgram.cs`
7. Add `<ShellContent>` in `AppShell.xaml`
8. Register the ViewModel and Page for DI in `MauiProgram.cs`

## Commit History

| Hash | Description |
|------|-------------|
| `009671a` | Fix HITL confirm flow: system instructions + plan card with Confirm/Reject |
| `f65df41` | Inline message templates: weather card and plan card render in message stream |
| `7c7a269` | Fix multi-message chat: filter tool calls from history, add error handling |
| `0b7991a` | Match all 7 demos to Blazor original — exact UI parity |
| `93b6342` | All 7 demos with working chat UI and locked flyout sidebar |
| `3c7a33b` | Working Agentic Chat demo with streaming AI responses |
| `7442587` | Fix app startup: resolve assembly version conflicts and XAML crashes |
| `875ad80` | Fix build: update to Agent Framework rc2 API, fix image names |
| `c5fd44f` | Fix all issues from Opus 4.5 + GPT 5.4 code reviews |
| `fcc03c8` | Fix MAUI project structure to match official template |
| `352e2e8` | Implement demos 1-4: Chat, Weather, HITL, Generative UI |
| `b93afcd` | Implement demos 5-7: Haiku, Shared State, Document Editor |
| `655f0d3` | Add reusable library controls, templates, converters, and images |
| `9b7e5eb` | Add placeholder demo pages, view models, and services |
| `27a42eb` | Add core AG-UI agent session services for MAUI |
