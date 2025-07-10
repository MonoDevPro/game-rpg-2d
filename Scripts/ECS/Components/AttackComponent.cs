using Godot;

namespace GameRpg2D.Scripts.ECS.Components
{
    public struct AttackComponent(float attackDuration = 0.5f, float attackRange = 1.0f, float attackCooldown = 0.3f)
    {
        public bool IsAttacking = false;
        public readonly float AttackDuration = attackDuration;
        public float AttackProgress = 0.0f;
        public Vector2I AttackDirection = Vector2I.Zero;
        public Vector2I LockedAttackDirection = Vector2I.Zero; // Direção travada durante combo
        public float AttackRange = attackRange;
        public readonly float AttackCooldown = attackCooldown;
        public float TimeSinceLastAttack = attackCooldown; // Iniciar com cooldown completo
        public bool CanCombo = false; // Iniciar sem combo disponível
        public int ComboCount = 0;
        public readonly float ComboWindow = 0.8f; // Janela para combo
        public float ComboTimer = 0.0f;
        public bool AllowMovementDuringAttack = true; // Permite movimento durante ataque
    }
}
