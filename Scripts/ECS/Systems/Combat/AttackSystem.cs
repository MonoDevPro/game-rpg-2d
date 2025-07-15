using Arch.Core;
using Arch.Core.Extensions;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components.Combat;
using GameRpg2D.Scripts.ECS.Components.Facing;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Events;
using GameRpg2D.Scripts.ECS.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Combat;
/// <summary>
/// Sistema responsável por processar o combate das entidades
/// </summary>
public partial class AttackSystem(World world) : BaseSystem<World, float>(world)
{
    private double _elapsedTime = 0.0;

    public override void BeforeUpdate(in float delta)
    {
        base.BeforeUpdate(in delta);
        _elapsedTime += delta;
    }

    [Query]
    [All<AttackInputComponent, AttackComponent, FacingComponent>]
    private void ProcessAttackStart(
        ref AttackInputComponent ai,
        ref AttackStateComponent ast,
        in AttackComponent ac,
        in FacingComponent fc)
    {
        // Verifica se o ataque já está ativo
        if (ast.IsActive)
            return;
        
        if (!ai.IsAttacking)
            return;
        
        // Cooldown: tempo desde o último ataque
        var sinceLast = _elapsedTime - ai.LastAttackTime;
        if (sinceLast < ac.AttackCooldown)
            return;

        // Direção válida
        var dir = fc.CurrentDirection;
        if (dir == Direction.None)
        {
            GD.Print("[AttackSystem] Direção inválida, ataque cancelado!");
            return;
        }

        ast.IsActive = true;
        ast.Progress = 0f; // reinicia o progresso do ataque
        ast.StartTime = _elapsedTime; // marca o tempo de início do ataque

        GD.Print($"[AttackSystem] Ataque iniciado na direção {dir}");
    }
    
    [Query]
    [All<AttackComponent, AttackStateComponent, AttackInputComponent>]
    private void ProcessAttackProgress(
        in AttackComponent ac,
        ref AttackStateComponent st,
        ref AttackInputComponent ai)
    {
        if (!st.IsActive)
            return;
        
        var timeSinceStart = _elapsedTime - st.StartTime;
        st.StartTime += _elapsedTime - st.StartTime; // atualiza o tempo de início com o delta

        // Avança o progresso com base no delta
        st.Progress += timeSinceStart / ac.AttackSpeed;
        
        if (st.Progress < 1f)
            return;

        // Fim do ataque
        GameEventBus.PublishEntityAttack(new EntityAttackEvent(
            attackerId:   0,
            attackDirection: Direction.None,
            damage:       ac.BaseDamage,
            range:        ac.GridAttackRange
        ));

        GD.Print($"[AttackSystem] Ataque concluído! Dano: {ac.BaseDamage}");
            
        // Começa o cooldown AQUI
        ai.LastAttackTime = _elapsedTime;
        st.IsActive = false; // desativa o estado de ataque
    }


}