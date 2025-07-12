using System.Runtime.CompilerServices;
using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.ECS;

/// <summary>
/// Sistema responsável por gerenciar animações de ataque (apenas visual)
/// </summary>
public partial class AttackSystem : BaseSystem<World, float>
{
    public AttackSystem(World world) : base(world) { }

    /// <summary>
    /// Processa input de ataque para o player (apenas para animação)
    /// </summary>
    [Query]
    [All<LocalPlayerTag>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessPlayerAttackInput(
        [Data] in float deltaTime,
        ref AttackComponent attack,
        ref AttackConfigComponent config,
        in LocalInputComponent input,
        in AnimationComponent animation)
    {
        // Atualiza timer de cooldown
        config.LastAttackTime += deltaTime;

        // Só pode atacar se não estiver atacando e passou o cooldown
        if (!attack.IsAttacking && input.AttackJustPressed && 
            config.LastAttackTime >= config.AttackCooldown)
        {
            // Inicia animação de ataque
            attack.IsAttacking = true;
            attack.AttackTimer = 0.0f;
            attack.AttackDirection = animation.CurrentDirection;
            attack.AttackDuration = config.AttackDuration;
            
            config.LastAttackTime = 0.0f;
        }
    }

    /// <summary>
    /// Processa duração da animação de ataque
    /// </summary>
    [Query]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ProcessAttackDuration(
        [Data] in float deltaTime,
        ref AttackComponent attack)
    {
        if (!attack.IsAttacking)
            return;

        attack.AttackTimer += deltaTime;

        // Finaliza animação de ataque quando acabar a duração
        if (attack.AttackTimer >= attack.AttackDuration)
        {
            attack.IsAttacking = false;
            attack.AttackTimer = 0.0f;
        }
    }

    /// <summary>
    /// Atualiza estado da animação baseado no ataque
    /// </summary>
    [Query]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateAttackAnimation(
        ref AnimationComponent animation,
        in AttackComponent attack)
    {
        if (attack.IsAttacking)
        {
            // Muda para animação de ataque
            if (animation.CurrentState != AnimationState.Attack)
            {
                animation.PreviousState = animation.CurrentState;
                animation.CurrentState = AnimationState.Attack;
                animation.CurrentDirection = attack.AttackDirection;
                animation.HasChanged = true;
            }
        }
        else
        {
            // Volta para idle/walk se não estiver atacando
            if (animation.CurrentState == AnimationState.Attack)
            {
                animation.PreviousState = animation.CurrentState;
                animation.CurrentState = AnimationState.Idle;
                animation.HasChanged = true;
            }
        }
    }
}