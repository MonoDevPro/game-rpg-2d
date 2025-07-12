using GameRpg2D.Scripts.ECS.Components;

namespace GameRpg2D.Scripts.ECS.Components.Tags;

/// <summary>
/// Tag que identifica entidades controladas por jogadores remotos
/// </summary>
public readonly struct RemotePlayerTag : IComponent
{
    /// <summary>
    /// ID do jogador remoto
    /// </summary>
    public readonly int PlayerId;

    /// <summary>
    /// ID da sessão de rede
    /// </summary>
    public readonly string SessionId;

    public RemotePlayerTag(int playerId, string sessionId = "")
    {
        PlayerId = playerId;
        SessionId = sessionId;
    }
}
