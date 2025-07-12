using Arch.Core;
using GameRpg2D.Scripts.Constants;
using GameRpg2D.Scripts.ECS.Components;
using GameRpg2D.Scripts.ECS.Components.Tags;
using Godot;

namespace GameRpg2D.Scripts
{
    public partial class CameraController : Camera2D
    {
        [Export] public float FollowSpeed = GameConstants.CAMERA_FOLLOW_SPEED;
        [Export] public bool SmoothFollow = true;
        
        private World _world;
        private QueryDescription _playerQuery;

        public override void _Ready()
        {
            // Aguardar o GameManager estar pronto
            CallDeferred(nameof(InitializeCamera));
        }

        private void InitializeCamera()
        {
            if (GameManager.Instance.Ecs.World != null)
            {
                _world = GameManager.Instance.Ecs.World;
                
                // Query para encontrar entidades com InputComponent (assumindo que apenas o jogador tem)
                _playerQuery = new QueryDescription()
                    .WithAll<LocalPlayerTag, PositionComponent, LocalInputComponent>();
                
                // Configurar câmera
                Enabled = true;
                
                GD.Print("CameraController inicializada");
            }
            else
            {
                // Tentar novamente no próximo frame se o GameManager não estiver pronto
                CallDeferred(nameof(InitializeCamera));
            }
        }

        public override void _Process(double delta)
        {
            if (_world == null) return;
            
            // Encontrar a posição do jogador
            _world.Query(in _playerQuery, (ref PositionComponent position) =>
            {
                Vector2 targetPosition = new(position.X, position.Y);
                
                if (SmoothFollow)
                {
                    // Seguir suavemente o jogador
                    GlobalPosition = GlobalPosition.Lerp(targetPosition, FollowSpeed * (float)delta);
                }
                else
                {
                    // Seguir diretamente
                    GlobalPosition = targetPosition;
                }
            });
        }
    }
}
