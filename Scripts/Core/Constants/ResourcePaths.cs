namespace GameRpg2D.Scripts.Core.Constants
{
    /// <summary>
    /// Constantes para caminhos de recursos do jogo
    /// </summary>
    public static class ResourcePaths
    {
        // Diretórios base
        public const string RESOURCES_DIR = "res://Resources/";
        public const string SCENES_DIR = "res://Scenes/";
        public const string SCRIPTS_DIR = "res://Scripts/";
        
        public const string SPRITES_DIR = RESOURCES_DIR + "Sprites/";
        
        // Sprites de personagens
        public static class Characters
        {
            public const string ARCHER_FEMALE = SPRITES_DIR + "Archer/Female/spriteframes.tres";
            public const string ARCHER_MALE = SPRITES_DIR + "Archer/Male/spriteframes.tres";
            public const string MAGE_FEMALE = SPRITES_DIR + "Mage/Female/spriteframes.tres";
            public const string MAGE_MALE = SPRITES_DIR + "Mage/Male/spriteframes.tres";
        }
        
        // Cenas principais
        public static class Scenes
        {
            public const string MAIN = SCENES_DIR + "main.tscn";
            public const string MENU = SCENES_DIR + "menu.tscn";
            public const string GAME = SCENES_DIR + "game.tscn";
        }
        
        // Configurações
        public static class Config
        {
            public const string GAME_SETTINGS = "user://game_settings.cfg";
            public const string SAVE_DATA = "user://save_data.dat";
        }
    }
}

