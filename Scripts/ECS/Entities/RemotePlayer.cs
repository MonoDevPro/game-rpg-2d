using GameRpg2D.Scripts.Core.Constants;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components.Animation;
using GameRpg2D.Scripts.ECS.Components.Combat;
using GameRpg2D.Scripts.ECS.Components.Movement;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;

namespace GameRpg2D.Scripts.ECS.Entities;

/// <summary>
/// Entidade do jogador remoto (para multiplayer)
/// </summary>
public partial class RemotePlayer : BaseBody
{
    [Export] private int _playerId = 0;
    [Export] private string _sessionId = "";

    protected override void RegisterComponents()
    {
        // Tag de jogador remoto
        AddComponent(new RemotePlayerTag(_playerId, _sessionId));

        base.RegisterComponents();
    }
}
