# Architecture & Design

## Goals

Port the Blazor AGUIDojo sample to native .NET MAUI so that:
1. The same AG-UI server works unchanged — MAUI connects to `http://localhost:5018`
2. All 7 demo scenarios reproduce the Blazor behavior using native MAUI controls
3. A reusable class library (`MauiDojo.Agent.Maui`) provides AG-UI infrastructure that any MAUI app can use
4. The app uses .NET 10 with C# 14 features and modern MAUI patterns

## Solution Structure

```
MauiDojo.slnx
├── src/MauiDojo.Agent.Maui/          # Reusable MAUI class library
│   ├── Services/
│   │   ├── IAgentSession.cs          # Core interface (port of Blazor's IAgentBoundaryContext)
│   │   ├── AgentSession.cs           # Streaming, tool execution, state events, HITL
│   │   ├── IAgentSessionFactory.cs   # Factory for DI
│   │   ├── AgentSessionFactory.cs
│   │   ├── ChatMessageViewModel.cs   # Observable message model for MAUI binding
│   │   └── InvocationContext.cs      # Tracks tool call → result pairs
│   ├── Controls/
│   │   ├── ChatControl.xaml          # Reusable chat UI (not yet used by demos)
│   │   ├── SuggestionBar.xaml        # Horizontal suggestion buttons
│   │   └── AgentLoadingIndicator.xaml
│   ├── Templates/
│   │   ├── MessageTemplateSelector.cs
│   │   ├── ContentTemplateSelector.cs
│   │   ├── UserMessageTemplate.xaml
│   │   └── AssistantMessageTemplate.xaml
│   ├── Converters/
│   │   ├── BoolToVisibilityConverter.cs
│   │   └── InverseBoolConverter.cs
│   ├── Extensions/
│   │   └── AgentMauiExtensions.cs    # AddAgentSession() DI helper
│   └── Suggestion.cs                # record Suggestion(Text, Message?)
│
├── src/MauiDojo/                     # Demo app
│   ├── MauiProgram.cs               # DI: 7 keyed AIAgent registrations
│   ├── AppShell.xaml                # Shell with locked flyout, 7 routes
│   ├── Models/                      # 14 data models (WeatherInfo, Recipe, Plan, etc.)
│   ├── Services/
│   │   ├── DemoService.cs           # Scenario registry
│   │   ├── BackgroundColorService.cs # For Agentic Chat color tool
│   │   └── HumanInTheLoopAgent.cs   # (deprecated — instructions via Session)
│   ├── ViewModels/                  # 7 demo ViewModels + MainViewModel
│   ├── Views/
│   │   ├── Demos/                   # 7 demo pages (XAML + code-behind)
│   │   └── Templates/              # Inline content templates
│   │       ├── ChatMessageTemplateSelector.cs  # DataTemplateSelector for message types
│   │       ├── InlineWeatherCardView.xaml       # Weather card ContentView
│   │       └── WeatherCardTemplate.xaml         # (legacy, unused)
│   ├── Converters/                  # App-specific value converters
│   ├── Resources/                   # Colors, Styles, Fonts, Images, Icons
│   └── Platforms/                   # Android, iOS, Mac, Windows entry points
```

## Key Abstractions

### IAgentSession (replaces Blazor's IAgentBoundaryContext)

The central service managing a conversation with an AG-UI agent:

```csharp
public interface IAgentSession : INotifyPropertyChanged
{
    ObservableCollection<ChatMessageViewModel> Messages { get; }
    ObservableCollection<ChatMessageViewModel> PendingMessages { get; }
    bool IsProcessing { get; }

    void RegisterTool(AITool tool);
    string? SystemInstructions { get; set; }

    Task SendAsync(params ChatMessage[] messages);

    // Human-in-the-loop
    Task<object> WaitForResponse(string key);
    void ProvideResponse(string key, object response);

    // State synchronization
    event Action<ReadOnlyMemory<byte>>? StateSnapshotReceived;
    event Action<ReadOnlyMemory<byte>>? StateDeltaReceived;
    event Action? ResponseUpdated;
}
```

### ChatMessageViewModel

Observable wrapper around `ChatMessage` for MAUI data binding. Exposes `Role`, `Text`,
`Contents` (collection of `AIContent`), and computed `IsUser`/`IsAssistant`.

Key method: `ToChatMessage()` reconstructs a `ChatMessage` for history, filtering out
`FunctionCallContent`/`FunctionResultContent` (only keeping `TextContent` and `DataContent`).

### ChatMessageTemplateSelector

A `DataTemplateSelector` that inspects message content to pick the right template:
- User messages → purple right-aligned bubble
- Assistant text → gray left-aligned bubble
- Messages with `get_weather` FunctionResultContent → inline weather card
- Messages with `create_plan` → plan card (future)

## Data Flow

```
User taps suggestion → code-behind looks up full message text
  → Session.SendAsync(new ChatMessage(ChatRole.User, text))
    → History rebuilt (text + data only, no tool calls)
    → System instructions prepended if set
    → Tools passed via ChatClientAgentRunOptions.ChatOptions.Tools
    → agent.RunStreamingAsync(history, options)
      → AGUIChatClient sends HTTP POST to server with SSE response
      → FunctionInvokingChatClient auto-executes local tools
      → Each streaming update yields AgentResponseUpdate
        → TextContent → appends to pending message Text
        → FunctionCallContent → tracked in InvocationContext
        → FunctionResultContent → result set on InvocationContext
        → DataContent → fires StateSnapshotReceived / StateDeltaReceived
    → Pending message promoted to Messages when stream ends
```

## AG-UI Protocol

The AG-UI protocol uses Server-Sent Events (SSE) over HTTP POST. Events include:
- `RUN_STARTED` / `RUN_FINISHED` / `RUN_ERROR`
- `TEXT_MESSAGE_START` / `TEXT_MESSAGE_CONTENT` / `TEXT_MESSAGE_END`
- `TOOL_CALL_START` / `TOOL_CALL_ARGS` / `TOOL_CALL_END` / `TOOL_CALL_RESULT`
- `STATE_SNAPSHOT` / `STATE_DELTA`

The `AGUIChatClient` handles this protocol transparently. Our `AgentSession` consumes
`AgentResponseUpdate` objects which abstract away the SSE details.
