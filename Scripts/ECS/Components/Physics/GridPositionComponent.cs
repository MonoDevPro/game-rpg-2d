using GameRpg2D.Scripts.Core.Utils;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components.Physics;

public struct GridPositionComponent(Vector2I gridPosition) : IComponent
{
    /// <summary>
    /// Posição da entidade no grid
    /// </summary>
    public Vector2I GridPosition = gridPosition;
}