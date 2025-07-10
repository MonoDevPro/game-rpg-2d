using Godot;
using GameRpg2D.Scripts.Constants;

namespace GameRpg2D.Scripts.Utilities
{
    /// <summary>
    /// Utilitários para determinação de direções e animações
    /// </summary>
    public static class DirectionUtils
    {
        /// <summary>
        /// Determina a direção baseada em prioridades: Input > Movimento > Última direção
        /// </summary>
        public static Vector2I DetermineDirection(Vector2I inputDirection, Vector2I movementDirection, Vector2I lastDirection)
        {
            if (inputDirection != Vector2I.Zero)
                return inputDirection;
            
            if (movementDirection != Vector2I.Zero)
                return movementDirection;
                
            return lastDirection != Vector2I.Zero ? lastDirection : GameConstants.Directions.DOWN;
        }

        /// <summary>
        /// Normaliza direção diagonal para direção cardinal (prioridade horizontal)
        /// </summary>
        public static Vector2I NormalizeDiagonalDirection(Vector2I direction)
        {
            // Para diagonais, usar a direção horizontal como prioridade
            if (direction.X != 0)
            {
                return direction.X > 0 ? GameConstants.Directions.RIGHT : GameConstants.Directions.LEFT;
            }
            
            // Fallback para vertical se não houver horizontal
            if (direction.Y != 0)
            {
                return direction.Y > 0 ? GameConstants.Directions.DOWN : GameConstants.Directions.UP;
            }
            
            return GameConstants.Directions.DOWN;
        }

        /// <summary>
        /// Obtém nome da animação baseado no tipo e direção
        /// </summary>
        public static string GetAnimationName(AnimationType type, Vector2I direction)
        {
            string prefix = type switch
            {
                AnimationType.Attack => "attack_",
                AnimationType.Move => "move_",
                AnimationType.Idle => "idle_",
                _ => "idle_"
            };

            string suffix = GetDirectionSuffix(direction);
            return prefix + suffix;
        }

        private static string GetDirectionSuffix(Vector2I direction)
        {
            // Tratar direções cardinais primeiro
            if (direction == GameConstants.Directions.UP)
                return "up";
            if (direction == GameConstants.Directions.DOWN)
                return "down";
            if (direction == GameConstants.Directions.LEFT)
                return "left";
            if (direction == GameConstants.Directions.RIGHT)
                return "right";
            
            // Para diagonais, normalizar primeiro
            var normalizedDirection = NormalizeDiagonalDirection(direction);
            return GetDirectionSuffix(normalizedDirection);
        }
    }

    public enum AnimationType
    {
        Attack,
        Move,
        Idle
    }
}
