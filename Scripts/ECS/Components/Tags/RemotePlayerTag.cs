namespace GameRpg2D.Scripts.ECS.Components.Tags;

/// <summary>
/// Tag para marcar entidades controladas por jogadores remotos (input via rede)
/// </summary>
[Component]
public struct RemotePlayerTag(int playerId)
{
    public readonly int PlayerId = playerId;
}