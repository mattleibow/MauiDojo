// Copyright (c) Microsoft. All rights reserved.

using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDojo.Agent.Maui;
using MauiDojo.Agent.Maui.Services;
using MauiDojo.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MauiDojo.ViewModels;

public partial class AgenticGenerativeUIViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Agentic Generative UI";

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private Plan? _currentPlan;

    public IAgentSession Session { get; }

    public ObservableCollection<Suggestion> Suggestions { get; } =
    [
        new("Plan a trip to Mars", "Plan a trip to Mars with detailed steps"),
        new("Make pizza from scratch", "Walk me through making pizza from scratch step by step"),
    ];

    public AgenticGenerativeUIViewModel(
        [FromKeyedServices("agentic-generative-ui")] AIAgent agent,
        IAgentSessionFactory factory)
    {
        Session = factory.Create(agent);
        Session.StateSnapshotReceived += OnStateSnapshotReceived;
        Session.StateDeltaReceived += OnStateDeltaReceived;
    }

    private void OnStateSnapshotReceived(ReadOnlyMemory<byte> data)
    {
        try
        {
            var plan = JsonSerializer.Deserialize(data.Span, MauiDojoSerializerContext.Default.Plan);
            if (plan is not null)
            {
                MainThread.BeginInvokeOnMainThread(() => CurrentPlan = plan);
            }
        }
        catch
        {
            // Ignore deserialization errors
        }
    }

    private void OnStateDeltaReceived(ReadOnlyMemory<byte> data)
    {
        try
        {
            var operations = JsonSerializer.Deserialize(data.Span, MauiDojoSerializerContext.Default.ListJsonPatchOperation);
            if (operations is null || CurrentPlan is null)
                return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ApplyPatchOperations(operations);
                OnPropertyChanged(nameof(CurrentPlan));
            });
        }
        catch
        {
            // Ignore deserialization errors
        }
    }

    private void ApplyPatchOperations(List<JsonPatchOperation> operations)
    {
        if (CurrentPlan is null)
            return;

        foreach (var op in operations)
        {
            // Parse paths like "/steps/0/status" or "/steps/0/description"
            var segments = op.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length >= 3 && segments[0] == "steps" && int.TryParse(segments[1], out var index))
            {
                if (index < 0 || index >= CurrentPlan.Steps.Count)
                    continue;

                var step = CurrentPlan.Steps[index];
                var field = segments[2];

                switch (op.Op)
                {
                    case "replace" or "add":
                        if (field == "status" && op.Value is JsonElement statusEl)
                        {
                            var statusStr = statusEl.ValueKind == JsonValueKind.String
                                ? statusEl.GetString()
                                : statusEl.GetRawText().Trim('"');
                            if (Enum.TryParse<StepStatus>(statusStr, ignoreCase: true, out var status))
                                step.Status = status;
                        }
                        else if (field == "description" && op.Value is JsonElement descEl)
                        {
                            step.Description = descEl.GetString() ?? step.Description;
                        }
                        break;
                }
            }
            else if (segments.Length == 2 && segments[0] == "steps" && op.Op == "add")
            {
                // Adding a new step: /steps/-  or /steps/N
                if (op.Value is JsonElement stepEl)
                {
                    var newStep = JsonSerializer.Deserialize(stepEl.GetRawText(), MauiDojoSerializerContext.Default.Step);
                    if (newStep is not null)
                    {
                        if (segments[1] == "-")
                            CurrentPlan.Steps.Add(newStep);
                        else if (int.TryParse(segments[1], out var idx))
                            CurrentPlan.Steps.Insert(Math.Min(idx, CurrentPlan.Steps.Count), newStep);
                    }
                }
            }
        }
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        var text = InputText?.Trim();
        if (string.IsNullOrEmpty(text) || Session.IsProcessing)
            return;

        InputText = string.Empty;
        await Session.SendAsync(new ChatMessage(ChatRole.User, text));
    }

    [RelayCommand]
    private async Task SendSuggestionAsync(Suggestion suggestion)
    {
        if (Session.IsProcessing)
            return;

        var message = suggestion.Message ?? suggestion.Text;
        InputText = string.Empty;
        await Session.SendAsync(new ChatMessage(ChatRole.User, message));
    }
}
