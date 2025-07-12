using System;
using System.Runtime.CompilerServices;
using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.Utilities;
using Godot;

namespace GameRpg2D.Scripts.ECS;

/// <summary>
/// Sistema responsável pela inteligência artificial dos NPCs
/// </summary>
public partial class AISystem : BaseSystem<World, float>
{
    private readonly Random _random = new();
    
    public AISystem(World world) : base(world) { }

    /// <summary>
    /// Processa comportamento de patrulha para NPCs
    /// </summary>
    [Query]
    [All<NpcTag>, None<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessPatrolBehavior(
        [Data] in float deltaTime,
        ref BehaviourComponent behavior,
        ref MovementComponent movement,
        in PositionComponent position)
    {
        if (behavior.BehaviourType != NpcBehaviourType.Patrol)
            return;

        behavior.ActionTimer += deltaTime;

        // Se não está se movendo e passou o tempo de ação
        if (!movement.IsMoving && behavior.ActionTimer >= behavior.ActionInterval)
        {
            // Escolhe uma direção aleatória para patrulhar
            var directions = new[] { Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };
            var randomDirection = directions[_random.Next(directions.Length)];
            
            var positionVector = position.ToVector2();
            var currentGridPos = GridUtils.WorldToGrid(positionVector);
            var targetGridPos = currentGridPos + randomDirection;
            
            // Inicia movimento
            movement.Direction = randomDirection;
            movement.StartGridPosition = currentGridPos;
            movement.TargetGridPosition = targetGridPos;
            movement.Progress = 0.0f;
            
            behavior.ActionTimer = 0.0f;
        }
    }

    /// <summary>
    /// Processa comportamento estático (NPCs que ficam parados)
    /// </summary>
    [Query]
    [All<NpcTag>, None<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessIdleBehavior(
        [Data] in float deltaTime,
        ref BehaviourComponent behavior,
        ref MovementComponent movement)
    {
        if (behavior.BehaviourType != NpcBehaviourType.Idle)
            return;

        // NPCs idle não se movem, apenas resetam movimento se estiverem se movendo
        if (movement.IsMoving)
        {
            movement.Direction = Vector2I.Zero;
            movement.Progress = 0.0f;
        }
    }

    /// <summary>
    /// Processa comportamento agressivo (perseguir player)
    /// </summary>
    [Query]
    [All<NpcTag>, None<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessAggressiveBehavior(
        [Data] in float deltaTime,
        ref BehaviourComponent behavior,
        ref MovementComponent movement,
        in PositionComponent position,
        ref AIStateComponent aiState)
    {
        if (behavior.BehaviourType != NpcBehaviourType.Aggressive)
            return;

        aiState.LastDecisionTime += deltaTime;
        
        // Só toma decisões a cada intervalo
        if (aiState.LastDecisionTime < aiState.DecisionCooldown)
            return;

        // Procura pelo player próximo
        var positionVector = position.ToVector2();
        var playerPosition = FindNearestPlayer(positionVector);
        
        if (playerPosition.HasValue)
        {
            var currentGridPos = GridUtils.WorldToGrid(positionVector);
            var playerGridPos = GridUtils.WorldToGrid(playerPosition.Value);
            
            // Calcula direção para o player
            var direction = GetDirectionToTarget(currentGridPos, playerGridPos);
            
            if (direction != Vector2I.Zero && !movement.IsMoving)
            {
                movement.Direction = direction;
                movement.StartGridPosition = currentGridPos;
                movement.TargetGridPosition = currentGridPos + direction;
                movement.Progress = 0.0f;
                
                aiState.LastKnownPlayerPosition = playerGridPos;
                aiState.HasSeenPlayer = true;
            }
        }
        
        aiState.LastDecisionTime = 0.0f;
    }

    /// <summary>
    /// Processa comportamento de fuga (fugir do player)
    /// </summary>
    [Query]
    [All<NpcTag>, None<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessFleeBehavior(
        [Data] in float deltaTime,
        ref BehaviourComponent behavior,
        ref MovementComponent movement,
        in PositionComponent position)
    {
        if (behavior.BehaviourType != NpcBehaviourType.Flee)
            return;

        var positionVector = position.ToVector2();
        var playerPosition = FindNearestPlayer(positionVector);
        
        if (playerPosition.HasValue)
        {
            var currentGridPos = GridUtils.WorldToGrid(positionVector);
            var playerGridPos = GridUtils.WorldToGrid(playerPosition.Value);
            
            // Calcula direção oposta ao player
            var direction = GetDirectionAwayFromTarget(currentGridPos, playerGridPos);
            
            if (direction != Vector2I.Zero && !movement.IsMoving)
            {
                movement.Direction = direction;
                movement.StartGridPosition = currentGridPos;
                movement.TargetGridPosition = currentGridPos + direction;
                movement.Progress = 0.0f;
            }
        }
    }

    /// <summary>
    /// Encontra o player mais próximo
    /// </summary>
    private static QueryDescription FindNearestPlayerQuery()
    {
        return new QueryDescription()
            .WithAll<LocalPlayerTag, PositionComponent>()
            .WithNone<NpcTag>();
    }
    private Vector2? FindNearestPlayer(Vector2 npcPosition)
    {
        Vector2? nearestPlayer = null;
        float nearestDistance = float.MaxValue;
        
        // Busca manual por entidades com LocalPlayerTag e PositionComponent
        World.Query<LocalPlayerTag, PositionComponent>(FindNearestPlayerQuery(),
            (Entity entity, ref LocalPlayerTag component, ref PositionComponent t1Component) =>
                
        {
            var distance = npcPosition.DistanceTo(t1Component.ToVector2());
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPlayer = t1Component.ToVector2();
            }
        });
        
        // Só retorna se o player estiver próximo (raio de detecção)
        return nearestDistance <= 5.0f ? nearestPlayer : null;
    }

    /// <summary>
    /// Calcula direção para um alvo
    /// </summary>
    private Vector2I GetDirectionToTarget(Vector2I from, Vector2I to)
    {
        var diff = to - from;
        
        // Prioriza movimento horizontal/vertical
        if (Mathf.Abs(diff.X) > Mathf.Abs(diff.Y))
        {
            return new Vector2I(Mathf.Sign(diff.X), 0);
        }
        else if (diff.Y != 0)
        {
            return new Vector2I(0, Mathf.Sign(diff.Y));
        }
        
        return Vector2I.Zero;
    }

    /// <summary>
    /// Calcula direção para fugir de um alvo
    /// </summary>
    private Vector2I GetDirectionAwayFromTarget(Vector2I from, Vector2I target)
    {
        var diff = from - target; // Inverte para fugir
        
        if (Mathf.Abs(diff.X) > Mathf.Abs(diff.Y))
        {
            return new Vector2I(Mathf.Sign(diff.X), 0);
        }
        else if (diff.Y != 0)
        {
            return new Vector2I(0, Mathf.Sign(diff.Y));
        }
        
        return Vector2I.Zero;
    }
}