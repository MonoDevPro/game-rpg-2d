/*
using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components.AI;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Components.Physics;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems.Inputs;

public partial class AiInputSystem : BaseSystem<World, float>
{
    public AiInputSystem(World world) : base(world) { }

    [Query, All<MovementInputComponent, NpcTag, PatrolComponent, GridPositionComponent>]
    private void UpdateNpcInput(
        [Data] in float delta,
        ref MovementInputComponent input,
        in PatrolComponent patrol,
        in GridPositionComponent grid)
    {
        // lógica de patrulha: decide se deve andar e em que direção
        var nextPoint = patrol.GetNextPatrolPoint(grid.GridPosition);
        var offset = nextPoint - grid.GridPosition;
        input.IsMoving = offset != Vector2I.Zero;
        input.MovementDirection = input.IsMoving
            ? DirectionHelper.VectorToDirection(offset)
            : Direction.None;
        input.RawMovement = PositionHelper.DirectionToVector(input.MovementDirection).ToVector2();
        // JustStarted = se passou de stopped → moving
        input.JustStarted = input.IsMoving && !patrol.WasMovingLastFrame;
        // marque no PatrolComponent para próximo frame…
    }
}
*/
