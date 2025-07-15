using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components.Facing;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Components.Movement;
using GameRpg2D.Scripts.ECS.Components.Physics;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.ECS.Events;
using GameRpg2D.Scripts.ECS.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Movement;

/// <summary>
/// Sistema responsável por processar o movimento de todas as entidades
/// </summary>
public partial class FacingSystem(World world) : BaseSystem<World, float>(world)
{
    [Query, All<FacingComponent, MovementInputComponent>]
    private void UpdateFacingDirection(
        ref FacingComponent facing,
        in MovementInputComponent input)
    {
        if (input.RawMovement == Vector2.Zero)
            return;
        
        // 3) Atualiza o componente
        facing.LastDirection    = facing.CurrentDirection;
        facing.CurrentDirection = DirectionHelper.VectorToDirection(input.RawMovement);
    }
}
