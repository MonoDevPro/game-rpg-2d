using System.Runtime.CompilerServices;
using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using GameRpg2D.Scripts.ECS.Components;
using Godot;

namespace GameRpg2D.Scripts.ECS;

/// <summary>
/// Sistema responsável pela renderização e sincronização das posições dos nodes
/// </summary>
public partial class RenderSystem : BaseSystem<World, float>
{
    public RenderSystem(World world) : base(world) { }
    
    /// <summary>
    /// Sincroniza posições do mundo ECS com os nodes do Godot
    /// </summary>
    [Query]
    [All<PositionComponent, NodeComponent>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SyncNodePositions(in PositionComponent position, ref NodeComponent node)
    {
        if (node.Node != null && GodotObject.IsInstanceValid(node.Node))
        {
            node.Node.Position = position.ToVector2();
        }
    }

    /// <summary>
    /// Atualiza a ordem de renderização baseada na posição Y (depth sorting)
    /// </summary>
    [Query]
    [All<PositionComponent, NodeComponent>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateZIndex(in PositionComponent position, ref NodeComponent node)
    {
        if (node.Node != null && GodotObject.IsInstanceValid(node.Node))
        {
            // Sprites mais embaixo (Y maior) devem ser renderizados por cima
            node.Node.ZIndex = (int)position.Y;
        }
    }

    /// <summary>
    /// Atualiza a visibilidade dos nodes
    /// </summary>
    [Query]
    [All<PositionComponent, NodeComponent>]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UpdateVisibility(in PositionComponent position, ref NodeComponent node)
    {
        if (node.Node == null || !GodotObject.IsInstanceValid(node.Node))
            return;

        // Mantém a visibilidade conforme o componente
        if (node.Node.Visible != node.IsVisible)
        {
            node.Node.Visible = node.IsVisible;
        }
    }

    /// <summary>
    /// Remove nodes inválidos do sistema (cleanup)
    /// </summary>
    [Query]
    [All<NodeComponent>]
    public void CleanupInvalidNodes(ref NodeComponent node, in Entity entity)
    {
        if (node.Node == null || !GodotObject.IsInstanceValid(node.Node))
        {
            // Remove a entidade se o node for inválido
            World.Destroy(entity);
        }
    }
}