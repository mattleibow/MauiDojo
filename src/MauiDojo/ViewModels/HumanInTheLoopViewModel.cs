// Copyright (c) Microsoft. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDojo.Agent.Maui;
using MauiDojo.Agent.Maui.Services;
using MauiDojo.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MauiDojo.ViewModels;

public partial class HumanInTheLoopViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    private string _title = "Human in the Loop";

    [ObservableProperty]
    private string _inputText = string.Empty;

    [ObservableProperty]
    private Plan? _currentPlan;

    [ObservableProperty]
    private bool _isAwaitingConfirmation;

    public IAgentSession Session { get; }

    /// <summary>Tracks which step indices the user has checked for confirmation.</summary>
    public ObservableCollection<int> SelectedSteps { get; } = [];

    public ObservableCollection<Suggestion> Suggestions { get; } =
    [
        new("Plan a birthday party", "Help me plan a birthday party for my friend"),
        new("Create a workout plan", "Create a weekly workout plan for a beginner"),
    ];

    public HumanInTheLoopViewModel(
        [FromKeyedServices("human-in-the-loop")] AIAgent agent,
        IAgentSessionFactory factory)
    {
        Session = factory.Create(agent);
        RegisterFrontendTools();
    }

    private void RegisterFrontendTools()
    {
        // 1. create_plan — creates a plan with multiple steps (matches Blazor's create_plan)
        var createPlanTool = AIFunctionFactory.Create(
            (List<string> steps) => CreatePlan(steps),
            "create_plan",
            "Create a plan with multiple steps. Call this first before confirm_plan.");

        // 2. confirm_plan — waits for user confirmation via UI (matches Blazor's confirm_plan)
        var confirmPlanTool = AIFunctionFactory.Create(
            (Plan plan) => ConfirmPlanAsync(plan),
            "confirm_plan",
            "Present the plan to the user for confirmation. The user can select which steps to proceed with. Pass the plan returned from create_plan.");

        // 3. update_plan_step — updates a step's status (matches Blazor's update_plan_step)
        var updatePlanStepTool = AIFunctionFactory.Create(
            (int index, string? description, string? status) => UpdatePlanStep(index, description, status),
            "update_plan_step",
            "Update a step in the plan with new description or status. Use status 'completed' to mark a step as done.");

        Session.RegisterTools(createPlanTool, confirmPlanTool, updatePlanStepTool);
    }

    [Description("Create a plan with multiple steps.")]
    private Plan CreatePlan(
        [Description("List of step descriptions to create the plan.")] List<string> steps)
    {
        var plan = new Plan
        {
            Steps = steps.Select(s => new Step
            {
                Description = s,
                Status = StepStatus.Pending,
                IsSelected = true,
            }).ToList()
        };

        MainThread.BeginInvokeOnMainThread(() =>
        {
            CurrentPlan = plan;
        });

        return plan;
    }

    [Description("Present the plan to the user for confirmation.")]
    private async Task<PlanConfirmationResult> ConfirmPlanAsync(
        [Description("The plan to present to the user for confirmation.")] Plan plan)
    {
        await MainThread.InvokeOnMainThreadAsync(() => IsAwaitingConfirmation = true);
        var response = await Session.WaitForResponse("confirm_plan");

        if (response is PlanConfirmationResult result)
            return result;

        return new PlanConfirmationResult { Confirmed = false };
    }

    [Description("Update a step in the plan with new description or status.")]
    private List<JsonPatchOperation> UpdatePlanStep(
        [Description("The index of the step to update.")] int index,
        [Description("The new description for the step (optional).")] string? description = null,
        [Description("The new status for the step: 'pending' or 'completed'.")] string? status = null)
    {
        var changes = new List<JsonPatchOperation>();

        if (CurrentPlan is null || index < 0 || index >= CurrentPlan.Steps.Count)
            return changes;

        var step = CurrentPlan.Steps[index];

        if (description is not null)
        {
            step.Description = description;
            changes.Add(new JsonPatchOperation
            {
                Op = "replace",
                Path = $"/steps/{index}/description",
                Value = description,
            });
        }

        if (status is not null)
        {
            if (Enum.TryParse<StepStatus>(status, ignoreCase: true, out var s))
                step.Status = s;
            changes.Add(new JsonPatchOperation
            {
                Op = "replace",
                Path = $"/steps/{index}/status",
                Value = status.ToLowerInvariant(),
            });
        }

        MainThread.BeginInvokeOnMainThread(() => OnPropertyChanged(nameof(CurrentPlan)));
        return changes;
    }

    [RelayCommand]
    private void ConfirmPlan()
    {
        IsAwaitingConfirmation = false;

        // Build selected step indices from Step.IsSelected
        var selectedIndices = new List<int>();
        if (CurrentPlan is not null)
        {
            for (int i = 0; i < CurrentPlan.Steps.Count; i++)
            {
                if (CurrentPlan.Steps[i].IsSelected)
                    selectedIndices.Add(i);
            }
        }

        Session.ProvideResponse("confirm_plan", new PlanConfirmationResult
        {
            Confirmed = true,
            SelectedStepIndices = selectedIndices,
        });
    }

    [RelayCommand]
    private void RejectPlan()
    {
        IsAwaitingConfirmation = false;
        Session.ProvideResponse("confirm_plan", new PlanConfirmationResult
        {
            Confirmed = false,
        });
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

    public void Dispose()
    {
        // No event subscriptions to clean up in this ViewModel,
        // but implement the pattern for consistency.
    }
}
