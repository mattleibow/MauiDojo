// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Agents.AI;

namespace MauiDojo.Agent.Maui.Services;

/// <summary>
/// Factory for creating <see cref="IAgentSession"/> instances bound to an <see cref="AIAgent"/>.
/// </summary>
public interface IAgentSessionFactory
{
    IAgentSession Create(AIAgent agent);
}
