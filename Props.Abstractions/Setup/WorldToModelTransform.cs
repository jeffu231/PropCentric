using System.Numerics;

namespace Props.Abstractions.Setup;

/// <summary>
/// Captures the world to model transform
/// </summary>
/// <param name="WorldMin"></param>
/// <param name="WorldMax"></param>
public record WorldToModelTransform(
    Vector2 WorldMin,
    Vector2 WorldMax);