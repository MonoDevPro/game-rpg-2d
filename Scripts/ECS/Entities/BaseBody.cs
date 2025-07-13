using System;
using Arch.Core;
using GameRpg2D.Scripts.Core.Constants;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Physics;
using GameRpg2D.Scripts.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Entities;

/// <summary>
/// Entidade base para todos os objetos do jogo (Jogadores, NPCs, etc.),
/// estendendo CharacterBody2D para aproveitar física e movimento nativo do Godot.
/// Obtém o World via nó autoload configurado no Godot (Project Settings > Autoload).
/// </summary>
public abstract partial class BaseBody : CharacterBody2D
{
    // Resources
    private AnimatedSprite2D _sprite;

    #region Parameters
    /// <summary>
    /// Caminho para o nó EcsRunner registrado como singleton/autoload
    /// </summary>
    // ReSharper disable once InconsistentNaming
    [Export] protected StringName DefaultAnimation = "idle_south";
    [Export] protected Vocation Vocation = Vocation.Mage;
    [Export] protected Gender Gender = Gender.Male;
    [Export] protected float MoveSpeed = GameConstants.DEFAULT_MOVEMENT_SPEED; // Velocidade de movimento padrão
    [Export] protected float AttackSpeed = GameConstants.DEFAULT_ATTACK_SPEED;
    [Export] protected float AttackCooldown = GameConstants.DEFAULT_ATTACK_COOLDOWN;
    [Export] protected bool EnableDebugCollision = false; // Habilita visualização de colisão para debug
    #endregion

    #region ECS
    /// <summary>
    /// Instância do mundo ECS compartilhado
    /// </summary>
    protected World World { get; private set; }

    /// <summary>
    /// Identificador interno da entidade ECS
    /// </summary>
    protected Entity Entity { get; private set; }
    #endregion

    /// <summary>
    /// Método chamado quando a entidade é criada no ECS.
    /// Subclasses devem implementar RegisterComponents para adicionar seus componentes específicos.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        // Busca o nó EcsRunner no autoload do ECS
        var ecsNode = GameManager.Instance?.EcsRunner;
        if (ecsNode is null)
            throw new InvalidOperationException("O EcsRunner ou não foi adicionado.");

        World = ecsNode.World;

        // Cria a entidade e adiciona componentes padrão
        Entity = World.Create(
            new NodeComponent(this)
        );

        if (!CheckAlive())
            GD.PrintErr($"Falha ao criar entidade {Entity.Id} no ECS.");
        else
            GD.Print($"[ECS] Entidade {Entity.Id} criada com sucesso.");

        RegisterComponents();

        // Adiciona componente de colisão automaticamente
        AddCollisionComponent();

        // Inicializa o sprite e animações
        InitializeSprite();
    }

    /// <summary>
    /// Adiciona um component ECS genérico usando construtor padrão
    /// </summary>
    protected bool AddComponent<T>() where T : IComponent, new()
    {
        if (World.Has<T>(Entity))
        {
            GD.PrintErr($"Component {typeof(T).Name} já registrado em {Entity.Id}.");
            return false;
        }
        else
        {
            GD.Print($"Adicionando component {typeof(T).Name} na entidade {Entity.Id}.");
            World.Add(Entity, new T());
            return true;
        }
    }

    /// <summary>
    /// Adiciona um component ECS genérico (instância fornecida)
    /// </summary>
    protected bool AddComponent<T>(T component) where T : IComponent
    {
        if (World.Has<T>(Entity))
        {
            GD.PrintErr($"Component {typeof(T).Name} já registrado em {Entity.Id}.");
            return false;
        }

        GD.Print($"Adicionando component {typeof(T).Name} na entidade {Entity.Id}.");
        World.Add(Entity, component);
        return true;
    }

    /// <summary>
    /// Remove um component ECS da entidade
    /// </summary>
    protected bool RemoveComponent<T>() where T : IComponent
    {
        if (!World.Has<T>(Entity))
        {
            GD.PrintErr($"Component {typeof(T).Name} não existe em {Entity.Id}.");
            return false;
        }

        GD.Print($"Removendo component {typeof(T).Name} de {Entity.Id}.");
        World.Remove<T>(Entity);
        return true;
    }

    public Entity GetEntity()
    {
        if (!CheckAlive())
            throw new InvalidOperationException($"Entidade {Entity.Id} não está viva no ECS.");

        return Entity;
    }

    /// <summary>
    /// Subclasses devem implementar para registrar seus próprios components
    /// </summary>
    protected abstract void RegisterComponents();

    /// <summary>
    /// Método chamado quando a entidade é removida do ECS
    /// </summary>
    public override void _ExitTree()
    {
        // Checa e log antes de destruir
        if (World.IsAlive(Entity))
            World.Destroy(Entity);
        base._ExitTree();
    }

    /// <summary>
    /// Verifica se a entidade está viva no ECS
    /// </summary>
    protected bool CheckAlive()
    {
        if (World.IsAlive(Entity))
            return true;

        GD.PrintErr($"Entidade {Entity.Id} não está viva no ECS.");
        return false;
    }

    private void InitializeSprite()
    {
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (_sprite == null)
            throw new InvalidOperationException("AnimatedSprite2D não encontrado como filho.");

        var spriteFrames = AssetService.Instance?.GetSpriteFrames(Vocation, Gender);
        if (spriteFrames == null)
            throw new InvalidOperationException($"SpriteFrames não encontrado para {Vocation}/{Gender}");

        _sprite.SpriteFrames = spriteFrames;

        // Não reproduz animação aqui - deixa para o AnimationSystem fazer
        // O AnimationSystem vai definir a animação correta baseado no estado inicial
    }

    /// <summary>
    /// Adiciona componente de colisão automaticamente
    /// </summary>
    private void AddCollisionComponent()
    {
        // Configura componente de colisão
        var collisionComponent = new CollisionComponent
        {
            Body = this,
            CollisionMask = CollisionMask,
            CollisionLayer = CollisionLayer,
            IsColliding = false,
            LastValidPosition = GlobalPosition,
            BlockedDirections = CollisionDirections.None,
            DetectionRadius = GameConstants.GRID_SIZE / 2.0f,
            EnableDebugVisualization = EnableDebugCollision // Pode ser habilitado
        };

        AddComponent(collisionComponent);
    }
}