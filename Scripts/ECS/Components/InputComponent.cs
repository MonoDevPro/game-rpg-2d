using Godot;

namespace GameRpg2D.Scripts.ECS.Components
{
    public struct InputComponent()
    {
        public Vector2I InputDirection = Vector2I.Zero;
        public bool HasInput = false;
        public bool AttackPressed = false;
        public bool AttackJustPressed = false;
    }
}
