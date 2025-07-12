using GameRpg2D.Scripts.Core.Constants;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components.Animation;
using GameRpg2D.Scripts.ECS.Components.Combat;
using GameRpg2D.Scripts.ECS.Components.Movement;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;

namespace GameRpg2D.Scripts.ECS.Entities;

/// <summary>
/// Entidade NPC
/// </summary>
public partial class Npc : BaseBody
{
    [Export] private int _npcId = 0;
    [Export] private string _npcName = "";
    [Export] private NpcBehaviourType _behaviourType = NpcBehaviourType.Idle;

    protected override void RegisterComponents()
    {
        // Tag de NPC
        AddComponent(new NpcTag(_npcId, _behaviourType, _npcName));

        // Componente de movimento
        var initialGridPosition = Vector2I.Zero;
        var initialWorldPosition = PositionHelper.GridToWorld(initialGridPosition);

        AddComponent(new MovementComponent
        {
            Speed = MoveSpeed,
            CurrentDirection = Direction.South,
            GridPosition = initialGridPosition,
            TargetGridPosition = initialGridPosition,
            WorldPosition = initialWorldPosition,
            TargetWorldPosition = initialWorldPosition,
            IsMoving = false,
            MoveProgress = 0.0f
        });

        // Componente de ataque (NPCs também podem atacar)
        AddComponent(new AttackComponent
        {
            AttackSpeed = AttackSpeed,
            AttackCooldown = AttackCooldown,
            LastAttackTime = 0.0,
            IsAttacking = false,
            AttackDirection = Direction.South,
            AttackProgress = 0.0f,
            BaseDamage = 8.0f, // NPCs têm dano menor por padrão
            AttackRange = 1
        });

        // Componente de animação (precisa ser adicionado após InitializeSprite)
        CallDeferred(nameof(AddAnimationComponent));
    }

    private void AddAnimationComponent()
    {
        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        if (sprite != null)
        {
            AddComponent(new AnimationComponent
            {
                State = AnimationState.Idle,
                Direction = Direction.South,
                Sprite = sprite,
                CurrentAnimation = DefaultAnimation,
                IsPlaying = true
            });
        }
    }
}
