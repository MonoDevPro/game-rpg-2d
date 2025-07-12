using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components;

namespace GameRpg2D.Scripts.ECS.Components.Tags;

/// <summary>
/// Tag que identifica entidades NPC
/// </summary>
public readonly struct NpcTag : IComponent
{
    /// <summary>
    /// ID único do NPC
    /// </summary>
    public readonly int NpcId;

    /// <summary>
    /// Tipo de comportamento do NPC
    /// </summary>
    public readonly NpcBehaviourType BehaviourType;

    /// <summary>
    /// Nome do NPC
    /// </summary>
    public readonly string Name;

    public NpcTag(int npcId, NpcBehaviourType behaviourType = NpcBehaviourType.Idle, string name = "")
    {
        NpcId = npcId;
        BehaviourType = behaviourType;
        Name = name;
    }
}
