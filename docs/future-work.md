# Known Issues & Future Work

## Known Issues

### P0 — Must fix

1. **Demos 4, 6, 7 state-driven UI not fully wired** — Server communicates correctly but
   state snapshot/delta events may not update the UI properly. The ViewModel event handlers
   (`OnStateSnapshotReceived`, `OnStateDeltaReceived`) exist but need end-to-end testing.

2. **Streaming not visually incremental** — Text appears in small chunks rather than
   character-by-character. This is a MAUI `Label` rendering behavior, not a streaming bug.
   The Blazor version appears smoother because browser DOM updates are more granular.

### P1 — Should fix

3. **Weather card loading state** — No skeleton/spinner shown while `get_weather` tool executes.
   The card simply doesn't appear until the `FunctionResultContent` arrives.

4. **Plan card not inline** — The HITL plan card renders above the message list (not inline)
   because `FunctionInvokingChatClient` swallows local tool `FunctionCallContent`. See Decision D9.

5. **Haiku gradient parsing** — CSS gradient strings like `"linear-gradient(135deg, #667eea, #764ba2)"`
   are parsed to extract only the first hex color. Full gradient rendering would require
   `LinearGradientBrush` creation at runtime.

6. **`update_plan_step` not verified** — After confirming the plan, the agent should call
   `update_plan_step` for each step. The ViewModel handler exists but hasn't been tested.

### P2 — Nice to have

7. **No "New Chat" button** — Blazor has a "+ New chat" button that calls `Reset()`. Missing from MAUI.

8. **No chat auto-scroll** — Messages list doesn't auto-scroll to bottom when new messages arrive.
   Blazor uses JS interop; MAUI would need `CollectionView.ScrollTo()` on `CollectionChanged`.

9. **Missing Blazor chat header** — Blazor shows "AGUI WebChat" header above messages. MAUI
   uses the Shell title bar instead.

10. **Images in haiku** — The Blazor demo shows optional images from bundled Japanese landscape photos.
    MAUI has the images bundled but the haiku card doesn't display them.

## Future Work

### Immediate next steps

1. **End-to-end test Demos 4, 6, 7** — Launch the app, navigate to each, tap suggestions,
   verify state updates render correctly. Use MauiDevFlow for screenshots.

2. **Add auto-scroll** — When `Session.Messages.CollectionChanged` fires, scroll the
   CollectionView to the last item.

3. **Loading states** — Add `ActivityIndicator` in weather card template that shows while
   `HasResult` is false on the `InvocationContext`.

### Medium-term improvements

4. **Refactor to use ChatControl** — The library has `ChatControl.xaml` but demos build
   their chat UI inline. Refactor demos to use the reusable control.

5. **Responsive layout** — Split-pane layouts (Demos 5, 6, 7) should collapse to single-column
   on narrow screens (phones).

6. **Dark mode** — Add `AppThemeBinding` support. Colors.xaml has both light and dark values
   but they're not wired up.

7. **Android/iOS testing** — Only tested on Windows. The `net10.0-android` and `net10.0-ios`
   TFMs are configured but not built/tested.

### Architectural improvements

8. **Proper inline plan card** — Investigate using the AG-UI server's tool call events
   (which ARE visible for server-executed tools) instead of local tool execution. The Blazor
   server at `/human_in_the_loop` doesn't have tools — they're all frontend. Consider whether
   the HITL tools should be sent to the server so the AG-UI protocol handles them.

9. **Extract reusable components** — The `InlineWeatherCardView` pattern could be generalized
   to a `ContentTemplateProvider` that other apps can plug into.

10. **Source-gen JSON serialization** — The `MauiDojoSerializerContext` exists but isn't
    used everywhere. Pass it to `AGUIChatClient` and `JsonSerializer` calls for AOT support.

## Bugs Found During Development (All Fixed)

| Bug | Severity | Root Cause | Fix |
|-----|----------|-----------|-----|
| Only first message works | Critical | `ToChatMessage()` included `FunctionCallContent` in history | Filter to TextContent + DataContent only |
| App crashes on startup | Critical | `Microsoft.Extensions.AI` version 10.3.0 loaded but 10.4.0 needed | Floating version ranges (`10.*`) |
| csproj wrong SDK | Critical | Used `Microsoft.NET.Sdk.Maui` instead of `Microsoft.NET.Sdk` | Match official template |
| Missing platform files | Critical | No Android/iOS/Windows entry points | Created all 16 platform files |
| `HumanInTheLoopAgent` never called | Critical | `new` keyword invisible when `_agent` typed as `AIAgent` | Use `Session.SystemInstructions` instead |
| Tools not reaching server | Important | Tools via `AdditionalProperties["Tools"]` not read | Use `ChatClientAgentRunOptions.ChatOptions.Tools` |
| Weather card above chat | Important | Rendered as separate Frame, not inline | `DataTemplateSelector` + `InlineWeatherCardView` |
| ResourceDictionary crash | Important | `x:Class` code-behind on Colors/Styles.xaml | Removed code-behind files |
| Image filenames rejected | Build | Uppercase, hyphens in filenames | Renamed to lowercase with underscores |
| `AgentRunResponseUpdate` not found | Build | Type renamed to `AgentResponseUpdate` in rc2 | Updated all references |
| `AgentThread` not found | Build | Type renamed to `AgentSession` in rc2 | Updated all references |
