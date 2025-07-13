using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components.AI;
using GameRpg2D.Scripts.ECS.Components.Animation;
using GameRpg2D.Scripts.ECS.Components.Combat;
using GameRpg2D.Scripts.ECS.Components.Movement;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;
using Godot.Collections;

namespace GameRpg2D.Scripts.ECS.Entities;

/// <summary>
/// Entidade NPC
/// </summary>
public partial class Npc : BaseBody
{
    [Export] private int _npcId = 0;
    [Export] private string _npcName = "";
    [Export] private NpcBehaviourType _behaviourType = NpcBehaviourType.Idle;

    // Configurações de patrulha
    [Export] private float _patrolSpeed = 50.0f;
    [Export] private float _waitDuration = 2.0f;
    [Export] private bool _isLooping = true;
    [Export] private bool _reverseOnEnd = false;
    [Export] private float _wayPointTolerance = 0.5f;

    // Waypoints
    [Export] private Array<Vector2I> _patrolWaypoints = [];
    protected override void RegisterComponents()
    {
        // Tag de NPC
        AddComponent(new NpcTag(_npcId, _behaviourType, _npcName));

        // Componente de movimento (inicializa na posição do primeiro waypoint se for patrol)
        var initialGridPosition = (_behaviourType == NpcBehaviourType.Patrol && _patrolWaypoints.Count > 0)
            ? _patrolWaypoints[0]
            : Vector2I.Zero;
        var initialWorldPosition = PositionHelper.GridToWorld(initialGridPosition);

        AddComponent(new MovementComponent
        {
            Speed = MoveSpeed,
            CurrentDirection = Direction.South,
            GridPosition = initialGridPosition,
            TargetGridPosition = initialGridPosition,
            WorldPosition = initialWorldPosition,
            TargetWorldPosition = initialWorldPosition,
            StartWorldPosition = initialWorldPosition,
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

        // Componente de patrulha (se NPC tem comportamento Patrol)
        if (_behaviourType == NpcBehaviourType.Patrol)
        {
            AddPatrolComponent();
        }

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

    /// <summary>
    /// Adiciona componente de patrulha ao NPC
    /// </summary>
    private void AddPatrolComponent()
    {
        if (_patrolWaypoints == null || _patrolWaypoints.Count == 0)
        {
            GD.PrintErr($"[Npc] {_npcName} configurado para patrulha mas sem waypoints!");
            return;
        }

        // Inicializa na posição do primeiro waypoint
        var initialGridPosition = _patrolWaypoints[0];
        var initialWorldPosition = PositionHelper.GridToWorld(initialGridPosition);

        // Atualiza o componente de movimento para a posição inicial correta
        if (World.Has<MovementComponent>(Entity))
        {
            ref var movement = ref World.Get<MovementComponent>(Entity);
            movement.GridPosition = initialGridPosition;
            movement.WorldPosition = initialWorldPosition;
            movement.TargetGridPosition = initialGridPosition;
            movement.TargetWorldPosition = initialWorldPosition;
            movement.StartWorldPosition = initialWorldPosition;
        }

        AddComponent(new PatrolComponent
        {
            WayPoints = [.. _patrolWaypoints], // Copia os waypoints
            CurrentWayPointIndex = 0,
            State = PatrolState.Moving,
            PatrolDirection = Direction.South,
            WaitTimer = 0.0f,
            WaitDuration = _waitDuration,
            PatrolSpeed = _patrolSpeed,
            IsLooping = _isLooping,
            ReverseOnEnd = _reverseOnEnd,
            IsReversing = false,
            InitialWayPoint = initialGridPosition,
            WayPointTolerance = _wayPointTolerance
        });

        GD.Print($"[Npc] {_npcName} configurado para patrulha com {_patrolWaypoints.Count} waypoints");
    }
}
