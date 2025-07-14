using GameRpg2D.Scripts.ECS.Components.AI;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;

namespace GameRpg2D.Scripts.ECS.Entities;

/// <summary>
/// Entidade do jogador local
/// </summary>
public partial class Player : BaseBody
{
    [Export] private int _playerId = 0;

    protected override void RegisterComponents()
    {
        // Tag de jogador local
        AddLocalPlayerTag(_playerId);

        // Componente de input (para capturar teclas e ações do jogador)
        AddInputComponent();
        
        // Componente de navegação (para movimentação do agente)
        AddNavigationComponent();
        
        base.RegisterComponents();
    }
    
    private void AddLocalPlayerTag(int playerId)
        => AddComponent(new LocalPlayerTag(playerId));
    private void AddInputComponent()
        => AddComponent(new InputComponent());

    private void AddNavigationComponent()
        => AddComponent(new NavigationComponent(
            NavigationAgent,
            StartingGridPosition,
            StartingGridPosition,
            0));
}
