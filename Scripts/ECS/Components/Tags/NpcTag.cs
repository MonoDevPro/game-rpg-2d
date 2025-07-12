namespace GameRpg2D.Scripts.ECS.Components.Tags;

/// <summary>
/// Tag para marcar entidades controladas por IA (NPCs)
/// </summary>
[Component]
public struct NpcTag(string npcType, int npcId = 0)
{
    public readonly string NPCType = npcType;
    public readonly int NPCId = npcId;
}