using Godot;

namespace GameRpg2D.Scripts.Constants;

/// <summary>
/// Constantes relacionadas ao movimento e grid do jogo
/// </summary>
public static class GameConstants
{
    // Grid e Movimento
    public const int GRID_SIZE = 32; // Tamanho de cada célula do grid em pixels
    public const float DEFAULT_MOVE_SPEED = 4.0f; // Velocidade de movimento padrão em pixels por segundo
    public const float CAMERA_FOLLOW_SPEED = 5.0f; // Velocidade de seguimento da câmera em pixels por segundo
    public const float INPUT_STOP_DELAY = 0.1f; // Delay antes de parar o movimento
    
    // Sistema de Ataque
    public const float DEFAULT_ATTACK_DURATION = 0.4f; // Duração do ataque em segundos
    public const float DEFAULT_ATTACK_COOLDOWN = 0.2f; // Tempo de recarga do ataque em segundos
    public const float DEFAULT_ATTACK_RANGE = 1.0f; // Alcance do ataque em células do grid
    public const float DEFAULT_ANIMATION_SPEED_SCALE = 1.0f; // Escala de velocidade da animação padrão
    
    public const float DEFAULT_ACTION_COOLDOWN = 2f; // Tempo de recarga para ações gerais (como interações)
}