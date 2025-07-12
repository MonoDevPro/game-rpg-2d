using Arch.Core;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using GameRpg2D.Scripts.Constants;
using GameRpg2D.Scripts.Core.Enums;
using GameRpg2D.Scripts.Infrastructure;
using Godot;

namespace GameRpg2D.Scripts.ECS.Entities;

public partial class Player : CharacterBody2D
{
    // ECS
    private Entity _entity;
    private World _world;
        
    // Resources
    private AnimatedSprite2D _sprite;
    
    #region  Parameters
    [Export] public Vocation Vocation = Vocation.Mage;
    [Export] public Gender Gender = Gender.Male;
    [Export] public float MoveSpeed = GameConstants.DEFAULT_MOVE_SPEED;
    [Export] public Vector2I StartGridPosition = Vector2I.Zero;
    #endregion
        
    #region Validations 
    private bool ValidateParameters()
    {
        return true;
    }
        
    private bool ValidateResources()
    {
        // Verifica se o mundo ainda está ativo
        if (_world == null)
        {
            GD.PrintErr("O mundo ECS não está ativo.");
            return false;
        }
            
        // Verifica se o AnimatedSprite2D está definido
        if (_sprite == null)
        {
            GD.PrintErr("AnimatedSprite2D não está definido.");
            return false;
        }
            
        return true;
    }
    #endregion

    public override void _Ready()
    {
        // Validar parâmetros antes de continuar
        if (!ValidateParameters())
        {
            GD.PrintErr("Parâmetros inválidos.");
            return;
        }
            
        _world = GameManager.Instance.Ecs.World;
        _sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _sprite.SpriteFrames = AssetService.Instance.GetSpriteFrames(Vocation, Gender);
        _sprite.Play("idle_down"); // Iniciar com animação idle para baixo
            
        // Validar recursos antes de continuar
        if (!ValidateResources())
        {
            GD.PrintErr("Recursos inválidos.");
            return;
        }
            
        // Criar entidade ECS para o jogador LOCAL
        _entity = _world.Create(
            new NodeComponent(this),
            new PositionComponent(StartGridPosition),
            new AttackComponent(),
            new AttackConfigComponent(GameConstants.DEFAULT_ATTACK_DURATION, GameConstants.DEFAULT_ATTACK_COOLDOWN),
            new MovementComponent(),
            new MovementConfigComponent(MoveSpeed),
            new AnimationComponent(),
            new AnimatedSpriteComponent(_sprite),
            new LocalInputComponent(),
            new BufferedInputComponent(),
            new LocalPlayerTag()
        );

        // Posicionar o nó na posição inicial do grid
        Position = StartGridPosition * GameConstants.GRID_SIZE;
            
        // Verifica se a entidade foi criada corretamente
        if (!_world.IsAlive(_entity))
        {
            GD.PrintErr("O mundo ECS não está ativo ou a entidade foi destruída.");
        }
    }
        
    public override void _ExitTree()
    {
        // Limpar a entidade quando o nó for removido
        if (_world != null && _world.IsAlive(_entity))
        {
            _world.Destroy(_entity);
        }
    }

    public Entity GetEntity()
    {
        return _entity;
    }
}