using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Constants;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components.Animation;
using GameRpg2D.Scripts.ECS.Components.Combat;
using GameRpg2D.Scripts.ECS.Components.Core;
using GameRpg2D.Scripts.ECS.Components.Input;
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
    public AttackSystem(World world) : base(world) { }

    /// <summary>
    /// Processa input de ataque para jogadores locais
    /// </summary>
    [Query]
    [All<AttackComponent, InputComponent, MovementComponent, LocalPlayerTag>]
    private void ProcessAttackInput([Data] in float deltaTime, ref AttackComponent attack, in InputComponent input, in MovementComponent movement, in LocalPlayerTag playerTag)
    {
        var currentTime = Time.GetTicksMsec() / 1000.0; // Converte para segundos

        // Verifica se pode atacar (não está atacando e passou o cooldown)
        var canAttack = !attack.IsAttacking &&
                       (currentTime - attack.LastAttackTime) >= attack.AttackCooldown;

        // Se recebeu input de ataque e pode atacar
        // Suporta tanto ataque único (JustPressed) quanto contínuo (Pressed após cooldown)
        var wantsToAttack = input.IsAttackJustPressed ||
                           (input.IsAttackPressed && canAttack);

        if (wantsToAttack && canAttack)
        {
            // Inicia ataque na direção atual do movimento
            var attackDirection = movement.CurrentDirection != Direction.None
                ? movement.CurrentDirection
                : Direction.South; // Direção padrão

            attack.LastAttackTime = currentTime;
            attack.IsAttacking = true;
            attack.AttackDirection = attackDirection;
            attack.AttackProgress = 0.0f;
        }
    }

    /// <summary>
    /// Processa o progresso dos ataques em andamento
    /// </summary>
    [Query]
    [All<AttackComponent>]
    private void ProcessAttackProgress([Data] in float deltaTime, ref AttackComponent attack, in Entity entity)
    {
        if (!attack.IsAttacking)
            return;        // Calcula progresso do ataque
        var progressIncrement = deltaTime / attack.AttackSpeed;
        var newProgress = Mathf.Clamp(attack.AttackProgress + progressIncrement, 0.0f, 1.0f);

        // Atualiza progresso
        attack.AttackProgress = newProgress;
        attack.IsAttacking = newProgress < 1.0f;

        // Se ataque foi concluído
        if (newProgress >= 1.0f)
        {
            // Aqui seria onde processaríamos dano, hit detection, etc.
            ExecuteAttackEffect(attack, entity.Id);
        }
    }

    /// <summary>
    /// Executa os efeitos do ataque (dano, hit detection, etc.)
    /// </summary>
    private void ExecuteAttackEffect(AttackComponent attack, int attackerId)
    {
        // Publica evento de ataque
        GameEventBus.PublishEntityAttack(new EntityAttackEvent(
            attackerId: (uint)attackerId,
            attackDirection: attack.AttackDirection,
            damage: attack.BaseDamage,
            range: attack.AttackRange
        ));

        // TODO: Implementar detecção de hit, aplicação de dano, etc.
        GD.Print($"Ataque executado! Direção: {attack.AttackDirection}, Dano: {attack.BaseDamage}");
    }
}
