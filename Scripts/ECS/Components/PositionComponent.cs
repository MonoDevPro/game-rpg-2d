using GameRpg2D.Scripts.Constants;
using Godot;

namespace GameRpg2D.Scripts.ECS.Components
{
    public struct PositionComponent(Vector2I gridPosition)
    {
        public Vector2I GridPosition = gridPosition;
        public Vector2 WorldPosition = new(gridPosition.X * GameConstants.GRID_SIZE, gridPosition.Y * GameConstants.GRID_SIZE);
    }
}
