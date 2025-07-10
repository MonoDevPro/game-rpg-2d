using Godot;

namespace GameRpg2D.Scripts.ECS.Components
{
    public struct AnimationComponent(AnimatedSprite2D animatedSprite)
    {
        public readonly AnimatedSprite2D AnimatedSprite = animatedSprite;
        public string CurrentAnimation = "";
        public Vector2I LastDirection = Vector2I.Zero;
        public bool IsMoving = false;
    }
}
