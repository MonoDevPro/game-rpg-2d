using System.Runtime.CompilerServices;
using Arch.Bus;
using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Events;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.Utilities;
using Godot;

namespace GameRpg2D.Scripts.ECS;

/// <summary>
/// Sistema responsável pelo movimento das entidades no mundo
/// </summary>
public partial class MovementSystem : BaseSystem<World, float>
{
    public MovementSystem(World world) : base(world) { }

    /// <summary>
    /// Processa o movimento das entidades que estão se movendo
    /// </summary>
    [Query]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessMovement(
        [Data] in float deltaTime,
        ref MovementComponent movement,
        ref PositionComponent position,
        in MovementConfigComponent config)
    {
        if (!movement.IsMoving)
            return;

        // Incrementa o progresso do movimento
        movement.Progress += config.MoveSpeed * deltaTime;

        // Se o movimento foi completado
        if (movement.Progress >= 1.0f)
        {
            // Finaliza o movimento
            movement.Progress = 1.0f;
            position = new PositionComponent(GridUtils.GridToWorld(movement.TargetGridPosition));
            
            // Reset do movimento
            movement.Direction = Vector2I.Zero;
            movement.StartGridPosition = movement.TargetGridPosition;
            movement.Progress = 0.0f;
        }
        else
        {
            // Interpola a posição atual
            var startWorld = GridUtils.GridToWorld(movement.StartGridPosition);
            var targetWorld = GridUtils.GridToWorld(movement.TargetGridPosition);
            var currentPosition = startWorld.Lerp(targetWorld, movement.Progress);
            
            position = new PositionComponent(currentPosition);
        }
    }

    /// <summary>
    /// Processa o timer de parada de input para entidades que pararam de se mover
    /// </summary>
    [Query]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessInputStopTimer(
        [Data] in float deltaTime,
        ref MovementComponent movement,
        in MovementConfigComponent config)
    {
        if (movement.IsMoving || movement.HasContinuousInput)
            return;

        movement.TimeSinceLastInput += deltaTime;
        
        // Se passou tempo suficiente desde o último input, para completamente
        if (movement.TimeSinceLastInput >= config.InputStopDelay)
        {
            movement.TimeSinceLastInput = 0.0f;
        }
    }

    /// <summary>
    /// Aplica movimento baseado em velocidade para entidades com VelocityComponent
    /// </summary>
    [Query]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyVelocity(
        [Data] in float deltaTime,
        ref PositionComponent position,
        ref VelocityComponent velocity)
    {
        position.X += velocity.X * deltaTime;
        position.Y += velocity.Y * deltaTime;
    }

    /// <summary>
    /// Zera a velocidade de entidades mortas ou inativas
    /// </summary>
    [Query]
    [All<NpcTag>, None<LocalPlayerTag>]
    public void ResetVelocityForInactive(ref VelocityComponent velocity)
    {
        // Implementar lógica para zerar velocidade de NPCs inativos
        // Por exemplo, quando estão em estado idle por muito tempo
    }

    /// <summary>
    /// Processa movimento com física simples e detecção de colisão
    /// </summary>
    [Query]
    [All<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessPlayerMovementWithPhysics(
        [Data] in float deltaTime,
        ref MovementComponent movement,
        ref PositionComponent position,
        in MovementConfigComponent config,
        in Entity entity)
    {
        if (!movement.IsMoving)
            return;

        var oldGridPosition = movement.StartGridPosition;
        
        // Processa movimento normalmente
        movement.Progress += config.MoveSpeed * deltaTime;

        if (movement.Progress >= 1.0f)
        {
            // Finaliza o movimento
            movement.Progress = 1.0f;
            position = new PositionComponent(GridUtils.GridToWorld(movement.TargetGridPosition));
            
            // Dispara evento de movimento completo
            var completedEvent = new MovementCompletedEvent(entity, oldGridPosition, movement.TargetGridPosition);
            EventBus.Send(ref completedEvent);
            
            // Reset do movimento
            movement.Direction = Vector2I.Zero;
            movement.StartGridPosition = movement.TargetGridPosition;
            movement.Progress = 0.0f;
        }
        else
        {
            // Interpola a posição atual
            var startWorld = GridUtils.GridToWorld(movement.StartGridPosition);
            var targetWorld = GridUtils.GridToWorld(movement.TargetGridPosition);
            var currentPosition = startWorld.Lerp(targetWorld, movement.Progress);
            
            position = new PositionComponent(currentPosition);
        }
    }

    /// <summary>
    /// Sistema de movimento para NPCs com comportamento diferenciado
    /// </summary>
    [Query]
    [All<NpcTag>, None<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessNpcMovement(
        [Data] in float deltaTime,
        ref MovementComponent movement,
        ref PositionComponent position,
        in MovementConfigComponent config,
        in Entity entity)
    {
        if (!movement.IsMoving)
            return;

        // NPCs podem ter velocidade diferente
        var npcSpeed = config.MoveSpeed * 0.8f; // 20% mais lentos que o player
        movement.Progress += npcSpeed * deltaTime;

        if (movement.Progress >= 1.0f)
        {
            movement.Progress = 1.0f;
            position = new PositionComponent(GridUtils.GridToWorld(movement.TargetGridPosition));
            
            // Dispara evento específico para NPCs
            var completedEvent = new MovementCompletedEvent(entity, movement.StartGridPosition, movement.TargetGridPosition);
            EventBus.Send(ref completedEvent);
            
            movement.Direction = Vector2I.Zero;
            movement.StartGridPosition = movement.TargetGridPosition;
            movement.Progress = 0.0f;
        }
        else
        {
            var startWorld = GridUtils.GridToWorld(movement.StartGridPosition);
            var targetWorld = GridUtils.GridToWorld(movement.TargetGridPosition);
            var currentPosition = startWorld.Lerp(targetWorld, movement.Progress);
            
            position = new PositionComponent(currentPosition);
        }
    }

    /// <summary>
    /// Dispara evento de movimento completo
    /// </summary>
    public void MovementCompleted(Entity entity, Vector2I oldGridPosition, Vector2I newGridPosition)
    {
        var eventCompleted = new MovementCompletedEvent(entity, oldGridPosition, newGridPosition);
        // Dispara evento de movimento completo
        EventBus.Send(ref eventCompleted);
    }
}