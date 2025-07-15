/*
using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Constants;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components.AI;
using GameRpg2D.Scripts.ECS.Components.Movement;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.ECS.Events;
using GameRpg2D.Scripts.ECS.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.AI;

/// <summary>
/// Sistema responsável por processar a lógica de patrulha dos NPCs
/// </summary>
public partial class PatrolSystem : BaseSystem<World, float>
{
    public PatrolSystem(World world) : base(world) { }

    /// <summary>
    /// Processa patrulha para NPCs com comportamento Patrol
    /// </summary>
    [Query]
    [All<PatrolComponent, MovementComponent, NpcTag>]
    private void ProcessPatrol([Data] in float deltaTime, ref PatrolComponent patrol, ref MovementComponent movement, in NpcTag npcTag)
    {
        // Só processa se NPC tem comportamento de patrulha
        if (npcTag.BehaviourType != NpcBehaviourType.Patrol)
            return;

        // Valida waypoints
        if (patrol.WayPoints == null || patrol.WayPoints.Length == 0)
        {
            GD.PrintErr($"[PatrolSystem] NPC {npcTag.NpcId} sem waypoints configurados");
            return;
        }

        // Processa baseado no estado atual
        switch (patrol.State)
        {
            case PatrolState.Moving:
                ProcessPatrolMovement(deltaTime, ref patrol, ref movement, npcTag);
                break;
            case PatrolState.Waiting:
                ProcessPatrolWaiting(deltaTime, ref patrol, ref movement, npcTag);
                break;
            case PatrolState.Returning:
                ProcessPatrolReturn(deltaTime, ref patrol, ref movement, npcTag);
                break;
            case PatrolState.Completed:
                ProcessPatrolCompleted(deltaTime, ref patrol, ref movement, npcTag);
                break;
            case PatrolState.Paused:
                // Não faz nada quando pausado
                break;
        }
    }

    /// <summary>
    /// Processa movimento para próximo waypoint
    /// </summary>
    private void ProcessPatrolMovement(float deltaTime, ref PatrolComponent patrol, ref MovementComponent movement, in NpcTag npcTag)
    {
        // Verifica se chegou ao waypoint atual
        var currentWayPoint = patrol.WayPoints[patrol.CurrentWayPointIndex];
        var targetWorldPosition = PositionHelper.GridToWorld(currentWayPoint);
        var distanceToWayPoint = movement.WorldPosition.DistanceTo(targetWorldPosition);

        if (distanceToWayPoint <= patrol.WayPointTolerance * GameConstants.GRID_SIZE)
        {
            // Chegou ao waypoint
            ReachWayPoint(ref patrol, ref movement, npcTag);
        }
        else
        {
            // Continua movimento para o waypoint
            MoveToWayPoint(ref patrol, ref movement, currentWayPoint);
        }
    }

    /// <summary>
    /// Processa espera no waypoint
    /// </summary>
    private void ProcessPatrolWaiting(float deltaTime, ref PatrolComponent patrol, ref MovementComponent movement, in NpcTag npcTag)
    {
        // Incrementa timer de espera
        patrol.WaitTimer += deltaTime;

        // Verifica se terminou a espera
        if (patrol.WaitTimer >= patrol.WaitDuration)
        {
            // Terminou espera, move para próximo waypoint
            patrol.WaitTimer = 0.0f;
            var nextWayPointIndex = CalculateNextWayPoint(ref patrol);

            if (nextWayPointIndex != -1)
            {
                patrol.CurrentWayPointIndex = nextWayPointIndex;
                ChangePatrolState(ref patrol, PatrolState.Moving, npcTag);
            }
            else
            {
                // Fim da patrulha
                HandlePatrolEnd(ref patrol, npcTag);
            }
        }
    }

    /// <summary>
    /// Processa retorno ao ponto inicial
    /// </summary>
    private void ProcessPatrolReturn(float deltaTime, ref PatrolComponent patrol, ref MovementComponent movement, in NpcTag npcTag)
    {
        // Verifica se chegou ao ponto inicial
        var distanceToInitial = movement.GridPosition.DistanceTo(patrol.InitialWayPoint);

        if (distanceToInitial <= patrol.WayPointTolerance)
        {
            // Chegou ao ponto inicial
            patrol.CurrentWayPointIndex = 0;
            patrol.IsReversing = false;
            ChangePatrolState(ref patrol, PatrolState.Waiting, npcTag);
        }
        else
        {
            // Continua movimento para o ponto inicial
            MoveToWayPoint(ref patrol, ref movement, patrol.InitialWayPoint);
        }
    }

    /// <summary>
    /// Processa patrulha completada
    /// </summary>
    private void ProcessPatrolCompleted(float deltaTime, ref PatrolComponent patrol, ref MovementComponent movement, in NpcTag npcTag)
    {
        // Se é loop, reinicia a patrulha
        if (patrol.IsLooping)
        {
            patrol.CurrentWayPointIndex = 0;
            patrol.IsReversing = false;
            patrol.WaitTimer = 0.0f;
            ChangePatrolState(ref patrol, PatrolState.Moving, npcTag);
        }
        // Senão, permanece idle
    }

    /// <summary>
    /// Chamado quando NPC alcança um waypoint
    /// </summary>
    private void ReachWayPoint(ref PatrolComponent patrol, ref MovementComponent movement, in NpcTag npcTag)
    {
        var currentWayPoint = patrol.WayPoints[patrol.CurrentWayPointIndex];

        // Para o movimento e atualiza posição
        movement.IsMoving = false;
        movement.GridPosition = currentWayPoint;
        movement.WorldPosition = PositionHelper.GridToWorld(currentWayPoint);
        movement.TargetGridPosition = currentWayPoint;
        movement.TargetWorldPosition = movement.WorldPosition;
        movement.StartWorldPosition = movement.WorldPosition; // CORREÇÃO: Sincroniza posição inicial
        movement.MoveProgress = 0.0f; // CORREÇÃO: Reseta progresso

        // Publica evento
        GameEventBus.PublishPatrolWaypointReached(new PatrolWaypointReachedEvent(
            entityId: (uint)npcTag.NpcId,
            wayPoint: currentWayPoint,
            wayPointIndex: patrol.CurrentWayPointIndex,
            newState: PatrolState.Waiting
        ));

        // Muda para estado de espera
        ChangePatrolState(ref patrol, PatrolState.Waiting, npcTag);
    }

    /// <summary>
    /// Move NPC para um waypoint específico
    /// </summary>
    private void MoveToWayPoint(ref PatrolComponent patrol, ref MovementComponent movement, Vector2I targetWayPoint)
    {
        // Movimento direto para o waypoint (não célula por célula)
        if (!movement.IsMoving)
        {
            // Configura movimento direto para o waypoint
            var targetWorldPosition = PositionHelper.GridToWorld(targetWayPoint);

            // Calcula direção para animação (baseada na direção predominante)
            var direction = CalculateDirectionToWayPoint(movement.GridPosition, targetWayPoint);

            movement.CurrentDirection = direction;
            movement.TargetGridPosition = targetWayPoint;
            movement.TargetWorldPosition = targetWorldPosition;
            movement.StartWorldPosition = movement.WorldPosition;
            movement.Speed = patrol.PatrolSpeed;
            movement.IsMoving = true;
            movement.MoveProgress = 0.0f;

            // Atualiza direção da patrulha
            patrol.PatrolDirection = direction;

            GD.Print($"[PatrolSystem] NPC movendo de {movement.GridPosition} para {targetWayPoint}");
        }
    }

    /// <summary>
    /// Calcula direção para chegar ao waypoint (direção predominante para animação)
    /// </summary>
    private Direction CalculateDirectionToWayPoint(Vector2I currentPos, Vector2I targetPos)
    {
        var delta = targetPos - currentPos;

        // Se já está no destino, não move
        if (delta.X == 0 && delta.Y == 0)
            return Direction.None;

        // Calcula direção predominante para animação
        Direction direction = Direction.None;

        // Determina direção baseada na maior distância
        if (Mathf.Abs(delta.X) > Mathf.Abs(delta.Y))
        {
            // Movimento horizontal predominante
            direction = delta.X > 0 ? Direction.East : Direction.West;
        }
        else if (Mathf.Abs(delta.Y) > Mathf.Abs(delta.X))
        {
            // Movimento vertical predominante  
            direction = delta.Y > 0 ? Direction.South : Direction.North;
        }
        else
        {
            // Mesma distância - prioriza horizontal
            if (delta.X != 0)
                direction = delta.X > 0 ? Direction.East : Direction.West;
            else if (delta.Y != 0)
                direction = delta.Y > 0 ? Direction.South : Direction.North;
        }

        return direction;
    }

    /// <summary>
    /// Calcula próximo waypoint na sequência
    /// </summary>
    private int CalculateNextWayPoint(ref PatrolComponent patrol)
    {
        var totalWayPoints = patrol.WayPoints.Length;

        if (patrol.ReverseOnEnd)
        {
            // Modo ping-pong
            if (!patrol.IsReversing)
            {
                // Indo para frente
                if (patrol.CurrentWayPointIndex < totalWayPoints - 1)
                    return patrol.CurrentWayPointIndex + 1;
                else
                {
                    // Chegou ao final, inverte direção
                    patrol.IsReversing = true;
                    return patrol.CurrentWayPointIndex - 1;
                }
            }
            else
            {
                // Indo para trás
                if (patrol.CurrentWayPointIndex > 0)
                    return patrol.CurrentWayPointIndex - 1;
                else
                {
                    // Chegou ao início, inverte direção
                    patrol.IsReversing = false;
                    return patrol.CurrentWayPointIndex + 1;
                }
            }
        }
        else
        {
            // Modo linear
            if (patrol.CurrentWayPointIndex < totalWayPoints - 1)
                return patrol.CurrentWayPointIndex + 1;
            else
                return -1; // Fim da patrulha
        }
    }

    /// <summary>
    /// Trata fim da patrulha
    /// </summary>
    private void HandlePatrolEnd(ref PatrolComponent patrol, in NpcTag npcTag)
    {
        if (patrol.IsLooping)
        {
            // Reinicia do começo
            patrol.CurrentWayPointIndex = 0;
            patrol.IsReversing = false;
            ChangePatrolState(ref patrol, PatrolState.Moving, npcTag);
        }
        else
        {
            // Completa patrulha
            ChangePatrolState(ref patrol, PatrolState.Completed, npcTag);

            GameEventBus.PublishPatrolCompleted(new PatrolCompletedEvent(
                entityId: (uint)npcTag.NpcId,
                finalWayPoint: patrol.WayPoints[patrol.CurrentWayPointIndex],
                willRestart: patrol.IsLooping
            ));
        }
    }

    /// <summary>
    /// Muda estado da patrulha
    /// </summary>
    private void ChangePatrolState(ref PatrolComponent patrol, PatrolState newState, in NpcTag npcTag)
    {
        var oldState = patrol.State;
        patrol.State = newState;

        // Publica evento de mudança de estado
        GameEventBus.PublishPatrolStateChanged(new PatrolStateChangedEvent(
            entityId: (uint)npcTag.NpcId,
            oldState: oldState,
            newState: newState,
            currentWayPoint: patrol.WayPoints[patrol.CurrentWayPointIndex]
        ));
    }
}
*/
