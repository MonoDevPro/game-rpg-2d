using System;
using System.Collections.Generic;
using GameRpg2D.Scripts.ECS.Events;
using Godot;

namespace GameRpg2D.Scripts.ECS.Infrastructure;

/// <summary>
/// Gerenciador de eventos global para o sistema ECS
/// </summary>
public static class GameEventBus
{
    private static readonly Dictionary<Type, List<object>> Subscribers = new();

    #region Subscription Methods

    /// <summary>
    /// Inscreve um handler para eventos
    /// </summary>
    public static void Subscribe<T>(Action<T> handler) where T : struct
    {
        var eventType = typeof(T);
        if (!Subscribers.TryGetValue(eventType, out var value))
        {
            value = [];
            Subscribers[eventType] = value;
        }

        value.Add(handler);
    }

    /// <summary>
    /// Remove inscrição de um handler
    /// </summary>
    public static void Unsubscribe<T>(Action<T> handler) where T : struct
    {
        var eventType = typeof(T);
        if (Subscribers.TryGetValue(eventType, out var subscriber))
            subscriber.Remove(handler);
    }

    #endregion

    #region Event Publishing Methods

    /// <summary>
    /// Publica um evento genérico
    /// </summary>
    public static void Publish<T>(T eventData) where T : struct
    {
        var eventType = typeof(T);
        
        if (!Subscribers.TryGetValue(eventType, out var subscriber1)) 
            return;
        
        foreach (var subscriber in subscriber1)
            if (subscriber is Action<T> handler)
                handler(eventData);
    }

    /// <summary>
    /// Publica um evento de movimento
    /// </summary>
    public static void PublishEntityMoved(EntityMovedEvent eventData)
    {
        Publish(eventData);
        GD.Print($"[EventBus] Entity {eventData.EntityId} moved from {eventData.FromGridPosition} to {eventData.ToGridPosition}");
    }

    /// <summary>
    /// Publica um evento de ataque
    /// </summary>
    public static void PublishEntityAttack(EntityAttackEvent eventData)
    {
        Publish(eventData);
        GD.Print($"[EventBus] Entity {eventData.AttackerId} attacked in direction {eventData.AttackDirection} with damage {eventData.Damage}");
    }

    /// <summary>
    /// Publica um evento de mudança de animação
    /// </summary>
    public static void PublishAnimationChanged(AnimationChangedEvent eventData)
    {
        Publish(eventData);
        GD.Print($"[EventBus] Entity {eventData.EntityId} animation changed from {eventData.OldState} to {eventData.NewState}");
    }

    #endregion

    /// <summary>
    /// Limpa todas as inscrições (útil para cleanup)
    /// </summary>
    public static void Clear()
    {
        Subscribers.Clear();
        GD.Print("[EventBus] All subscriptions cleared");
    }
}
