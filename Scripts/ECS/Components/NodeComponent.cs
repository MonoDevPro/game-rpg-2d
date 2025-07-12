using Godot;

namespace GameRpg2D.Scripts.ECS.Components;

[Component]
public struct NodeComponent
{
    public Node2D Node;
    public bool IsVisible;
    
    public NodeComponent(Node2D node)
    {
        Node = node;
        IsVisible = true;
    }
}