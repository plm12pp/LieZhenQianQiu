#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Unity项目初始化工具
/// 在Unity编辑器中运行此脚本生成必要的场景和预设
/// </summary>
public class ProjectInitializer : EditorWindow
{
    [MenuItem("列阵千秋/初始化项目")]
    public static void InitializeProject()
    {
        Debug.Log("开始初始化列阵千秋项目...");

        // 创建必要的文件夹
        CreateDirectories();

        // 创建主菜单场景
        CreateMainMenuScene();

        // 创建战斗场景
        CreateBattleScene();

        // 创建预设
        CreatePrefabs();

        // 生成程序化资源
        GenerateResources();

        Debug.Log("项目初始化完成！");
    }

    static void CreateDirectories()
    {
        string[] dirs = {
            "Assets/Scenes",
            "Assets/Prefabs",
            "Assets/Materials",
            "Assets/Sprites",
            "Assets/Audio"
        };

        foreach (string dir in dirs)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Debug.Log($"创建目录: {dir}");
            }
        }
    }

    [MenuItem("列阵千秋/生成兵种图标")]
    public static void GenerateTroopIcons()
    {
        Debug.Log("开始生成兵种图标...");

        if (GameManager.instance == null || GameManager.instance.troopsData == null)
        {
            Debug.LogError("请先确保GameManager已加载兵种数据！");
            return;
        }

        string outputDir = "Assets/Sprites/TroopIcons";
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        foreach (var troop in GameManager.instance.troopsData.troops)
        {
            Sprite sprite = TroopIconGenerator.GenerateTroopIcon(troop.id, 128);

            // 创建纹理
            Texture2D tex = sprite.texture;
            byte[] bytes = tex.EncodeToPNG();
            string path = $"{outputDir}/{troop.id}.png";
            File.WriteAllBytes(path, bytes);
            Debug.Log($"生成图标: {troop.name}");
        }

        AssetDatabase.Refresh();
        Debug.Log("兵种图标生成完成！");
    }

    [MenuItem("列阵千秋/生成UI纹理")]
    public static void GenerateUITextures()
    {
        Debug.Log("开始生成UI纹理...");

        string outputDir = "Assets/Sprites/UI";
        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        // 生成按钮纹理
        Texture2D normalBtn = UITextureGenerator.CreateButtonBackground(
            200, 60, new Color(0.7f, 0.7f, 0.8f), new Color(0.5f, 0.5f, 0.6f), 10);
        File.WriteAllBytes($"{outputDir}/button_normal.png", normalBtn.EncodeToPNG());

        // 生成面板纹理
        Texture2D panelBg = UITextureGenerator.CreatePanelBackground(
            400, 300, new Color(0.25f, 0.25f, 0.3f), new Color(0.4f, 0.35f, 0.25f), 4, 15);
        File.WriteAllBytes($"{outputDir}/panel_background.png", panelBg.EncodeToPNG());

        // 生成货币图标
        Sprite coin = UITextureGenerator.CreateCurrencyIcon("coin", 64);
        File.WriteAllBytes($"{outputDir}/icon_coin.png", coin.texture.EncodeToPNG());

        AssetDatabase.Refresh();
        Debug.Log("UI纹理生成完成！");
    }

    [MenuItem("列阵千秋/生成所有资源")]
    public static void GenerateAllResources()
    {
        GenerateTroopIcons();
        GenerateUITextures();
        Debug.Log("所有资源生成完成！");
    }

    static void CreateMainMenuScene()
    {
        string scenePath = "Assets/Scenes/MainMenu.unity";

        if (File.Exists(scenePath))
        {
            Debug.Log("主菜单场景已存在，跳过创建");
            return;
        }

        // 创建场景
        UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);

        // 创建Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // 创建主菜单管理器
        GameObject menuManager = new GameObject("MainMenuManager");
        menuManager.AddComponent<UIManager>();
        menuManager.AddComponent<ResourceManager>();

        // 保存场景
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene(), scenePath);

        Debug.Log($"创建主菜单场景: {scenePath}");
    }

    static void CreateBattleScene()
    {
        string scenePath = "Assets/Scenes/Battle.unity";

        if (File.Exists(scenePath))
        {
            Debug.Log("战斗场景已存在，跳过创建");
            return;
        }

        // 创建场景
        UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects);

        // 创建Camera
        Camera.main.gameObject.tag = "MainCamera";

        // 创建战场管理器
        GameObject battleManager = new GameObject("BattleManager");
        battleManager.AddComponent<BattleManager>();
        battleManager.AddComponent<BattleGrid>();
        battleManager.AddComponent<FormationManager>();
        battleManager.AddComponent<AIBattleController>();
        battleManager.AddComponent<BattleEffectsManager>();
        battleManager.AddComponent<BattleLogManager>();

        // 创建UI管理器
        GameObject uiManager = new GameObject("UIManager");
        uiManager.AddComponent<UIManager>();

        // 创建货币管理器
        GameObject currencyManager = new GameObject("CurrencyManager");
        currencyManager.AddComponent<CurrencyManager>();

        // 创建音效管理器
        GameObject soundManager = new GameObject("SoundManager");
        soundManager.AddComponent<SoundManager>();

        // 创建画布
        GameObject canvasObj = new GameObject("BattleCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // 保存场景
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene(), scenePath);

        Debug.Log($"创建战斗场景: {scenePath}");
    }

    static void CreatePrefabs()
    {
        string prefabDir = "Assets/Prefabs";
        if (!Directory.Exists(prefabDir))
            Directory.CreateDirectory(prefabDir);

        // 创建兵种预设
        GameObject troopPrefab = CreateSimplePrefab("TroopUnit");
        PrefabUtility.SaveAsPrefabAsset(troopPrefab, $"{prefabDir}/TroopUnit.prefab");
        Debug.Log("创建兵种预设");

        // 创建UI预设
        GameObject buttonPrefab = CreateSimplePrefab("UIButton");
        PrefabUtility.SaveAsPrefabAsset(buttonPrefab, $"{prefabDir}/UIButton.prefab");
        Debug.Log("创建按钮预设");

        DestroyImmediate(troopPrefab);
        DestroyImmediate(buttonPrefab);
    }

    static GameObject CreateSimplePrefab(string name)
    {
        GameObject obj = new GameObject(name);
        obj.AddComponent<SpriteRenderer>();
        return obj;
    }
}
#endif
