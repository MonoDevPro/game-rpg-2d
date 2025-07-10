using System.Collections.Generic;
using GameRpg2D.Scripts.Constants;
using GameRpg2D.Scripts.Core.Enums;
using Godot;

namespace GameRpg2D.Scripts.Infrastructure;

public class AssetService
{
    private static AssetService _instance;
    public static AssetService Instance => _instance ??= new AssetService();

    private readonly Dictionary<string, Resource> _cache = new();

    // Construtor privado para singleton
    private AssetService() { }

    /// <summary>
    /// Carrega (e faz cache) qualquer Resource a partir de um path.
    /// </summary>
    public T Load<T>(string path) where T : Resource
    {
        // Validação de entrada
        if (string.IsNullOrEmpty(path))
        {
            GD.PrintErr($"AssetService: Path inválido ou vazio");
            return null;
        }

        // Verificar cache primeiro
        if (_cache.TryGetValue(path, out var cachedResource))
        {
            // Verificar se o recurso ainda é válido
            if (cachedResource != null && GodotObject.IsInstanceValid(cachedResource))
            {
                return (T)cachedResource;
            }
            else
            {
                // Remover recurso inválido do cache
                _cache.Remove(path);
            }
        }

        // Carregar recurso
        var loaded = GD.Load<T>(path);
        if (loaded != null)
        {
            _cache[path] = loaded;
            GD.Print($"AssetService: Loaded and cached '{path}'");
        }
        else
        {
            GD.PrintErr($"AssetService: Failed to load resource at '{path}'");
        }

        return loaded;
    }

    /// <summary>
    /// Retorna o SpriteFrames de acordo com a vocação e o gênero.
    /// </summary>
    public SpriteFrames GetSpriteFrames(Vocation vocation, Gender gender)
    {
        string path = (vocation, gender) switch
        {
            (Vocation.Mage, Gender.Male)   => ResourcePaths.Characters.MAGE_MALE,
            (Vocation.Mage, Gender.Female) => ResourcePaths.Characters.MAGE_FEMALE,
            (Vocation.Archer, Gender.Male)   => ResourcePaths.Characters.ARCHER_MALE,
            (Vocation.Archer, Gender.Female) => ResourcePaths.Characters.ARCHER_FEMALE,
            _ => throw new KeyNotFoundException(
                $"No SpriteFrames for vocation '{vocation}' with gender '{gender}'"
            )
        };

        return Load<SpriteFrames>(path);
    }

    /// <summary>
    /// Pré-carrega recursos específicos para melhor performance
    /// </summary>
    public void PreloadCharacterSprites()
    {
        // Pré-carregar todos os sprites de personagens
        foreach (var vocation in System.Enum.GetValues<Vocation>())
        {
            foreach (var gender in System.Enum.GetValues<Gender>())
            {
                try
                {
                    GetSpriteFrames(vocation, gender);
                }
                catch (KeyNotFoundException)
                {
                    // Ignora combinações não implementadas
                }
            }
        }
    }

    /// <summary>
    /// Limpa o cache de recursos
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
        GD.Print("AssetService: Cache cleared");
    }

    /// <summary>
    /// Retorna informações sobre o cache
    /// </summary>
    public void PrintCacheInfo()
    {
        GD.Print($"AssetService: Cache contains {_cache.Count} resources");
        foreach (var kvp in _cache)
        {
            string status = GodotObject.IsInstanceValid(kvp.Value) ? "Valid" : "Invalid";
            GD.Print($"  - {kvp.Key}: {status}");
        }
    }

    /// <summary>
    /// Verifica se um recurso está em cache
    /// </summary>
    public bool IsInCache(string path)
    {
        return _cache.ContainsKey(path) && GodotObject.IsInstanceValid(_cache[path]);
    }
}