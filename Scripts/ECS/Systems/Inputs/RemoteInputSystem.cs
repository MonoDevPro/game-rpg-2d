using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Components.Tags;

namespace GameRpg2D.Scripts.ECS.Systems.Inputs;

/*
public partial class RemoteInputSystem(World world) : BaseSystem<World, float>(world)
{
    [Query, All<MovementInputComponent, RemotePlayerTag>]
    private void UpdateRemoteInput(
        ref MovementInputComponent input,
        in RemotePlayerTag tag)
    {
        // supondo que net.MovementVector venha da rede
        input.RawMovement = net.MovementVector;
        input.IsMoving    = input.RawMovement.LengthSquared() > 0;
        input.JustStarted = true;  // algum flag vindo da rede
        // direction igual ao local:
        input.MovementDirection = input.IsMoving
            ? GetDirectionFromInput(input.RawMovement)
            : Direction.None;
        // timestamps, etc...
    }
}
*/
