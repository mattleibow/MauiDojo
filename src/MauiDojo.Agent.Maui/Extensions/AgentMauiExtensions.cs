// Copyright (c) Microsoft. All rights reserved.

using MauiDojo.Agent.Maui.Services;

namespace MauiDojo.Agent.Maui.Extensions;

/// <summary>
/// Extension methods for registering AG-UI agent session services in MAUI DI.
/// </summary>
public static class AgentMauiExtensions
{
    /// <summary>
    /// Registers the core AG-UI agent session infrastructure.
    /// </summary>
    public static IServiceCollection AddAgentSession(this IServiceCollection services)
    {
        services.AddTransient<IAgentSessionFactory, AgentSessionFactory>();
        return services;
    }
}
