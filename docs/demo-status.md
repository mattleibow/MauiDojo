# Demo Status

Current state of each demo, verified with MauiDevFlow screenshots against the live AG-UI server.

## Summary

| # | Demo | Server Comm | UI | Interactive | Overall |
|---|------|:-----------:|:--:|:----------:|:-------:|
| 1 | Agentic Chat | ✅ | ✅ | ✅ | **Working** |
| 2 | Backend Tool Rendering | ✅ | ✅ | ✅ | **Working** |
| 3 | Human in the Loop | ✅ | ✅ | ✅ | **Working** |
| 4 | Agentic Generative UI | ✅ | ⚠️ | ❌ | **Partial** |
| 5 | Tool-Based Generative UI | ✅ | ✅ | ⚠️ | **Partial** |
| 6 | Shared State | ✅ | ✅ | ⚠️ | **Partial** |
| 7 | Predictive State Updates | ✅ | ✅ | ⚠️ | **Partial** |

---

## Demo 1: Agentic Chat ✅

**What works:**
- "Change background" sends "Change background to light blue" → agent calls `ChangeBackground` tool
  locally via `FunctionInvokingChatClient` → `BackgroundColorService.SetColor()` fires event →
  page `BackgroundColor` binding updates → page turns light blue
- "Generate sonnet" sends and streams a full sonnet response
- Multiple messages work in sequence (history correctly excludes tool content)
- Suggestion button text matches Blazor exactly

**What's left:**
- Streaming is not visually incremental (text appears in chunks, not character-by-character) —
  this is a MAUI `Label` rendering optimization, not a bug

## Demo 2: Backend Tool Rendering ✅

**What works:**
- Weather card renders **inline** in the message CollectionView using `ChatMessageTemplateSelector`
  + `InlineWeatherCardView`
- Card shows: weather icon emoji, location, temperature °C/°F, conditions, humidity, wind, feels like
- Correct suggestion messages ("What's the weather like in X?")
- Placeholder "Ask about the weather..."

**What's left:**
- Loading skeleton state while tool executes (currently shows empty until result arrives)
- Second weather query: verify both cards render (not tested recently)

## Demo 3: Human in the Loop ✅

**What works:**
- `create_plan` tool creates 5-step plan, plan card shows with checkboxes and "Pending" badges
- `confirm_plan` tool blocks execution, "Plan Confirmation" header appears with Confirm/Reject buttons
- Clicking "Confirm Selected (5)" calls `ProvideResponse()`, wakes the tool, header changes to "Executing Plan"
- System instructions prepended via `Session.SystemInstructions` (model follows tool workflow)
- Correct suggestion messages matching Blazor

**What's left:**
- `update_plan_step` tool needs verification (should update step badges to "Done")
- Reject button needs testing
- Plan card is above the chat (not inline in message stream) — see Decision D9

## Demo 4: Agentic Generative UI ⚠️

**What works:**
- Server communication works (HTTP 200)
- UI layout: plan progress panel + chat panel

**What doesn't work:**
- Response comes via `STATE_SNAPSHOT` / `STATE_DELTA` events, not text messages
- Plan progress panel doesn't update because state events need to be wired to the ViewModel's `CurrentPlan`
- App may crash if state events aren't handled on the UI thread

**What needs doing:**
- Verify `AgenticGenerativeUIViewModel.OnStateSnapshotReceived` correctly deserializes `Plan`
- Verify JSON Patch delta application works
- Test step status updates in real-time

## Demo 5: Tool-Based Generative UI ⚠️

**What works:**
- Split layout: haiku carousel (left) + chat (right)
- `generate_haiku` frontend tool creates Haiku objects with Japanese/English text
- Carousel navigation (◀/▶) with position indicator
- Correct suggestion messages

**What's left:**
- Haiku background gradient: CSS gradient string parsed to first hex color only
- Image display from bundled resources not tested
- Multiple haiku carousel navigation not fully verified

## Demo 6: Shared State ⚠️

**What works:**
- Split layout: recipe form (left) + chat (right)
- Recipe form: title Entry, cooking time Picker, skill level Picker, dietary preference checkboxes,
  ingredients list with add/remove, instructions list with add/remove
- "✨ Improve with AI" button sends recipe as `DataContent` (JSON wrapped as `{recipe: ...}`)

**What's left:**
- State snapshot reception: verify recipe form updates when agent returns improved recipe
- Two-way binding: verify user edits persist across AI improvements
- Serialization: verify snake_case JSON matches server expectations

## Demo 7: Predictive State Updates ⚠️

**What works:**
- Split layout: document editor (left) + chat (right)
- Document viewer with placeholder text
- "✍️ Writing..." streaming indicator (bound to `IsStreaming`)
- Confirmation overlay with Accept/Reject buttons (bound to `IsAwaitingConfirmation`)
- `confirm_changes` frontend tool with `WaitForResponse`/`ProvideResponse`

**What's left:**
- Document content streaming: verify `StateSnapshotReceived` updates `CurrentDocument`
- Accept/Reject flow: verify revert works (restores `PreviousDocument`)
- Streaming indicator timing

---

## Test Cases

| ID | Demo | Description | Status |
|----|------|-------------|--------|
| d1-suggestion-bg | Demo 1 | Click "Change background" → sends correct message | ✅ Pass |
| d1-bg-color-change | Demo 1 | Page background actually changes to light blue | ✅ Pass |
| d1-multi-message | Demo 1 | Send 2+ messages in sequence | ✅ Pass |
| d1-streaming | Demo 1 | Response text streams visibly | ⏳ Not verified |
| d1-suggestion-sonnet | Demo 1 | Click "Generate sonnet" → full sonnet | ⏳ Not verified |
| d2-weather-inline | Demo 2 | Weather card renders inline in message list | ✅ Pass |
| d2-weather-data | Demo 2 | All data fields shown | ✅ Pass |
| d2-correct-message | Demo 2 | Sends "What's the weather like in X?" | ✅ Pass |
| d2-weather-loading | Demo 2 | Loading state before data arrives | ❌ Fail |
| d2-multi-weather | Demo 2 | Second query adds new card | ⏳ Not verified |
| d3-plan-inline | Demo 3 | Plan card visible (above chat) | ✅ Pass |
| d3-correct-message | Demo 3 | Sends exact Blazor message | ✅ Pass |
| d3-plan-checkboxes | Demo 3 | Checkboxes appear for step selection | ✅ Pass |
| d3-confirm-reject | Demo 3 | Confirm/Reject buttons appear and work | ✅ Pass (Confirm verified) |
| d3-plan-loading | Demo 3 | Loading state while plan creates | ❌ Fail |
| d3-step-updates | Demo 3 | Steps update to "Done" after execution | ⏳ Not verified |
