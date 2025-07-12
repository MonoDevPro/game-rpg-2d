using GameRpg2D.Scripts.ECS.Components;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components.Physics;

/// <summary>
/// Componente responsável por dados de colisão das entidades
/// </summary>
public struct CollisionComponent : IComponent
{
    /// <summary>
    /// Referência ao nó CharacterBody2D da entidade
    /// </summary>
    public CharacterBody2D Body;

    /// <summary>
    /// Máscara de colisão (layers que pode colidir)
    /// </summary>
    public uint CollisionMask;

    /// <summary>
    /// Layer de colisão (layer onde a entidade está)
    /// </summary>
    public uint CollisionLayer;

    /// <summary>
    /// Indica se está atualmente colidindo
    /// </summary>
    public bool IsColliding;

    /// <summary>
    /// Última posição válida conhecida
    /// </summary>
    public Vector2 LastValidPosition;

    /// <summary>
    /// Direções bloqueadas por colisão
    /// </summary>
    public CollisionDirections BlockedDirections;

    /// <summary>
    /// Raio de detecção para verificação antecipada
    /// </summary>
    public float DetectionRadius;

    /// <summary>
    /// Habilita debug visual de colisões
    /// </summary>
    public bool EnableDebugVisualization;
}

/// <summary>
/// Flags para indicar quais direções estão bloqueadas
/// </summary>
[System.Flags]
public enum CollisionDirections : byte
{
    None = 0,
    North = 1 << 0,
    South = 1 << 1,
    East = 1 << 2,
    West = 1 << 3,
    NorthEast = 1 << 4,
    NorthWest = 1 << 5,
    SouthEast = 1 << 6,
    SouthWest = 1 << 7
}
