using Godot;

namespace GameRpg2D.Scripts.ECS.Components;

public readonly struct NodeComponent(Node2D node) : IComponent
{
    public readonly Node2D Node = node;
    public bool IsVisible => Node.Visible;
}