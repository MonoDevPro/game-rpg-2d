namespace GameRpg2D.Scripts.Core.Constants;

/// <summary>
/// Constantes relacionadas ao movimento e grid do jogo
/// </summary>
public static class GameConstants
{
    // Grid
    public const int GRID_SIZE = 32; // Tamanho de cada célula do grid em pixels
    
    // Sistema de Movimento
    public const float DEFAULT_WALK_SPEED = 4.0f; // Velocidade de movimento padrão em pixels por segundo
    public const float INPUT_STOP_WALK_DELAY = 0.1f; // Delay antes de parar o movimento
    
    // Sistema de Ataque
    public const float DEFAULT_ATTACK_SPEED = 0.4f; // Velocidade de ataque padrão em segundos
    public const float DEFAULT_ATTACK_COOLDOWN = 1f; // Tempo de recarga do ataque em segundos
}