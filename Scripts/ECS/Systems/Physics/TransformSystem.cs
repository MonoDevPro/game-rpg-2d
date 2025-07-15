using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.ECS.Components.Physics;

namespace GameRpg2D.Scripts.ECS.Systems.Physics;

public partial class TransformSystem(World world) : BaseSystem<World, float>(world)
{
    [Query, All<TransformComponent, NodeComponent>]
    private void UpdateTransform([Data] in float dt,
        in TransformComponent tf, in NodeComponent node)
    {
        // Posiciona no espaço local (ou use GlobalPosition se preferir)
        node.Node.Position = tf.WorldPosition;
        node.Node.Scale    = tf.Scale;
        node.Node.Rotation = tf.Rotation;
    }
}