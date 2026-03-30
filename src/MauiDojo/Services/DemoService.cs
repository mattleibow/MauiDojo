// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.Models;

namespace MauiDojo.Services;

/// <summary>
/// Provides the catalog of demo scenarios available in the MAUI Dojo.
/// </summary>
public class DemoService
{
    public IReadOnlyList<DemoScenario> AllScenarios { get; } =
    [
        new(
            Id: "agentic-chat",
            Title: "Agentic Chat",
            Description: "A chat assistant that can call frontend tools to change the UI, such as updating the background color.",
            Tags: ["Chat", "Frontend Tools", "AG-UI"],
            Endpoint: "agentic_chat",
            Icon: "💬"),

        new(
            Id: "backend-tool-rendering",
            Title: "Backend Tool Rendering",
            Description: "Demonstrates server-side tool calls (e.g., weather lookup) with results rendered on the client.",
            Tags: ["Tools", "Backend", "Weather"],
            Endpoint: "backend_tool_rendering",
            Icon: "🌤️"),

        new(
            Id: "human-in-the-loop",
            Title: "Human in the Loop",
            Description: "The agent creates a plan and waits for user confirmation before executing each step.",
            Tags: ["Planning", "Confirmation", "Human Review"],
            Endpoint: "human_in_the_loop",
            Icon: "🙋"),

        new(
            Id: "agentic-generative-ui",
            Title: "Agentic Generative UI",
            Description: "Long-running agent tasks that stream progress updates and render dynamic UI.",
            Tags: ["Generative UI", "Progress", "Streaming"],
            Endpoint: "agentic_generative_ui",
            Icon: "📊"),

        new(
            Id: "tool-based-generative-ui",
            Title: "Tool-Based Generative UI",
            Description: "Tool calls that return structured data rendered as rich UI components, such as haiku cards.",
            Tags: ["Generative UI", "Tools", "Haiku"],
            Endpoint: "tool_based_generative_ui",
            Icon: "🎋"),

        new(
            Id: "shared-state",
            Title: "Shared State",
            Description: "Agent and user collaborate on shared state — a recipe that both can read and update.",
            Tags: ["Shared State", "Collaboration", "Recipe"],
            Endpoint: "shared_state",
            Icon: "🍳"),

        new(
            Id: "predictive-state-updates",
            Title: "Predictive State Updates",
            Description: "The agent streams document edits in real time, showing predictive state changes as they arrive.",
            Tags: ["Predictive", "Streaming", "Document"],
            Endpoint: "predictive_state_updates",
            Icon: "📝"),
    ];
}
