using Godot;

namespace GameRpg2D.Scripts.ECS.Components.Physics;

public struct TransformComponent(Vector2 worldPosition, Vector2 scale, float rotation) : IComponent
{
    /// <summary>
    /// Posição da entidade no mundo
    /// </summary>
    public Vector2 WorldPosition = worldPosition;
    
    /// <summary>
    /// Escala da entidade
    /// </summary>
    public Vector2 Scale = scale;

    /// <summary>
    /// Ângulo de rotação da entidade em radianos
    /// </summary>
    public float Rotation = rotation;
}