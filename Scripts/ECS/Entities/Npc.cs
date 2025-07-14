using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Data.Behaviours;
using GameRpg2D.Scripts.ECS.Components.AI;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;

namespace GameRpg2D.Scripts.ECS.Entities;

/// <summary>
/// Entidade NPC
/// </summary>
public partial class Npc : BaseBody
{
    [Export] private int _npcId = 0;
    [Export] private string _npcName = "";
    
    [ExportCategory("Behaviour")]
    [Export] private NpcBehaviourType _behaviourType = NpcBehaviourType.Idle;

    [Export]
    private PatrolData _patrolData = new();
    
    protected override void RegisterComponents()
    {
        // Tag de NPC
        AddComponent(new NpcTag(_npcId, _behaviourType, _npcName));

        // Componente de patrulha (se NPC tem comportamento Patrol)
        if (_behaviourType == NpcBehaviourType.Patrol)
            AddPatrolComponent(_patrolData);
        
        base.RegisterComponents();
    }

    /// <summary>
    /// Adiciona componente de patrulha ao NPC
    /// </summary>
    private void AddPatrolComponent(PatrolData patrolData)
    {
        if (patrolData.IsEmpty)
        {
            GD.PrintErr($"[Npc] {_npcName} não possui waypoints definidos para patrulha.");
            return;
        }

        AddComponent(new PatrolComponent
        {
            WayPoints = [.. patrolData.PatrolWaypoints], // Copia os waypoints
            CurrentWayPointIndex = 0,
            State = PatrolState.Moving,
            PatrolDirection = Direction.South,
            WaitTimer = 0.0f,
            WaitDuration = patrolData.WaitDuration,
            PatrolSpeed = patrolData.PatrolSpeed,
            IsLooping = patrolData.IsLooping,
            ReverseOnEnd = patrolData.ReverseOnEnd,
            IsReversing = false,
            InitialWayPoint = patrolData.PatrolWaypoints[0],
            WayPointTolerance = patrolData.WayPointTolerance
        });

        GD.Print($"[Npc] {_npcName} configurado para patrulha com {patrolData.PatrolWaypoints.Count} waypoints");
    }
}
