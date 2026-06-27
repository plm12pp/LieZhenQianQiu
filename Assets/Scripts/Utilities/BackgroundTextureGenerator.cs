using UnityEngine;

/// <summary>
/// 背景纹理生成器 - 生成战场背景和地形纹理
/// </summary>
public class BackgroundTextureGenerator
{
    /// <summary>
    /// 生成战场背景（草地/土地）
    /// </summary>
    public static Texture2D CreateBattlefieldBackground(int width, int height, int seed = 0)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Trilinear;
        Color[] colors = new Color[width * height];
        
        System.Random rng = new System.Random(seed);
        
        // 基础颜色 - 战场土地色
        Color baseColor = new Color(0.45f, 0.35f, 0.25f);
        Color grassColor = new Color(0.35f, 0.5f, 0.25f);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 基础颜色 + 噪声
                float noise1 = (float)rng.NextDouble() * 0.1f;
                float noise2 = (float)rng.NextDouble() * 0.05f;
                
                // 混合草地和土地
                float grassAmount = Mathf.PerlinNoise(x * 0.02f, y * 0.02f);
                
                Color pixelColor = Color.Lerp(baseColor, grassColor, grassAmount);
                pixelColor += new Color(noise1, noise1 * 0.8f, noise1 * 0.6f);
                
                // 添加一些深色斑点（泥土）
                if (noise2 > 0.08f)
                {
                    pixelColor *= 0.8f;
                }
                
                colors[y * width + x] = pixelColor;
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Repeat;
        
        return tex;
    }

    /// <summary>
    /// 生成主菜单背景
    /// </summary>
    public static Texture2D CreateMainMenuBackground(int width, int height)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] colors = new Color[width * height];
        
        // 渐变背景 - 从深蓝到深红
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / height;
            Color topColor = new Color(0.1f, 0.15f, 0.3f);
            Color bottomColor = new Color(0.3f, 0.1f, 0.1f);
            Color baseColor = Color.Lerp(topColor, bottomColor, t);
            
            for (int x = 0; x < width; x++)
            {
                // 添加云雾效果
                float cloud = Mathf.PerlinNoise(x * 0.01f + Time.time * 0.1f, y * 0.01f);
                cloud = (cloud - 0.5f) * 0.1f;
                
                colors[y * width + x] = baseColor + new Color(cloud, cloud, cloud);
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        
        return tex;
    }

    /// <summary>
    /// 生成地形纹理
    /// </summary>
    public static Texture2D CreateTerrainTexture(int width, int height, string terrainType)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] colors = new Color[width * height];
        
        System.Random rng = new System.Random(terrainType.GetHashCode());
        
        Color baseColor, detailColor;
        float detailScale = 0.05f;
        
        switch (terrainType.ToLower())
        {
            case "plains":
                baseColor = new Color(0.4f, 0.55f, 0.3f);
                detailColor = new Color(0.5f, 0.65f, 0.35f);
                break;
            case "desert":
                baseColor = new Color(0.76f, 0.7f, 0.5f);
                detailColor = new Color(0.85f, 0.75f, 0.55f);
                break;
            case "mountain":
                baseColor = new Color(0.4f, 0.35f, 0.3f);
                detailColor = new Color(0.5f, 0.45f, 0.4f);
                break;
            case "snow":
                baseColor = new Color(0.9f, 0.95f, 1f);
                detailColor = new Color(0.7f, 0.8f, 0.9f);
                break;
            case "forest":
                baseColor = new Color(0.2f, 0.4f, 0.2f);
                detailColor = new Color(0.15f, 0.35f, 0.15f);
                break;
            default:
                baseColor = new Color(0.5f, 0.5f, 0.4f);
                detailColor = new Color(0.6f, 0.6f, 0.5f);
                break;
        }
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noise = Mathf.PerlinNoise(x * detailScale, y * detailScale);
                Color pixelColor = Color.Lerp(baseColor, detailColor, noise);
                
                // 添加随机变化
                float randomNoise = (float)rng.NextDouble() * 0.1f;
                pixelColor += new Color(randomNoise, randomNoise, randomNoise);
                
                colors[y * width + x] = pixelColor;
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Repeat;
        
        return tex;
    }

    /// <summary>
    /// 生成UI背景装饰
    /// </summary>
    public static Texture2D CreateUIBackground(int width, int height, Color primaryColor, Color secondaryColor)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] colors = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 垂直渐变
                float t = (float)y / height;
                Color baseColor = Color.Lerp(primaryColor, secondaryColor, t);
                
                // 添加边框效果
                bool isBorder = x < 8 || x >= width - 8 || y < 8 || y >= height - 8;
                
                if (isBorder)
                {
                    colors[y * width + x] = new Color(secondaryColor.r * 0.6f, secondaryColor.g * 0.6f, secondaryColor.b * 0.6f);
                }
                else
                {
                    // 内区域带一点暗角
                    float cornerDist = Mathf.Max(
                        Mathf.Abs(x - width / 2f) / (width / 2f),
                        Mathf.Abs(y - height / 2f) / (height / 2f)
                    );
                    cornerDist = (cornerDist - 0.5f) * 0.3f;
                    
                    colors[y * width + x] = baseColor - new Color(cornerDist, cornerDist, cornerDist);
                }
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        
        return tex;
    }

    /// <summary>
    /// 生成地图瓦片
    /// </summary>
    public static Texture2D CreateMapTile(int width, int height, int tileType)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] colors = new Color[width * height];
        
        Color baseColor, lineColor;
        
        switch (tileType)
        {
            case 0: // 空地
                baseColor = new Color(0.4f, 0.45f, 0.35f);
                lineColor = new Color(0.3f, 0.35f, 0.25f);
                break;
            case 1: // 河流
                baseColor = new Color(0.3f, 0.5f, 0.7f);
                lineColor = new Color(0.2f, 0.4f, 0.6f);
                break;
            case 2: // 山地
                baseColor = new Color(0.5f, 0.45f, 0.4f);
                lineColor = new Color(0.4f, 0.35f, 0.3f);
                break;
            case 3: // 森林
                baseColor = new Color(0.25f, 0.45f, 0.25f);
                lineColor = new Color(0.2f, 0.35f, 0.2f);
                break;
            default:
                baseColor = Color.gray;
                lineColor = Color.darkGray;
                break;
        }
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isLine = x == 0 || y == 0;
                
                if (isLine)
                {
                    colors[y * width + x] = lineColor;
                }
                else
                {
                    colors[y * width + x] = baseColor;
                }
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        
        return tex;
    }
}
