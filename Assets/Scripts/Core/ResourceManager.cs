using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 资源管理器 - 统一管理所有程序化生成的资源
/// 在游戏启动时生成并缓存所有需要的纹理和精灵
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    // 缓存的纹理
    private Dictionary<string, Texture2D> textureCache;
    private Dictionary<string, Sprite> spriteCache;
    private Dictionary<string, Material> materialCache;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            textureCache = new Dictionary<string, Texture2D>();
            spriteCache = new Dictionary<string, Sprite>();
            materialCache = new Dictionary<string, Material>();
            
            // 在编辑器模式下预生成资源
            #if UNITY_EDITOR
            GenerateAllResources();
            #endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 生成所有程序化资源
    /// </summary>
    public void GenerateAllResources()
    {
        Debug.Log("ResourceManager: 开始生成程序化资源...");
        
        GenerateTroopIcons();
        GenerateUIResources();
        GenerateBattlefieldResources();
        
        Debug.Log($"ResourceManager: 资源生成完成！纹理:{textureCache.Count} 精灵:{spriteCache.Count}");
    }

    /// <summary>
    /// 生成所有兵种图标
    /// </summary>
    void GenerateTroopIcons()
    {
        if (GameManager.instance == null || GameManager.instance.troopsData == null)
            return;

        foreach (var troop in GameManager.instance.troopsData.troops)
        {
            // 为每个兵种生成多个尺寸的图标
            int[] sizes = { 64, 128, 256 };
            foreach (int size in sizes)
            {
                string key = $"troop_icon_{troop.id}_{size}";
                Sprite sprite = TroopIconGenerator.GenerateTroopIcon(troop.id, size);
                if (sprite != null)
                {
                    spriteCache[key] = sprite;
                }
            }
        }
    }

    /// <summary>
    /// 生成UI资源
    /// </summary>
    void GenerateUIResources()
    {
        // 生成货币图标
        string[] currencies = { "coin", "token", "jade" };
        foreach (string curr in currencies)
        {
            Sprite icon = UITextureGenerator.CreateCurrencyIcon(curr, 64);
            spriteCache[$"currency_{curr}"] = icon;
        }

        // 生成星形图标
        Color[] starColors = { Color.yellow, new Color(1f, 0.8f, 0.2f), new Color(0.9f, 0.7f, 0.1f) };
        for (int i = 0; i < starColors.Length; i++)
        {
            Sprite star = UITextureGenerator.CreateStarIcon(32, starColors[i]);
            spriteCache[$"star_{i}"] = star;
        }

        // 生成按钮背景
        Texture2D normalBtn = UITextureGenerator.CreateButtonBackground(
            200, 60, 
            new Color(0.7f, 0.7f, 0.8f), 
            new Color(0.5f, 0.5f, 0.6f), 
            10
        );
        textureCache["btn_normal"] = normalBtn;

        Texture2D highlightedBtn = UITextureGenerator.CreateButtonBackground(
            200, 60,
            new Color(0.85f, 0.85f, 0.95f),
            new Color(0.65f, 0.65f, 0.75f),
            10
        );
        textureCache["btn_highlighted"] = highlightedBtn;

        // 生成面板背景
        Texture2D panelBg = UITextureGenerator.CreatePanelBackground(
            400, 300,
            new Color(0.25f, 0.25f, 0.3f, 0.9f),
            new Color(0.4f, 0.35f, 0.25f),
            4, 15
        );
        textureCache["panel_background"] = panelBg;

        // 生成对话框
        Texture2D dialogBg = UITextureGenerator.CreateDialogBackground(
            500, 350,
            new Color(0.2f, 0.18f, 0.15f),
            new Color(0.75f, 0.6f, 0.3f)
        );
        textureCache["dialog_background"] = dialogBg;

        // 生成进度条
        Texture2D progressBg = UITextureGenerator.CreateProgressBarBackground(
            200, 20,
            new Color(0.3f, 0.3f, 0.3f),
            new Color(0.5f, 0.4f, 0.3f)
        );
        textureCache["progress_background"] = progressBg;

        Texture2D progressFill = UITextureGenerator.CreateProgressBarFill(
            200, 20,
            new Color(0.8f, 0.5f, 0.2f)
        );
        textureCache["progress_fill"] = progressFill;
    }

    /// <summary>
    /// 生成战场资源
    /// </summary>
    void GenerateBattlefieldResources()
    {
        // 生成战场背景
        Texture2D battlefieldBg = BackgroundTextureGenerator.CreateBattlefieldBackground(1024, 1024, 12345);
        textureCache["battlefield_ground"] = battlefieldBg;

        // 生成地形纹理
        string[] terrains = { "plains", "desert", "mountain", "snow", "forest" };
        foreach (string terrain in terrains)
        {
            Texture2D terrainTex = BackgroundTextureGenerator.CreateTerrainTexture(256, 256, terrain);
            textureCache[$"terrain_{terrain}"] = terrainTex;
        }

        // 生成河流纹理
        Texture2D riverTex = UITextureGenerator.CreateRiverTexture(256, 64);
        textureCache["river"] = riverTex;

        // 生成网格纹理
        Texture2D gridTex = UITextureGenerator.CreateGridCellTexture(
            64, 64,
            new Color(0.5f, 0.5f, 0.45f, 0.3f),
            new Color(0.4f, 0.35f, 0.25f)
        );
        textureCache["grid_cell"] = gridTex;
    }

    /// <summary>
    /// 获取缓存的精灵
    /// </summary>
    public Sprite GetSprite(string key)
    {
        if (spriteCache.ContainsKey(key))
            return spriteCache[key];
        
        Debug.LogWarning($"ResourceManager: 找不到精灵 '{key}'");
        return null;
    }

    /// <summary>
    /// 获取缓存的纹理
    /// </summary>
    public Texture2D GetTexture(string key)
    {
        if (textureCache.ContainsKey(key))
            return textureCache[key];
        
        Debug.LogWarning($"ResourceManager: 找不到纹理 '{key}'");
        return null;
    }

    /// <summary>
    /// 获取兵种图标
    /// </summary>
    public Sprite GetTroopIcon(string troopId, int size = 128)
    {
        string key = $"troop_icon_{troopId}_{size}";
        if (spriteCache.ContainsKey(key))
            return spriteCache[key];
        
        // 如果不存在，动态生成
        Sprite sprite = TroopIconGenerator.GenerateTroopIcon(troopId, size);
        if (sprite != null)
        {
            spriteCache[key] = sprite;
        }
        return sprite;
    }

    /// <summary>
    /// 创建精灵材质
    /// </summary>
    public Material GetSpriteMaterial(string textureKey, Color tint)
    {
        string matKey = $"{textureKey}_{tint}";
        
        if (materialCache.ContainsKey(matKey))
            return materialCache[matKey];
        
        Texture2D tex = GetTexture(textureKey);
        if (tex == null) return null;
        
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.mainTexture = tex;
        mat.color = tint;
        
        materialCache[matKey] = mat;
        return mat;
    }

    /// <summary>
    /// 清理缓存（节省内存时调用）
    /// </summary>
    public void ClearCache()
    {
        foreach (var tex in textureCache.Values)
        {
            if (tex != null) Destroy(tex);
        }
        foreach (var sprite in spriteCache.Values)
        {
            if (sprite != null && sprite.texture != null) Destroy(sprite.texture);
        }
        foreach (var mat in materialCache.Values)
        {
            if (mat != null) Destroy(mat);
        }
        
        textureCache.Clear();
        spriteCache.Clear();
        materialCache.Clear();
        
        Debug.Log("ResourceManager: 缓存已清理");
    }
}
