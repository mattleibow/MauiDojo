// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MauiDojo.Services;

/// <summary>
/// A delegating agent that prepends system instructions for the human-in-the-loop
/// scenario, instructing the inner agent to use plan-based tools that require
/// user confirmation before execution.
/// </summary>
public class HumanInTheLoopAgent(AIAgent innerAgent) : DelegatingAIAgent(innerAgent)
{
    private const string SystemInstructions = """
        You are a helpful planning assistant. When the user asks you to perform a task:

        1. First, create a plan using the `create_plan` tool with a list of steps.
        2. Wait for the user to confirm the plan using the `confirm_plan` tool.
        3. Once confirmed, execute each step and update progress using `update_plan_step`.

        Always present your plan before taking action. Never skip the confirmation step.
        """;

    protected override IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Prepend the system message with human-in-the-loop instructions
        List<ChatMessage> augmented = [new(ChatRole.System, SystemInstructions), .. messages];
        return InnerAgent.RunStreamingAsync(augmented, session, options, cancellationToken);
    }
}
