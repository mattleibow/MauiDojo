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

public partial class SharedStateViewModel : ObservableObject, IDisposable
{
    [ObservableProperty]
    private string title = "Shared State";

    [ObservableProperty]
    private string userMessage = string.Empty;

    [ObservableProperty]
    private string recipeTitle = "Pasta Primavera";

    [ObservableProperty]
    private string skillLevel = "Beginner";

    [ObservableProperty]
    private string cookingTime = "30 min";

    public IAgentSession Session { get; }

    public ObservableCollection<string> SpecialPreferences { get; } = [];

    public ObservableCollection<Ingredient> Ingredients { get; } = [];

    public ObservableCollection<string> Instructions { get; } = [];

    public IReadOnlyList<string> AvailablePreferences { get; } =
    [
        "High Protein", "Low Carb", "Spicy", "Budget-Friendly",
        "One-Pot", "Vegetarian", "Vegan",
    ];

    public IReadOnlyList<string> AvailableCookingTimes { get; } =
    [
        "5 min", "10 min", "15 min", "30 min", "45 min", "60+ min",
    ];

    public IReadOnlyList<string> AvailableSkillLevels { get; } =
    [
        "Beginner", "Intermediate", "Advanced",
    ];

    public ObservableCollection<Suggestion> Suggestions { get; } =
    [
        new("Make it healthier"),
        new("Add more protein"),
        new("Make it spicier"),
    ];

    public SharedStateViewModel(
        [FromKeyedServices("shared-state")] AIAgent agent,
        IAgentSessionFactory factory)
    {
        Session = factory.Create(agent);
        Session.StateSnapshotReceived += OnStateSnapshotReceived;

        // Default recipe
        Ingredients.Add(new Ingredient { Icon = "🍝", Name = "Pasta", Amount = "200g" });
        Ingredients.Add(new Ingredient { Icon = "🫑", Name = "Bell Pepper", Amount = "1" });
        Ingredients.Add(new Ingredient { Icon = "🧅", Name = "Onion", Amount = "1" });
        Ingredients.Add(new Ingredient { Icon = "🧄", Name = "Garlic", Amount = "3 cloves" });
        Ingredients.Add(new Ingredient { Icon = "🫒", Name = "Olive Oil", Amount = "2 tbsp" });

        Instructions.Add("Boil pasta according to package directions");
        Instructions.Add("Sauté vegetables in olive oil");
        Instructions.Add("Combine pasta with vegetables and serve");
    }

    private void OnStateSnapshotReceived(ReadOnlyMemory<byte> data)
    {
        Recipe? recipe = null;

        try
        {
            var response = JsonSerializer.Deserialize(data.Span, MauiDojoSerializerContext.Default.RecipeResponse);
            recipe = response?.Recipe;
        }
        catch
        {
            // Not a RecipeResponse wrapper
        }

        if (recipe is null)
        {
            try
            {
                recipe = JsonSerializer.Deserialize(data.Span, MauiDojoSerializerContext.Default.Recipe);
            }
            catch
            {
                return;
            }
        }

        if (recipe is null)
            return;

        MainThread.BeginInvokeOnMainThread(() => ApplyRecipe(recipe));
    }

    private void ApplyRecipe(Recipe recipe)
    {
        RecipeTitle = recipe.Title;
        SkillLevel = recipe.SkillLevel;
        CookingTime = recipe.CookingTime;

        SpecialPreferences.Clear();
        foreach (var pref in recipe.SpecialPreferences)
            SpecialPreferences.Add(pref);

        Ingredients.Clear();
        foreach (var ing in recipe.Ingredients)
            Ingredients.Add(ing);

        Instructions.Clear();
        foreach (var step in recipe.Instructions)
            Instructions.Add(step);
    }

    private Recipe BuildCurrentRecipe() => new()
    {
        Title = RecipeTitle,
        SkillLevel = SkillLevel,
        CookingTime = CookingTime,
        SpecialPreferences = [.. SpecialPreferences],
        Ingredients = [.. Ingredients],
        Instructions = [.. Instructions],
    };

    /// <summary>
    /// Serializes the current recipe wrapped as <c>{ recipe: ... }</c> with snake_case
    /// property names, matching the Blazor SharedStateDemo pattern.
    /// </summary>
    private static readonly JsonSerializerOptions s_snakeCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    private byte[] SerializeWrappedRecipe()
    {
        var recipe = BuildCurrentRecipe();
        var stateWrapper = new RecipeResponse { Recipe = recipe };
        return JsonSerializer.SerializeToUtf8Bytes(stateWrapper, s_snakeCaseOptions);
    }

    [RelayCommand]
    private async Task ImproveWithAIAsync()
    {
        var stateBytes = SerializeWrappedRecipe();
        var message = new ChatMessage(ChatRole.User,
        [
            new TextContent("Improve this recipe"),
            new DataContent(stateBytes, "application/json"),
        ]);
        await Session.SendAsync(message);
    }

    [RelayCommand]
    private async Task SendAsync()
    {
        var text = UserMessage?.Trim();
        if (string.IsNullOrEmpty(text))
            return;

        UserMessage = string.Empty;
        var stateBytes = SerializeWrappedRecipe();
        var message = new ChatMessage(ChatRole.User,
        [
            new TextContent(text),
            new DataContent(stateBytes, "application/json"),
        ]);
        await Session.SendAsync(message);
    }

    [RelayCommand]
    private async Task SendSuggestionAsync(Suggestion suggestion)
    {
        var text = suggestion.Message ?? suggestion.Text;
        var stateBytes = SerializeWrappedRecipe();
        var message = new ChatMessage(ChatRole.User,
        [
            new TextContent(text),
            new DataContent(stateBytes, "application/json"),
        ]);
        await Session.SendAsync(message);
    }

    [RelayCommand]
    private void TogglePreference(string preference)
    {
        if (SpecialPreferences.Contains(preference))
            SpecialPreferences.Remove(preference);
        else
            SpecialPreferences.Add(preference);
    }

    [RelayCommand]
    private void AddIngredient()
    {
        Ingredients.Add(new Ingredient { Icon = "🍽️", Name = "", Amount = "" });
    }

    [RelayCommand]
    private void RemoveIngredient(Ingredient ingredient)
    {
        Ingredients.Remove(ingredient);
    }

    [RelayCommand]
    private void AddInstruction()
    {
        Instructions.Add("New step");
    }

    [RelayCommand]
    private void RemoveInstruction(string instruction)
    {
        var index = Instructions.IndexOf(instruction);
        if (index >= 0)
            Instructions.RemoveAt(index);
    }

    public void Dispose()
    {
        Session.StateSnapshotReceived -= OnStateSnapshotReceived;
    }
}
