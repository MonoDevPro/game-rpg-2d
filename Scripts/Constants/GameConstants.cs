using Godot;

namespace GameRpg2D.Scripts.Constants;

/// <summary>
/// Constantes relacionadas ao movimento e grid do jogo
/// </summary>
public static class GameConstants
{
    // Grid e Movimento
    public const int GRID_SIZE = 32;
    public const float DEFAULT_MOVE_SPEED = 4.0f;
    public const float CAMERA_FOLLOW_SPEED = 5.0f;
    public const float INPUT_STOP_DELAY = 0.1f; // Delay antes de parar o movimento
    
    // Sistema de Ataque
    public const float DEFAULT_ATTACK_DURATION = 0.4f;
    public const float DEFAULT_ATTACK_COOLDOWN = 0.2f;
    public const float DEFAULT_ATTACK_RANGE = 1.0f;
    public const float DEFAULT_COMBO_WINDOW = 0.8f;
    public const int MAX_COMBO_COUNT = 3; // Eliminar magic number
    public const float DEFAULT_ANIMATION_SPEED_SCALE = 1.0f;
        
    // Direções (para facilitar cálculos)
    public static class Directions
    {
        public static readonly Vector2I UP = new(0, -1);
        public static readonly Vector2I DOWN = new(0, 1);
        public static readonly Vector2I LEFT = new(-1, 0);
        public static readonly Vector2I RIGHT = new(1, 0);
            
        public static readonly Vector2I UP_LEFT = new(-1, -1);
        public static readonly Vector2I UP_RIGHT = new(1, -1);
        public static readonly Vector2I DOWN_LEFT = new(-1, 1);
        public static readonly Vector2I DOWN_RIGHT = new(1, 1);
    }
}