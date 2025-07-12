using System.Runtime.CompilerServices;
using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Constants;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.Utilities;
using Godot;

namespace GameRpg2D.Scripts.ECS;

/// <summary>
/// Sistema responsável por gerenciar animações das entidades
/// </summary>
public partial class AnimationSystem : BaseSystem<World, float>
{
    public AnimationSystem(World world) : base(world) { }

    /// <summary>
    /// Atualiza estado da animação baseado no movimento
    /// </summary>
    [Query]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateAnimationState(
        ref AnimationComponent animation,
        in MovementComponent movement)
    {
        var newState = movement.IsMoving ? AnimationState.Walk : AnimationState.Idle;
        
        if (animation.CurrentState != newState)
        {
            animation.PreviousState = animation.CurrentState;
            animation.CurrentState = newState;
            animation.HasChanged = true;
        }
    }

    /// <summary>
    /// Atualiza direção da animação baseado no movimento
    /// </summary>
    [Query]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateAnimationDirection(
        ref AnimationComponent animation,
        in MovementComponent movement)
    {
        if (movement.Direction != Vector2I.Zero)
        {
            var newDirection = movement.Direction.ToDirection();
            
            if (animation.CurrentDirection != newDirection)
            {
                animation.PreviousDirection = animation.CurrentDirection;
                animation.CurrentDirection = newDirection;
                animation.HasChanged = true;
            }
        }
    }

    /// <summary>
    /// Aplica animações no AnimatedSprite2D
    /// </summary>
    [Query]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ApplyAnimations(
        ref AnimationComponent animation,
        in AnimatedSpriteComponent sprite)
    {
        if (!animation.HasChanged || sprite.AnimatedSprite == null)
            return;

        if (!GodotObject.IsInstanceValid(sprite.AnimatedSprite))
            return;

        var animationName = GetAnimationName(animation.CurrentState, animation.CurrentDirection);
        
        if (sprite.AnimatedSprite.SpriteFrames != null && 
            sprite.AnimatedSprite.SpriteFrames.HasAnimation(animationName))
        {
            sprite.AnimatedSprite.Animation = animationName;
            sprite.AnimatedSprite.SpeedScale = animation.AnimationSpeed;
            
            if (sprite.AutoPlay && !sprite.AnimatedSprite.IsPlaying())
            {
                sprite.AnimatedSprite.Play();
            }
        }

        animation.HasChanged = false;
    }

    /// <summary>
    /// Processa animações específicas para o player
    /// </summary>
    [Query]
    [All<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessPlayerAnimations(
        ref AnimationComponent animation,
        in AnimatedSpriteComponent sprite,
        in MovementComponent movement)
    {
        // Player pode ter animações especiais
        if (movement.IsMoving)
        {
            animation.AnimationSpeed = 1.0f;
        }
        else
        {
            animation.AnimationSpeed = 0.5f; // Idle mais lento
        }
    }

    /// <summary>
    /// Processa animações específicas para NPCs
    /// </summary>
    [Query]
    [All<NpcTag>, None<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessNpcAnimations(
        ref AnimationComponent animation,
        in AnimatedSpriteComponent sprite,
        in MovementComponent movement)
    {
        // NPCs podem ter comportamentos de animação diferentes
        if (movement.IsMoving)
        {
            animation.AnimationSpeed = 0.8f; // Mais lentos que o player
        }
        else
        {
            animation.AnimationSpeed = 0.3f; // Idle bem lento
        }
    }

    /// <summary>
    /// Gera o nome da animação baseado no estado e direção
    /// </summary>
    private static string GetAnimationName(AnimationState state, Direction direction)
    {
        return state switch
        {
            AnimationState.Idle => $"idle_{direction.ToString().ToLower()}",
            AnimationState.Walk => $"walk_{direction.ToString().ToLower()}",
            AnimationState.Attack => $"attack_{direction.ToString().ToLower()}",
            AnimationState.Death => "death",
            _ => "idle_down"
        };
    }
}