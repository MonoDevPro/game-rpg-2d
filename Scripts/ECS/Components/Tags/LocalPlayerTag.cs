using GameRpg2D.Scripts.ECS.Components;

namespace GameRpg2D.Scripts.ECS.Components.Tags;

/// <summary>
/// Tag que identifica entidades controladas pelo jogador local
/// </summary>
public readonly struct LocalPlayerTag : IComponent
{
    /// <summary>
    /// ID do jogador local (útil para multiplayer)
    /// </summary>
    public readonly int PlayerId;

    public LocalPlayerTag(int playerId = 0)
    {
        PlayerId = playerId;
    }
}
