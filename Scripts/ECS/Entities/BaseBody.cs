using System;
using System.Collections.Generic;
using Arch.Core;
using GameRpg2D.Scripts.Core.Constants;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Core.Utils;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Animation;
using GameRpg2D.Scripts.ECS.Components.Combat;
using GameRpg2D.Scripts.ECS.Components.Facing;
using GameRpg2D.Scripts.ECS.Components.Inputs;
using GameRpg2D.Scripts.ECS.Components.Movement;
using GameRpg2D.Scripts.ECS.Components.Physics;
using GameRpg2D.Scripts.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Entities;

/// <summary>
/// Entidade base para todos os objetos do jogo (Jogadores, NPCs, etc.),
/// estendendo CharacterBody2D para aproveitar física e movimento nativo do Godot.
/// Obtém o World via nó autoload configurado no Godot (Project Settings > Autoload).
/// </summary>
public partial class BaseBody : CharacterBody2D
{
    // Resources
    protected AnimatedSprite2D _sprite;
    protected NavigationAgent2D NavigationAgent;

    #region Parameters
    /// <summary>
    /// Caminho para o nó EcsRunner registrado como singleton/autoload
    /// </summary>
    // ReSharper disable once InconsistentNaming
    [Export] protected Vector2I StartingGridPosition = Vector2I.Zero; // Posição inicial no grid
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
        
        NavigationAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");

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
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public List<Vector2I> GetPathTo(Vector2I targetGridPosition)
    {
        if (NavigationAgent == null)
        {
            GD.PrintErr("NavigationAgent2D não encontrado.");
            return [];
        }

        // Calcula o caminho usando o NavigationAgent2D
        var paths = NavigationAgent.GetCurrentNavigationPath();

        // Converte o caminho de Vector2 para Vector2I (grid)
        var gridPath = new List<Vector2I>();
        foreach (var point in paths)
        {
            gridPath.Add(PositionHelper.WorldToGrid(point));
        }

        return gridPath;
    }

    /// <summary>
    /// Subclasses devem implementar para registrar seus próprios components
    /// </summary>
    protected virtual void RegisterComponents()
    {
        // Adiciona componente de movimento com posição inicial
        AddMovementComponent(StartingGridPosition);

        // Adiciona componente de ataque
        AddAttackComponent();

        // Adiciona componente de colisão automaticamente
        AddCollisionComponent();

        // Adiciona componente de animação
        AddAnimationComponent();
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

    private void AddAnimationComponent()
    {
        InitializeSprite();

        if (_sprite != null)
        {
            AddComponent(new AnimationComponent
            {
                State = AnimationState.Idle,
                Direction = Direction.South,
                Sprite = _sprite,
                CurrentAnimation = DefaultAnimation,
                IsPlaying = true
            });
        }
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

    private void AddMovementComponent(Vector2I initialGridPosition)
    {
        var worldPosition = PositionHelper.GridToWorld(initialGridPosition);
        AddComponent(new TransformComponent(worldPosition, Scale, Rotation));
        AddComponent(new GridPositionComponent(initialGridPosition));
        AddComponent(new FacingComponent(Direction.South));
        
        AddComponent(new MovementComponent
        {
            Speed = MoveSpeed,
            FromGridPosition = initialGridPosition,
            ToGridPosition = initialGridPosition,
        });
        AddComponent(new MovementInputComponent());
    }

    private void AddAttackComponent()
    {
        // Componente de ataque
        AddComponent(new AttackComponent
        {
            AttackSpeed = AttackSpeed,
            AttackCooldown = AttackCooldown,
            BaseDamage = 10.0f,
            GridAttackRange = 1
        });

        AddComponent(new AttackStateComponent());
        AddComponent(new AttackInputComponent());
    }
}