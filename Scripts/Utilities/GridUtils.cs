using GameRpg2D.Scripts.Constants;
using Godot;

namespace GameRpg2D.Scripts.Utilities
{
    /// <summary>
    /// Utilitários para cálculos relacionados ao grid
    /// </summary>
    public static class GridUtils
    {
        /// <summary>
        /// Converte posição do mundo para posição no grid
        /// </summary>
        public static Vector2I WorldToGrid(Vector2 worldPosition)
        {
            return new Vector2I(
                Mathf.FloorToInt(worldPosition.X / GameConstants.GRID_SIZE),
                Mathf.FloorToInt(worldPosition.Y / GameConstants.GRID_SIZE)
            );
        }
        
        /// <summary>
        /// Converte posição do grid para posição no mundo
        /// </summary>
        public static Vector2 GridToWorld(Vector2I gridPosition)
        {
            return new Vector2(
                gridPosition.X * GameConstants.GRID_SIZE,
                gridPosition.Y * GameConstants.GRID_SIZE
            );
        }
        
        /// <summary>
        /// Centraliza a posição no grid (adiciona metade do tamanho do tile)
        /// </summary>
        public static Vector2 CenterInGrid(Vector2I gridPosition)
        {
            return GridToWorld(gridPosition) + Vector2.One * (GameConstants.GRID_SIZE / 2.0f);
        }
        
        /// <summary>
        /// Calcula distância em grid entre duas posições
        /// </summary>
        public static int GridDistance(Vector2I from, Vector2I to)
        {
            return Mathf.Abs(to.X - from.X) + Mathf.Abs(to.Y - from.Y);
        }
        
        /// <summary>
        /// Verifica se uma posição no grid é válida (pode ser expandido com lógica de colisão)
        /// </summary>
        public static bool IsValidGridPosition(Vector2I gridPosition)
        {
            // Por enquanto, apenas verifica limites básicos
            // Pode ser expandido para verificar colisões, limites do mapa, etc.
            return gridPosition.X >= -100 && gridPosition.X <= 100 &&
                   gridPosition.Y >= -100 && gridPosition.Y <= 100;
        }
    }
}
