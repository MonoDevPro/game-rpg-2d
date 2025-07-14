using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace GameRpg2D.Scripts.Data.Behaviours;

public partial class PatrolData : Resource
{
    // Configurações de patrulha
    [Export] public Vector2I StartGridPosition = Vector2I.Zero;
    [Export] public float PatrolSpeed = 50.0f;
    [Export] public float WaitDuration = 2.0f;
    [Export] public bool IsLooping = true;
    [Export] public bool ReverseOnEnd = false;
    [Export] public float WayPointTolerance = 0.5f;
    // Waypoints
    [Export] 
    public Array<Vector2I> PatrolWaypoints = [];
    public bool IsEmpty => PatrolWaypoints.Count == 0;
    
    public void AddWayPoint(Vector2I wayPoint)
    {
        if (!PatrolWaypoints.Contains(wayPoint))
        {
            PatrolWaypoints.Add(wayPoint);
        }
    }
    
    public void RemoveWayPoint(Vector2I wayPoint)
    {
        if (PatrolWaypoints.Contains(wayPoint))
        {
            PatrolWaypoints.Remove(wayPoint);
        }
    }
    
    public void ClearWayPoints()
    {
        PatrolWaypoints.Clear();
    }
}