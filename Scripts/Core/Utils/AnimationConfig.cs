using GameRpg2D.Scripts.Core.Constants;
using GameRpg2D.Scripts.Core.Enums;

namespace GameRpg2D.Scripts.Core.Utils;

/// <summary>
/// Configurações de timing para diferentes tipos de animação
/// </summary>
public static class AnimationConfig
{
    /// <summary>
    /// Obtém a duração ideal para uma animação baseada no seu estado e contexto
    /// </summary>
    /// <param name="state">Estado da animação</param>
    /// <param name="attackSpeed">Velocidade de ataque (apenas para AnimationState.Attack)</param>
    /// <param name="movementSpeed">Velocidade de movimento (apenas para AnimationState.Move)</param>
    /// <returns>Duração em segundos, ou null para usar velocidade padrão</returns>
    public static float? GetAnimationDuration(AnimationState state, float? attackSpeed = null, float? movementSpeed = null)
    {
        return state switch
        {
            // Animação de ataque deve durar exatamente o tempo do ataque
            AnimationState.Attack when attackSpeed.HasValue => attackSpeed.Value,

            // Animação de movimento deve sincronizar com o tempo de mover uma célula do grid
            AnimationState.Move when movementSpeed.HasValue => GameConstants.GRID_SIZE / movementSpeed.Value,

            // Animação idle usa velocidade padrão do SpriteFrames
            AnimationState.Idle => null,

            // Fallback para velocidade padrão
            _ => null
        };
    }

    /// <summary>
    /// Configurações específicas para diferentes tipos de animação
    /// </summary>
    public static class Timing
    {
        // Ataque: Sincroniza com AttackSpeed do componente
        public const float DEFAULT_ATTACK_DURATION = GameConstants.DEFAULT_ATTACK_SPEED;

        // Movimento: Calculado dinamicamente baseado na velocidade
        public static float GetMovementDuration(float speed) => GameConstants.GRID_SIZE / speed;

        // Idle: Usa velocidade padrão do SpriteFrames (sem override)
        public static readonly float? DEFAULT_IDLE_DURATION = null;
    }
}
