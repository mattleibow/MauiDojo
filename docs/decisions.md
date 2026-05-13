# Decisions Log

Every significant architectural and implementation decision, with rationale.

## D1: Two-project solution (library + app)

**Decision**: Split into `MauiDojo.Agent.Maui` (reusable library) and `MauiDojo` (demo app).

**Why**: The AG-UI infrastructure (IAgentSession, ChatControl, message templates) is generic and
could be used by any MAUI app connecting to an AG-UI server. App-specific code (demo pages,
ViewModels, data models) stays in the app project.

**What goes where**:
- Library: IAgentSession, AgentSession, ChatMessageViewModel, InvocationContext, converters, controls
- App: ViewModels, demo pages, data models (WeatherInfo, Recipe, Plan), DemoService

## D2: MVVM with CommunityToolkit.Mvvm

**Decision**: Use `[ObservableProperty]`, `[RelayCommand]` source generators instead of manual INotifyPropertyChanged.

**Why**: Reduces boilerplate significantly. Every ViewModel is a `partial class` inheriting `ObservableObject`.

## D3: Shell with locked flyout

**Decision**: Use `Shell.FlyoutBehavior="Locked"` with all 7 demos as `ShellContent` items.

**Why**: Matches the Blazor app's always-visible sidebar. `FlyoutWidth="280"` gives enough room
for demo titles. Each demo is a separate page navigated via the flyout.

## D4: Parameterless page constructors

**Decision**: All pages use parameterless constructors and resolve ViewModels from DI in the constructor.

**Why**: MAUI Shell `ContentTemplate="{DataTemplate ...}"` instantiates pages via parameterless constructors.
DI-injected constructor parameters cause XAML crashes. We resolve VMs via
`App.Current?.Handler?.MauiContext?.Services.GetService<T>()` with try/catch.

## D5: No x:DataType compiled bindings

**Decision**: Removed `x:DataType` from all demo pages.

**Why**: ViewModels are resolved at runtime, not at compile time. `x:DataType` causes build errors
when the ViewModel isn't directly referenced in XAML. Runtime bindings work fine for this app.

## D6: Inline colors instead of StaticResource

**Decision**: Use hardcoded hex colors in XAML (`BackgroundColor="#6366F1"`) instead of `{StaticResource Primary}`.

**Why**: The Shell flyout header was crashing when referencing `StaticResource` colors — the resource
dictionaries may not be loaded when Shell initializes. Inline colors are more reliable and
this is a demo app, not a production theming scenario.

## D7: ToChatMessage() filters tool content from history

**Decision**: When rebuilding chat history for subsequent messages, only include `TextContent` and
`DataContent`. Exclude `FunctionCallContent` and `FunctionResultContent`.

**Why**: The AG-UI server couldn't handle tool call/result messages in the history on subsequent turns.
Including them caused the server to return empty responses (the "only first message works" bug).
Tool execution is handled by `FunctionInvokingChatClient` within a single `RunStreamingAsync` call.

**Exception**: `DataContent` IS preserved because the Shared State demo sends recipe JSON as `DataContent`.

## D8: SystemInstructions on IAgentSession (not agent wrapper)

**Decision**: Added `IAgentSession.SystemInstructions` property, prepended as a System ChatMessage
in the history. Abandoned the `HumanInTheLoopAgent` `DelegatingAIAgent` wrapper approach.

**Why**: The `DelegatingAIAgent` used `new` keyword (not `override`) because the `IEnumerable<ChatMessage>`
overload is non-virtual in the rc2 API. Since `AgentSession._agent` is typed as `AIAgent`, the `new`
methods were never called — the system instructions were silently dropped. The model never saw
the tool-usage instructions, so it responded with plain text instead of calling `create_plan`/`confirm_plan`.

Prepending a System ChatMessage directly in the history is simple and always works.

## D9: Plan card above chat (not inline via DataTemplateSelector)

**Decision**: The HITL plan card renders above the message list, bound to ViewModel state
(`HasPlan`, `IsAwaitingConfirmation`), not inline via `ChatMessageTemplateSelector`.

**Why**: `FunctionInvokingChatClient` swallows `FunctionCallContent` — tool calls are executed
internally and never appear in the streaming updates that `AgentSession` receives. The
`ChatMessageTemplateSelector` never sees `create_plan` in `ChatMessageViewModel.Contents`,
so it can't pick the plan card template. The weather card works differently because
`FunctionResultContent` IS visible in the stream (the server returns the result, not the local tool).

## D10: Weather card inline via DataTemplateSelector

**Decision**: Weather cards render inline in the CollectionView using `ChatMessageTemplateSelector`
+ `InlineWeatherCardView`.

**Why**: The `get_weather` tool executes on the SERVER. The AG-UI protocol streams back
`FunctionCallContent` and `FunctionResultContent` which ARE visible to `AgentSession`. The
`InlineWeatherCardView` extracts location from `FunctionCallContent.Arguments` and weather data
from `FunctionResultContent.Result`.

## D11: NuGet version strategy — floating versions

**Decision**: Use floating version ranges (`1.0.0-*`, `10.*`, `8.*`, `*`) for most packages.

**Why**: The Agent Framework packages are in active preview. Pinning to specific versions caused
assembly binding failures at runtime (e.g., `Microsoft.Extensions.AI 10.3.0` loaded but
`AGUIChatClient` needed `10.4.0`). Floating versions resolve to the latest compatible version.

## D12: MauiDevFlow for runtime debugging

**Decision**: Integrated `Redth.MauiDevFlow.Agent` NuGet package, registered with
`builder.AddMauiDevFlowAgent()` in `#if DEBUG`.

**Why**: Enables `maui-devflow MAUI screenshot`, `tap`, `tree`, `logs` from the terminal for
automated testing without a visual debugger. Critical for CI-style validation.

## D13: Suggestion button text vs message mapping

**Decision**: Each page's code-behind has a `Dictionary<string, string> SuggestionMessages` that
maps button text to the full message sent to the agent.

**Why**: The Blazor app's `Suggestion` class has `(text, message)` where button text is short
("Simple plan") but the sent message is long ("Create a simple 5-step plan for organizing a birthday party").
We replicate this with a static dictionary in the code-behind.

## D14: Microsoft.NET.Sdk (not Microsoft.NET.Sdk.Maui)

**Decision**: Use `<Project Sdk="Microsoft.NET.Sdk">` with `<UseMaui>true</UseMaui>`.

**Why**: The official .NET 10 MAUI template uses `Microsoft.NET.Sdk`, not `Microsoft.NET.Sdk.Maui`.
Using the wrong SDK caused workload resolver failures. Discovered by creating a `dotnet new maui`
reference template and comparing.

## D15: Multi-TFM with platform-specific conditional

**Decision**: Use conditional TargetFrameworks for cross-platform:
```xml
<TargetFrameworks>net10.0-android</TargetFrameworks>
<TargetFrameworks Condition="!$([MSBuild]::IsOSPlatform('linux'))">$(TargetFrameworks);net10.0-ios;net10.0-maccatalyst</TargetFrameworks>
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>
```

**Why**: Matches the official template. iOS/Mac targets only on non-Linux, Windows target only on Windows.
