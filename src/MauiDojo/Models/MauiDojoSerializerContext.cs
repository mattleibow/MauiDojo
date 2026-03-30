// Copyright (c) Microsoft. All rights reserved.

using System.Text.Json;
using System.Text.Json.Serialization;
using MauiDojo.Models;

namespace MauiDojo;

/// <summary>
/// Source-generated JSON serializer context for AOT/trimming support.
/// </summary>
[JsonSerializable(typeof(WeatherInfo))]
[JsonSerializable(typeof(Recipe))]
[JsonSerializable(typeof(Ingredient))]
[JsonSerializable(typeof(RecipeResponse))]
[JsonSerializable(typeof(Plan))]
[JsonSerializable(typeof(Step))]
[JsonSerializable(typeof(StepStatus))]
[JsonSerializable(typeof(DocumentState))]
[JsonSerializable(typeof(JsonPatchOperation))]
[JsonSerializable(typeof(List<JsonPatchOperation>))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(PlanConfirmationResult))]
[JsonSerializable(typeof(ConfirmChangesResult))]
[JsonSerializable(typeof(Haiku))]
public partial class MauiDojoSerializerContext : JsonSerializerContext;
