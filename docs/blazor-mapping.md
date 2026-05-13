# Blazor-to-MAUI Mapping

How each Blazor concept was translated to MAUI.

## Component Mapping

| Blazor Concept | MAUI Equivalent | Notes |
|----------------|-----------------|-------|
| `AgentBoundary` | `IAgentSession` (ViewModel-owned) | Blazor cascades context via component tree; MAUI uses DI service |
| `AgentBoundaryContext` | `AgentSession` class | Same responsibilities: streaming, tools, HITL, state events |
| `Messages` component | `CollectionView` with `ItemTemplate` | Blazor renders via `ContentTemplates`; MAUI uses `DataTemplateSelector` |
| `ContentTemplateBase` | `DataTemplateSelector` + `DataTemplate` | Blazor matches by `When()` method; MAUI matches by inspecting `Contents` |
| `InvocationContext` (Blazor) | `InvocationContext` (library) | Same purpose: tracks function call → result pair |
| `AgentInput` | `Entry` + `Button` in XAML | Direct translation |
| `AgentSuggestions` | `HorizontalStackLayout` with `Button` items | Code-behind maps button text → full message via `SuggestionMessages` dictionary |
| `AgentLoadingIndicator` | `ActivityIndicator` or custom control | Bound to `Session.IsProcessing` |
| `AgentState<TState>` | `Session.StateSnapshotReceived` event | Blazor uses a component; MAUI uses events on ViewModel |
| `CascadingParameter` | DI + `BindingContext` | No cascading in MAUI; ViewModels injected via DI |
| `@bind` (two-way) | `{Binding ..., Mode=TwoWay}` | Standard MAUI binding |
| `StateHasChanged()` | `OnPropertyChanged()` / `[ObservableProperty]` | CommunityToolkit source generators handle this |
| Razor routing (`@page`) | Shell routes (`Route="AgenticChat"`) | Shell manages navigation via flyout |
| CSS scoped styles | XAML `Style` resources + inline attributes | No CSS in MAUI |
| JS Interop | Not needed | Native controls handle auto-scroll, auto-resize |

## Tool Registration

| Blazor | MAUI |
|--------|------|
| `context.RegisterTools(tool)` in `OnContextCreated` | `Session.RegisterTools(tool)` in ViewModel constructor |
| Tools passed in `AgentBoundaryContext.SendAsync` | Tools passed via `ChatClientAgentRunOptions.ChatOptions.Tools` |
| `AIFunctionFactory.Create(method, name, description)` | Same — `AIFunctionFactory.Create(method, name, description)` |

## Human-in-the-Loop Pattern

| Blazor | MAUI |
|--------|------|
| `await context.WaitForResponse("key")` in tool | `await Session.WaitForResponse("key")` in tool |
| `context.ProvideResponse("key", result)` in UI | `Session.ProvideResponse("key", result)` via `[RelayCommand]` |
| `SubscribeToResponseUpdates()` to detect tool calls | Not needed — tools are auto-executed by `FunctionInvokingChatClient` |

## State Synchronization

| Blazor | MAUI |
|--------|------|
| `AgentState<TState>` component with `OnSnapshot`/`OnDelta` callbacks | `Session.StateSnapshotReceived` / `StateDeltaReceived` events on ViewModel |
| Cascading `TState` to child components | ViewModel property (e.g., `CurrentPlan`, `CurrentDocument`) with `[ObservableProperty]` |
| JSON Patch via `PlanPatcher.Apply()` | Inline patch logic in ViewModel's `OnStateDeltaReceived` |

## Content Template Rendering

### Blazor approach:
```csharp
// In AgentBoundaryContext, each message's content is iterated:
// ContentTemplateBase.When(context) is called for each template
// First matching template renders the content

<Messages>
    <ContentTemplates>
        <WeatherCallTemplate />    // Matches FunctionCallContent with name="get_weather"
    </ContentTemplates>
</Messages>
```

### MAUI approach:
```csharp
// ChatMessageTemplateSelector inspects ChatMessageViewModel.Contents:
// If FunctionCallContent with specific name found → picks specialized DataTemplate
// Otherwise → UserMessageTemplate or AssistantTextTemplate

<CollectionView ItemTemplate="{StaticResource MessageTemplateSelector}">
```

### Key difference:
Blazor's `FunctionInvokingChatClient` yields `FunctionCallContent` in the stream. But in our MAUI app,
the `FunctionInvokingChatClient` for **local tools** (like `create_plan`) swallows the tool call —
only the final text response appears. **Server tools** (like `get_weather`) DO yield
`FunctionCallContent`/`FunctionResultContent` because the server executes them.

This means:
- **Server tool results** (weather) → inline via DataTemplateSelector ✅
- **Local tool results** (plan) → bound to ViewModel state, shown above/below chat ✅
