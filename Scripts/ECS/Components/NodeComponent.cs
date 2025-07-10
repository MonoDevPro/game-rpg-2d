using Godot;

namespace GameRpg2D.Scripts.ECS.Components
{
    public struct NodeComponent(Node2D node)
    {
        public readonly Node2D Node = node;
    }
}
