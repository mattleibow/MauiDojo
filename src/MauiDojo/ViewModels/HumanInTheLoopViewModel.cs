// Copyright (c) Microsoft. All rights reserved.

using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiDojo.Agent.Maui;
using MauiDojo.Agent.Maui.Services;
using MauiDojo.Models;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MauiDojo.ViewModels;

public partial class HumanInTheLoopViewModel : ObservableObject
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
        [Description("Create a plan with a list of steps for the user to review.")]
        Plan CreatePlan(
            [Description("The list of step descriptions for the plan.")] List<string> steps)
        {
            var plan = new Plan
            {
                Steps = steps.Select(s => new Step { Description = s, Status = StepStatus.Pending }).ToList()
            };

            MainThread.BeginInvokeOnMainThread(() =>
            {
                CurrentPlan = plan;
                SelectedSteps.Clear();
                // Select all steps by default
                for (int i = 0; i < plan.Steps.Count; i++)
                    SelectedSteps.Add(i);
                IsAwaitingConfirmation = true;
            });

            return plan;
        }

        [Description("Wait for the user to confirm or reject the plan.")]
        async Task<PlanConfirmationResult> ConfirmPlanTool(
            [Description("The plan to confirm.")] Plan plan)
        {
            var response = await Session.WaitForResponse("confirm_plan");

            if (response is PlanConfirmationResult result)
                return result;

            return new PlanConfirmationResult { Confirmed = false };
        }

        [Description("Update a specific step in the plan with a new status or description.")]
        string UpdatePlanStep(
            [Description("Zero-based index of the step.")] int index,
            [Description("New description for the step.")] string? description,
            [Description("New status: 'pending' or 'completed'.")] string? status)
        {
            if (CurrentPlan is null || index < 0 || index >= CurrentPlan.Steps.Count)
                return "Invalid step index";

            var step = CurrentPlan.Steps[index];

            if (description is not null)
                step.Description = description;

            if (status is not null && Enum.TryParse<StepStatus>(status, ignoreCase: true, out var s))
                step.Status = s;

            MainThread.BeginInvokeOnMainThread(() => OnPropertyChanged(nameof(CurrentPlan)));
            return $"Step {index} updated";
        }

        Session.RegisterTools(
            AIFunctionFactory.Create(CreatePlan),
            AIFunctionFactory.Create(ConfirmPlanTool),
            AIFunctionFactory.Create(UpdatePlanStep));
    }

    [RelayCommand]
    private void ConfirmPlan()
    {
        IsAwaitingConfirmation = false;
        Session.ProvideResponse("confirm_plan", new PlanConfirmationResult
        {
            Confirmed = true,
            SelectedStepIndices = [.. SelectedSteps],
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
}
