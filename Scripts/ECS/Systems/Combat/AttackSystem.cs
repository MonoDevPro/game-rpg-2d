using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components.Combat;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Components.Movement;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.ECS.Events;
using GameRpg2D.Scripts.ECS.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Combat;
/// <summary>
/// Sistema responsável por processar o combate das entidades
/// </summary>
public partial class AttackSystem : BaseSystem<World, float>
{
    private double _elapsedTime = 0.0;

    public AttackSystem(World world) : base(world) { }

    public override void BeforeUpdate(in float delta)
    {
        base.BeforeUpdate(in delta);
        _elapsedTime += delta;
    }

    /// <summary>
    /// Processa input de ataque para jogadores locais
    /// </summary>
    [Query, All<AttackComponent, InputComponent, MovementComponent, LocalPlayerTag>]
    private void ProcessAttackInput(ref AttackComponent attack, in InputComponent input, in MovementComponent movement)
    {
        // Verifica se pode atacar (não está em progresso e passou o cooldown)
        var canAttack = !attack.IsAttacking &&
                        (_elapsedTime - attack.LastAttackTime) >= attack.AttackCooldown;

        // Suporta ataque único (JustPressed) e contínuo (Pressed após cooldown)
        var wantsToAttack = input.IsAttackJustPressed ||
                            (input.IsAttackPressed && canAttack);

        if (wantsToAttack && canAttack)
        {
            // Define direção de ataque (ou movendo-se, ou padrão)
            attack.AttackDirection = movement.CurrentDirection != Direction.None
                ? movement.CurrentDirection
                : Direction.South;

            attack.LastAttackTime = _elapsedTime;
            attack.IsAttacking = true;
            attack.AttackProgress = 0.0f;
        }
    }

    /// <summary>
    /// Processa o progresso dos ataques em andamento
    /// </summary>
    [Query, All<AttackComponent>]
    private void ProcessAttackProgress([Data] in float deltaTime, ref AttackComponent attack, in Entity entity)
    {
        if (!attack.IsAttacking)
            return;

        var increment = deltaTime / attack.AttackSpeed;
        var newProgress = Mathf.Clamp(attack.AttackProgress + increment, 0.0f, 1.0f);

        attack.AttackProgress = newProgress;
        attack.IsAttacking = newProgress < 1.0f;

        if (newProgress >= 1.0f)
        {
            ExecuteAttackEffect(attack, entity.Id);
        }
    }

    /// <summary>
    /// Executa os efeitos do ataque (dano, hit detection, etc.)
    /// </summary>
    private void ExecuteAttackEffect(in AttackComponent attack, int attackerId)
    {
        GameEventBus.PublishEntityAttack(new EntityAttackEvent(
            attackerId: (uint)attackerId,
            attackDirection: attack.AttackDirection,
            damage: attack.BaseDamage,
            range: attack.AttackRange
        ));

        GD.Print($"[AttackSystem] Ataque executado! Direção: {attack.AttackDirection}, Dano: {attack.BaseDamage}");
    }
}