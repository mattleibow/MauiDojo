// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Agents.AI;

namespace MauiDojo.Agent.Maui.Services;

/// <summary>
/// Default implementation of <see cref="IAgentSessionFactory"/>.
/// </summary>
public class AgentSessionFactory : IAgentSessionFactory
{
    public IAgentSession Create(AIAgent agent) => new AgentSession(agent);
}
