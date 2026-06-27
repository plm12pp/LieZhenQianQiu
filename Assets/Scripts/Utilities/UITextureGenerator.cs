using UnityEngine;

/// <summary>
/// UI纹理生成器 - 程序化生成UI所需的各种纹理资源
/// 包括按钮背景、面板背景、图标等
/// </summary>
public class UITextureGenerator
{
    /// <summary>
    /// 生成渐变按钮背景
    /// </summary>
    public static Texture2D CreateButtonBackground(int width, int height, Color topColor, Color bottomColor, int radius = 8)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Trilinear;
        Color[] colors = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / height;
            Color rowColor = Color.Lerp(topColor, bottomColor, t);
            
            for (int x = 0; x < width; x++)
            {
                // 圆角矩形
                if (IsInsideRoundedRect(x, y, width, height, radius))
                {
                    colors[y * width + x] = rowColor;
                }
                else
                {
                    colors[y * width + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// 生成面板背景
    /// </summary>
    public static Texture2D CreatePanelBackground(int width, int height, Color fillColor, Color borderColor, int borderWidth = 4, int radius = 12)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Trilinear;
        Color[] colors = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isOuter = IsInsideRoundedRect(x, y, width, height, radius);
                bool isInner = IsInsideRoundedRect(x, y, width - borderWidth * 2, height - borderWidth * 2, radius - borderWidth);
                
                if (isOuter && !isInner)
                {
                    colors[y * width + x] = borderColor;
                }
                else if (isInner)
                {
                    colors[y * width + x] = fillColor;
                }
                else
                {
                    colors[y * width + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// 生成对话框背景
    /// </summary>
    public static Texture2D CreateDialogBackground(int width, int height, Color bgColor, Color borderColor)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Trilinear;
        Color[] colors = new Color[width * height];
        
        // 主背景
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 外边框
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    colors[y * width + x] = borderColor;
                }
                // 内边框装饰
                else if (x == 2 || x == width - 3 || y == 2 || y == height - 3)
                {
                    colors[y * width + x] = new Color(borderColor.r * 0.7f, borderColor.g * 0.7f, borderColor.b * 0.7f);
                }
                // 主区域
                else if (x >= 4 && x < width - 4 && y >= 4 && y < height - 4)
                {
                    // 渐变效果
                    float t = (float)y / height;
                    colors[y * width + x] = Color.Lerp(bgColor, new Color(bgColor.r * 0.9f, bgColor.g * 0.9f, bgColor.b * 0.9f), t);
                }
            }
        }
        
        // 角落装饰
        DrawCornerDecoration(colors, width, height, borderColor, 8);
        
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// 生成进度条背景
    /// </summary>
    public static Texture2D CreateProgressBarBackground(int width, int height, Color bgColor, Color borderColor)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] colors = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y == 0 || y == height - 1)
                {
                    colors[y * width + x] = borderColor;
                }
                else if (x == 0 || x == width - 1)
                {
                    colors[y * width + x] = borderColor;
                }
                else
                {
                    colors[y * width + x] = bgColor;
                }
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// 生成进度条填充
    /// </summary>
    public static Texture2D CreateProgressBarFill(int width, int height, Color fillColor)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] colors = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 渐变效果
                float t = (float)x / width;
                colors[y * width + x] = Color.Lerp(
                    new Color(fillColor.r * 0.7f, fillColor.g * 0.7f, fillColor.b * 0.7f),
                    fillColor,
                    t
                );
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// 生成货币图标
    /// </summary>
    public static Sprite CreateCurrencyIcon(string currencyType, int size = 64)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] colors = new Color[size * size];
        
        Color mainColor, accentColor;
        switch (currencyType.ToLower())
        {
            case "coin":
                mainColor = new Color(1f, 0.84f, 0f);  // 金色
                accentColor = new Color(0.8f, 0.6f, 0f);
                break;
            case "token":
                mainColor = new Color(0.75f, 0.75f, 0.75f);  // 银色
                accentColor = new Color(0.55f, 0.55f, 0.55f);
                break;
            case "jade":
                mainColor = new Color(0f, 1f, 0.5f);  // 翠绿色
                accentColor = new Color(0f, 0.8f, 0.4f);
                break;
            default:
                mainColor = Color.white;
                accentColor = Color.gray;
                break;
        }
        
        // 绘制圆形硬币
        int center = size / 2;
        int radius = size / 2 - 4;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                
                if (dist <= radius)
                {
                    // 外圈
                    if (dist > radius - 6)
                    {
                        colors[y * size + x] = accentColor;
                    }
                    else if (dist > radius - 10)
                    {
                        colors[y * size + x] = mainColor;
                    }
                    else
                    {
                        // 内圈渐变
                        float t = dist / (radius - 10);
                        colors[y * size + x] = Color.Lerp(mainColor, new Color(mainColor.r * 1.1f, mainColor.g * 1.1f, mainColor.b * 1.1f), t);
                    }
                    
                    // 中心图案
                    if (dist < size / 6)
                    {
                        colors[y * size + x] = accentColor;
                    }
                }
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    /// <summary>
    /// 生成星形图标（用于星级显示）
    /// </summary>
    public static Sprite CreateStarIcon(int size, Color color)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] colors = new Color[size * size];
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float outerRadius = size * 0.45f;
        float innerRadius = size * 0.2f;
        int points = 5;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 point = new Vector2(x, y);
                float angle = Mathf.Atan2(point.y - center.y, point.x - center.x);
                float dist = Vector2.Distance(point, center);
                
                float normalizedAngle = (angle + Mathf.PI / 2) % (Mathf.PI * 2);
                if (normalizedAngle < 0) normalizedAngle += Mathf.PI * 2;
                
                float segment = (Mathf.PI * 2) / points;
                float segmentPos = normalizedAngle % segment;
                float midAngle = segment / 2f;
                
                float radiusAtAngle;
                if (segmentPos <= midAngle)
                {
                    float t = segmentPos / midAngle;
                    radiusAtAngle = Mathf.Lerp(outerRadius, innerRadius, t);
                }
                else
                {
                    float t = (segmentPos - midAngle) / midAngle;
                    radiusAtAngle = Mathf.Lerp(innerRadius, outerRadius, t);
                }
                
                if (dist <= radiusAtAngle)
                {
                    float alpha = 1f - (dist / outerRadius) * 0.2f;
                    colors[y * size + x] = new Color(color.r, color.g, color.b, alpha);
                }
                else
                {
                    colors[y * size + x] = Color.clear;
                }
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    /// <summary>
    /// 生成战场网格纹理
    /// </summary>
    public static Texture2D CreateGridCellTexture(int size, Color fillColor, Color lineColor, float lineWidth = 2f)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] colors = new Color[size * size];
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // 边框
                if (x < lineWidth || x >= size - lineWidth || y < lineWidth || y >= size - lineWidth)
                {
                    colors[y * size + x] = lineColor;
                }
                else
                {
                    colors[y * size + x] = fillColor;
                }
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// 生成河流纹理
    /// </summary>
    public static Texture2D CreateRiverTexture(int width, int height)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] colors = new Color[width * height];
        
        System.Random rng = new System.Random(12345); // 固定种子保证一致性
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 基础蓝色
                float noise = (float)rng.NextDouble() * 0.1f;
                Color baseColor = new Color(0.4f + noise, 0.55f + noise, 0.7f + noise);
                
                // 波纹效果
                float wave = Mathf.Sin(x * 0.3f + y * 0.1f) * 0.05f;
                baseColor += new Color(wave, wave, wave);
                
                colors[y * width + x] = baseColor;
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    private static bool IsInsideRoundedRect(int x, int y, int width, int height, int radius)
    {
        if (x < radius && y < radius)
        {
            float dx = radius - x;
            float dy = radius - y;
            return dx * dx + dy * dy <= radius * radius;
        }
        if (x >= width - radius && y < radius)
        {
            float dx = x - (width - radius - 1);
            float dy = radius - y;
            return dx * dx + dy * dy <= radius * radius;
        }
        if (x < radius && y >= height - radius)
        {
            float dx = radius - x;
            float dy = y - (height - radius - 1);
            return dx * dx + dy * dy <= radius * radius;
        }
        if (x >= width - radius && y >= height - radius)
        {
            float dx = x - (width - radius - 1);
            float dy = y - (height - radius - 1);
            return dx * dx + dy * dy <= radius * radius;
        }
        return true;
    }

    private static void DrawCornerDecoration(Color[] colors, int width, int height, Color color, int size)
    {
        // 左上角
        for (int i = 0; i < size; i++)
        {
            if (i < height) colors[i * width + i] = color;
        }
        // 右上角
        for (int i = 0; i < size; i++)
        {
            int x = width - 1 - i;
            if (x >= 0 && i < height) colors[i * width + x] = color;
        }
        // 左下角
        for (int i = 0; i < size; i++)
        {
            int y = height - 1 - i;
            if (y >= 0 && i < width) colors[y * width + i] = color;
        }
        // 右下角
        for (int i = 0; i < size; i++)
        {
            int y = height - 1 - i;
            int x = width - 1 - i;
            if (y >= 0 && x >= 0) colors[y * width + x] = color;
        }
    }
}
