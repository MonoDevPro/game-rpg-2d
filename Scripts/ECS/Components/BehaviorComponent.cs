using Godot;

namespace GameRpg2D.Scripts.ECS.Components
{
    /// <summary>
    /// Componente para controlar comportamento de NPCs
    /// </summary>
    public struct BehaviorComponent(NPCBehaviorType behaviorType = NPCBehaviorType.Idle)
    {
        public readonly NPCBehaviorType BehaviorType = behaviorType;
        public float StateTimer = 0.0f;
        public Vector2I LastDirection = Vector2I.Zero;
        public Vector2I PatrolTarget = Vector2I.Zero;
        public readonly float ActionCooldown = 2.0f;
        public float TimeSinceLastAction = 0.0f;
        
        // Configurações de comportamento
        public float WanderRadius = 3.0f;
        public float IdleTime = 2.0f;
        public float MoveTime = 1.5f;
    }
    
    public enum NPCBehaviorType
    {
        Idle,           // Parado
        Wander,         // Caminha aleatoriamente
        Patrol,         // Patrulha entre pontos
        Follow,         // Segue o jogador
        Aggressive      // Ataca o jogador
    }
}
