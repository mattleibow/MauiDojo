// Copyright (c) Microsoft. All rights reserved.

using CommunityToolkit.Maui;
using MauiDojo.Services;
using MauiDojo.ViewModels;
using MauiDojo.Views;
using MauiDojo.Views.Demos;
using MauiDojo.Agent.Maui.Extensions;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.AGUI;
using Microsoft.Extensions.AI;
using System.ComponentModel;
#if DEBUG
using MauiDevFlow.Agent;
#endif

namespace MauiDojo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.AddMauiDevFlowAgent();
#endif

        // AG-UI server URL (matches the Blazor sample's default)
        string serverUrl = "http://localhost:5018";

        // Register HttpClient for the AG-UI server
        builder.Services.AddHttpClient("aguiserver", httpClient =>
            httpClient.BaseAddress = new Uri(serverUrl));

        // Register the AG-UI agent session infrastructure from the library
        builder.Services.AddAgentSession();

        // Register app-specific services
        builder.Services.AddSingleton<DemoService>();
        builder.Services.AddSingleton<IBackgroundColorService, BackgroundColorService>();

        // Register keyed AIAgent instances for each demo scenario
        RegisterAgents(builder.Services, serverUrl);

        // Register ViewModels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<AgenticChatViewModel>();
        builder.Services.AddTransient<BackendToolRenderingViewModel>();
        builder.Services.AddTransient<HumanInTheLoopViewModel>();
        builder.Services.AddTransient<AgenticGenerativeUIViewModel>();
        builder.Services.AddTransient<ToolBasedGenerativeUIViewModel>();
        builder.Services.AddTransient<SharedStateViewModel>();
        builder.Services.AddTransient<PredictiveStateUpdatesViewModel>();

        // Register Pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AgenticChatPage>();
        builder.Services.AddTransient<BackendToolRenderingPage>();
        builder.Services.AddTransient<HumanInTheLoopPage>();
        builder.Services.AddTransient<AgenticGenerativeUIPage>();
        builder.Services.AddTransient<ToolBasedGenerativeUIPage>();
        builder.Services.AddTransient<SharedStatePage>();
        builder.Services.AddTransient<PredictiveStateUpdatesPage>();

        return builder.Build();
    }

    private static void RegisterAgents(IServiceCollection services, string serverUrl)
    {
        // 1. Agentic Chat — with frontend tool for background color
        services.AddKeyedSingleton<AIAgent>("agentic-chat", (sp, _) =>
        {
            HttpClient httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("aguiserver");
            AGUIChatClient aguiChatClient = new(httpClient, "agentic_chat");

            IBackgroundColorService backgroundService = sp.GetRequiredService<IBackgroundColorService>();

            [Description("Change the background color of the chat interface.")]
            string ChangeBackground(
                [Description("The color to change the background to. Can be a color name (e.g., 'blue'), or hex value (e.g., '#FF5733').")] string color)
            {
                backgroundService.SetColor(color);
                return $"Background color changed to {color}";
            }

            AITool[] frontendTools = [AIFunctionFactory.Create(ChangeBackground)];

            return new ChatClientAgent(aguiChatClient, name: "AgenticChatAssistant", description: "A helpful assistant for the agentic chat demo", tools: frontendTools);
        });

        // 2. Backend Tool Rendering — weather demo (server-side tool)
        services.AddKeyedSingleton<AIAgent>("backend-tool-rendering", (sp, _) =>
        {
            HttpClient httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("aguiserver");
            AGUIChatClient aguiChatClient = new(httpClient, "backend_tool_rendering");

            return new ChatClientAgent(aguiChatClient, name: "BackendToolRenderingAssistant", description: "A helpful assistant that can look up weather information");
        });

        // 3. Human in the Loop — plan confirmation
        // Instructions are set via Session.SystemInstructions in the ViewModel
        services.AddKeyedSingleton<AIAgent>("human-in-the-loop", (sp, _) =>
        {
            HttpClient httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("aguiserver");
            AGUIChatClient aguiChatClient = new(httpClient, "human_in_the_loop");

            return (AIAgent)new ChatClientAgent(aguiChatClient,
                name: "HumanInTheLoopAssistant",
                description: "A helpful assistant that creates plans and asks for user confirmation");
        });

        // 4. Tool-Based Generative UI — haiku generator
        services.AddKeyedSingleton<AIAgent>("tool-based-generative-ui", (sp, _) =>
        {
            HttpClient httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("aguiserver");
            AGUIChatClient aguiChatClient = new(httpClient, "tool_based_generative_ui");

            return new ChatClientAgent(aguiChatClient, name: "ToolBasedGenerativeUIAssistant", description: "A helpful assistant that generates haikus with Japanese text and images");
        });

        // 5. Agentic Generative UI — plan progress
        services.AddKeyedSingleton<AIAgent>("agentic-generative-ui", (sp, _) =>
        {
            HttpClient httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("aguiserver");
            AGUIChatClient aguiChatClient = new(httpClient, "agentic_generative_ui");

            return new ChatClientAgent(aguiChatClient, name: "AgenticGenerativeUIAssistant", description: "A helpful assistant that executes long-running tasks and shows progress");
        });

        // 6. Shared State — recipe copilot
        services.AddKeyedSingleton<AIAgent>("shared-state", (sp, _) =>
        {
            HttpClient httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("aguiserver");
            AGUIChatClient aguiChatClient = new(httpClient, "shared_state");

            return new ChatClientAgent(aguiChatClient, name: "SharedStateAssistant", description: "A recipe copilot that reads and updates collaboratively");
        });

        // 7. Predictive State Updates — document editor
        services.AddKeyedSingleton<AIAgent>("predictive-state-updates", (sp, _) =>
        {
            HttpClient httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("aguiserver");
            AGUIChatClient aguiChatClient = new(httpClient, "predictive_state_updates");

            return new ChatClientAgent(aguiChatClient, name: "PredictiveStateUpdatesAssistant", description: "An AI document editor that streams content updates");
        });
    }
}
