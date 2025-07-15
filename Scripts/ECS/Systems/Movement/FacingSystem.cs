using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components.Combat;
using GameRpg2D.Scripts.ECS.Components.Facing;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Movement;

/// <summary>
/// Sistema responsável por processar o movimento de todas as entidades
/// </summary>
public partial class FacingSystem(World world) : BaseSystem<World, float>(world)
{
    // Alternativa: permitir mudança de direção apenas se não estiver atacando
    [Query, All<FacingComponent, MovementInputComponent, AttackStateComponent>]
    private void UpdateFacingDirection(
        ref FacingComponent facing,
        in MovementInputComponent input,
        in AttackStateComponent attackState)
    {
        // Só atualiza direção se não estiver atacando
        if (attackState.IsActive || input.RawMovement == Vector2.Zero)
            return;
        
        facing.LastDirection    = facing.CurrentDirection;
        facing.CurrentDirection = DirectionHelper.VectorToDirection(input.RawMovement);
    }
}
