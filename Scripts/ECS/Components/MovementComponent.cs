using Godot;

namespace GameRpg2D.Scripts.ECS.Components
{
    public struct MovementComponent(float moveSpeed = 4.0f, float inputStopDelay = 0.1f)
    {
        public Vector2I Direction = Vector2I.Zero;
        public bool IsMoving = false;
        public readonly float MoveSpeed = moveSpeed;
        public Vector2 TargetWorldPosition = Vector2.Zero;
        public Vector2 StartWorldPosition = Vector2.Zero;
        public float MoveProgress = 0.0f;
        
        // Novo sistema de movimento contínuo
        public readonly float InputStopDelay = inputStopDelay;
        public float TimeSinceLastInput = 0.0f;
        public bool HasContinuousInput = false;

        // Configurações para movimento contínuo
    }
}
