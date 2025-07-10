using Arch.Core;
using GameRpg2D.Scripts.ECS.Components;
using Godot;

namespace GameRpg2D.Scripts.ECS.Systems
{
    public class RenderSystem(World world)
    {
        private readonly QueryDescription _renderQuery = new QueryDescription()
            .WithAll<PositionComponent, NodeComponent>();

        public void Update()
        {
            world.Query(in _renderQuery, (ref PositionComponent position, ref NodeComponent node) =>
            {
                if (node.Node != null && GodotObject.IsInstanceValid(node.Node))
                {
                    node.Node.Position = position.WorldPosition;
                }
            });
        }
    }
}
