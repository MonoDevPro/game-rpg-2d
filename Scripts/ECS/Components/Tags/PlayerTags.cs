namespace GameRpg2D.Scripts.ECS.Components.Tags
{
    /// <summary>
    /// Tag para marcar entidades controladas pelo jogador local (input direto do dispositivo)
    /// </summary>
    public struct LocalPlayerTag
    {
        // Empty struct - usado apenas como marcador/tag
    }
    
    /// <summary>
    /// Tag para marcar entidades controladas por jogadores remotos (input via rede)
    /// </summary>
    public struct RemotePlayerTag(int playerId)
    {
        public readonly int PlayerId = playerId;
    }
    
    /// <summary>
    /// Tag para marcar entidades controladas por IA (NPCs)
    /// </summary>
    public struct NPCTag(string npcType, int npcId = 0)
    {
        public readonly string NPCType = npcType;
        public readonly int NPCId = npcId;
    }
}

