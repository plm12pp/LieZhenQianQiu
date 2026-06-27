using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 兵种图标生成器 - 使用代码生成程序化兵种图标
/// 支持步兵、骑兵、弓兵、器械等不同类型兵种的图标生成
/// </summary>
public class TroopIconGenerator
{
    // 兵种类型对应的颜色方案
    private static readonly Dictionary<string, Color[]> TroopColors = new Dictionary<string, Color[]>
    {
        // 近战步兵 - 红色系
        { "infantry", new Color[] {
            new Color(0.8f, 0.2f, 0.2f),
            new Color(0.6f, 0.15f, 0.15f),
            new Color(0.9f, 0.3f, 0.3f)
        }},
        // 骑兵 - 棕色系
        { "cavalry", new Color[] {
            new Color(0.6f, 0.4f, 0.2f),
            new Color(0.4f, 0.3f, 0.15f),
            new Color(0.7f, 0.5f, 0.25f)
        }},
        // 弓兵 - 绿色系
        { "archer", new Color[] {
            new Color(0.2f, 0.6f, 0.3f),
            new Color(0.15f, 0.4f, 0.2f),
            new Color(0.3f, 0.7f, 0.4f)
        }},
        // 器械 - 灰色系
        { "siege", new Color[] {
            new Color(0.5f, 0.5f, 0.5f),
            new Color(0.35f, 0.35f, 0.35f),
            new Color(0.6f, 0.6f, 0.6f)
        }},
        // 水军 - 蓝色系
        { "naval", new Color[] {
            new Color(0.2f, 0.4f, 0.8f),
            new Color(0.15f, 0.3f, 0.6f),
            new Color(0.3f, 0.5f, 0.9f)
        }},
        // 精英部队 - 紫色系
        { "elite", new Color[] {
            new Color(0.6f, 0.3f, 0.8f),
            new Color(0.4f, 0.2f, 0.6f),
            new Color(0.7f, 0.4f, 0.9f)
        }},
        // 传说部队 - 金色系
        { "legendary", new Color[] {
            new Color(0.9f, 0.7f, 0.2f),
            new Color(0.7f, 0.5f, 0.1f),
            new Color(1.0f, 0.85f, 0.3f)
        }}
    };

    /// <summary>
    /// 根据兵种类型生成图标
    /// </summary>
    public static Sprite GenerateTroopIcon(string troopId, int size = 128)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Trilinear;
        Color[] colors = new Color[size * size];
        
        // 确定兵种类型
        string type = GetTroopType(troopId);
        Color[] scheme = GetTroopColorScheme(type);
        
        // 生成背景
        GenerateBackground(colors, size, scheme[0]);
        
        // 根据类型生成不同形状的图标
        switch (type)
        {
            case "infantry":
                GenerateInfantryIcon(colors, size, scheme);
                break;
            case "cavalry":
                GenerateCavalryIcon(colors, size, scheme);
                break;
            case "archer":
                GenerateArcherIcon(colors, size, scheme);
                break;
            case "siege":
                GenerateSiegeIcon(colors, size, scheme);
                break;
            case "naval":
                GenerateNavalIcon(colors, size, scheme);
                break;
            default:
                GenerateInfantryIcon(colors, size, scheme);
                break;
        }
        
        // 添加边框和高光
        AddBorderAndHighlight(colors, size, scheme[2]);
        
        tex.SetPixels(colors);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size / 2f);
    }

    private static string GetTroopType(string troopId)
    {
        if (troopId.Contains("cavalry") || troopId.Contains("qi") || troopId.Contains("xuanjia") ||
            troopId.Contains("iron_buddha") || troopId.Contains("tiger") || troopId.Contains("yuan"))
            return "cavalry";
        if (troopId.Contains("archer") || troopId.Contains("nu") || troopId.Contains("gong"))
            return "archer";
        if (troopId.Contains("catapult") || troopId.Contains("cannon") || troopId.Contains("tiger_cannon"))
            return "siege";
        if (troopId.Contains("ship") || troopId.Contains("mengchong") || troopId.Contains("naval") || 
            troopId.Contains("water") || troopId.Contains("liangjun"))
            return "naval";
        if (troopId.Contains("elite") || troopId.Contains("legendary") || troopId.Contains("qin_ruishi") ||
            troopId.Contains("hujun") || troopId.Contains("虎贲"))
            return "legendary";
        return "infantry";
    }

    private static Color[] GetTroopColorScheme(string type)
    {
        if (TroopColors.ContainsKey(type))
            return TroopColors[type];
        return TroopColors["infantry"];
    }

    private static void GenerateBackground(Color[] colors, int size, Color bgColor)
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // 圆形背景
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(size / 2f, size / 2f));
                float maxDist = size / 2f;
                
                if (dist <= maxDist * 0.9f)
                {
                    float alpha = 1f - (dist / maxDist) * 0.3f;
                    colors[y * size + x] = new Color(bgColor.r, bgColor.g, bgColor.b, alpha);
                }
                else
                {
                    colors[y * size + x] = Color.clear;
                }
            }
        }
    }

    private static void GenerateInfantryIcon(Color[] colors, int size, Color[] scheme)
    {
        int centerX = size / 2;
        int centerY = size / 2;
        int bodyRadius = size / 5;
        int headRadius = size / 10;
        
        // 绘制身体
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY - size/10));
                if (dist <= bodyRadius)
                {
                    colors[y * size + x] = scheme[0];
                }
            }
        }
        
        // 绘制头部
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY - size/3));
                if (dist <= headRadius)
                {
                    colors[y * size + x] = scheme[2];
                }
            }
        }
        
        // 绘制武器（剑/戈）
        DrawLine(colors, size, centerX + bodyRadius - 5, centerY - size/10, centerX + bodyRadius + 15, centerY - size/3, scheme[2], 3);
    }

    private static void GenerateCavalryIcon(Color[] colors, int size, Color[] scheme)
    {
        int centerX = size / 2;
        int centerY = size / 2;
        
        // 绘制马身
        DrawEllipse(colors, size, centerX, centerY, size/4, size/6, scheme[0]);
        
        // 绘制马头
        DrawEllipse(colors, size, centerX + size/5, centerY + size/8, size/8, size/10, scheme[0]);
        
        // 绘制骑手
        int riderX = centerX - size/10;
        int riderY = centerY - size/10;
        DrawCircle(colors, size, riderX, riderY, size/12, scheme[2]);
        
        // 绘制长枪
        DrawLine(colors, size, centerX - size/4, centerY - size/10, centerX + size/3, centerY - size/5, scheme[2], 2);
    }

    private static void GenerateArcherIcon(Color[] colors, int size, Color[] scheme)
    {
        int centerX = size / 2;
        int centerY = size / 2;
        
        // 绘制弓
        DrawArc(colors, size, centerX - size/8, centerY, size/4, 180f, scheme[2], 4);
        
        // 绘制弓弦
        DrawLine(colors, size, centerX - size/8, centerY - size/4, centerX - size/8, centerY + size/4, scheme[2], 1);
        
        // 绘制箭
        DrawLine(colors, size, centerX - size/8, centerY, centerX + size/3, centerY, scheme[2], 2);
        
        // 绘制箭羽
        DrawTriangle(colors, size, centerX + size/3, centerY, size/12, scheme[1]);
    }

    private static void GenerateSiegeIcon(Color[] colors, int size, Color[] scheme)
    {
        int centerX = size / 2;
        int centerY = size / 2;
        
        // 绘制投石臂
        DrawLine(colors, size, centerX, centerY + size/6, centerX + size/3, centerY - size/6, scheme[0], 6);
        
        // 绘制投石筐
        DrawCircle(colors, size, centerX + size/3, centerY - size/6, size/10, scheme[2]);
        
        // 绘制底座
        DrawRectangle(colors, size, centerX - size/4, centerY + size/6, size/2, size/8, scheme[1]);
        
        // 绘制轮子
        DrawCircle(colors, size, centerX - size/4, centerY + size/3, size/8, scheme[2]);
        DrawCircle(colors, size, centerX + size/4, centerY + size/3, size/8, scheme[2]);
    }

    private static void GenerateNavalIcon(Color[] colors, int size, Color[] scheme)
    {
        int centerX = size / 2;
        int centerY = size / 2;
        
        // 绘制船身
        DrawShipShape(colors, size, centerX, centerY, size/2, size/4, scheme[0]);
        
        // 绘制桅杆
        DrawLine(colors, size, centerX, centerY, centerX, centerY - size/3, scheme[2], 3);
        
        // 绘制船帆
        DrawTriangle(colors, size, centerX, centerY - size/3, size/6, scheme[2]);
        
        // 绘制船桨
        DrawLine(colors, size, centerX - size/4, centerY + size/8, centerX - size/3, centerY + size/3, scheme[1], 2);
        DrawLine(colors, size, centerX + size/4, centerY + size/8, centerX + size/3, centerY + size/3, scheme[1], 2);
    }

    private static void AddBorderAndHighlight(Color[] colors, int size, Color highlightColor)
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(size / 2f, size / 2f));
                float maxDist = size / 2f;
                
                // 边框
                if (dist > maxDist * 0.85f && dist <= maxDist * 0.9f)
                {
                    if (colors[y * size + x].a > 0.5f)
                    {
                        colors[y * size + x] = highlightColor;
                    }
                }
                
                // 顶部高光
                if (y < size / 3 && x > size / 4 && x < size * 3 / 4)
                {
                    if (colors[y * size + x].a > 0.3f)
                    {
                        Color c = colors[y * size + x];
                        colors[y * size + x] = new Color(c.r + 0.2f, c.g + 0.2f, c.b + 0.2f, c.a);
                    }
                }
            }
        }
    }

    // 辅助绘图函数
    private static void DrawCircle(Color[] colors, int size, int cx, int cy, int radius, Color color)
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                if (dist <= radius)
                {
                    colors[y * size + x] = color;
                }
            }
        }
    }

    private static void DrawEllipse(Color[] colors, int size, int cx, int cy, int radiusX, int radiusY, Color color)
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x - cx) / (float)radiusX;
                float dy = (y - cy) / (float)radiusY;
                if (dx * dx + dy * dy <= 1f)
                {
                    colors[y * size + x] = color;
                }
            }
        }
    }

    private static void DrawLine(Color[] colors, int size, int x1, int y1, int x2, int y2, Color color, int thickness)
    {
        int halfThick = thickness / 2;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (x >= x1 - halfThick && x <= x2 + halfThick && y >= y1 - halfThick && y <= y2 + halfThick)
                {
                    float t = Mathf.InverseLerp(x1, x2, x);
                    int lineY = Mathf.RoundToInt(Mathf.Lerp(y1, y2, t));
                    if (Mathf.Abs(y - lineY) <= halfThick && x >= x1 && x <= x2)
                    {
                        colors[y * size + x] = color;
                    }
                }
            }
        }
    }

    private static void DrawTriangle(Color[] colors, int size, int cx, int cy, int halfSize, Color color)
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int dy = cy - y;
                int dx = Mathf.Abs(x - cx);
                if (dy >= 0 && dy <= halfSize && dx <= dy)
                {
                    colors[y * size + x] = color;
                }
            }
        }
    }

    private static void DrawRectangle(Color[] colors, int size, int x, int y, int width, int height, Color color)
    {
        for (int row = y; row < y + height && row < size; row++)
        {
            for (int col = x; col < x + width && col < size; col++)
            {
                if (row >= 0 && col >= 0)
                {
                    colors[row * size + col] = color;
                }
            }
        }
    }

    private static void DrawArc(Color[] colors, int size, int cx, int cy, int radius, float angle, Color color, int thickness)
    {
        int halfThick = thickness / 2;
        float startAngle = (180f - angle / 2f) * Mathf.Deg2Rad;
        float endAngle = (180f + angle / 2f) * Mathf.Deg2Rad;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                if (dist >= radius - halfThick && dist <= radius + halfThick)
                {
                    float pointAngle = Mathf.Atan2(y - cy, x - cx);
                    if (pointAngle >= startAngle && pointAngle <= endAngle)
                    {
                        colors[y * size + x] = color;
                    }
                }
            }
        }
    }

    private static void DrawShipShape(Color[] colors, int size, int cx, int cy, int width, int height, Color color)
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int dx = Mathf.Abs(x - cx);
                int dy = Mathf.Abs(y - cy);
                
                if (dy <= height)
                {
                    int maxDx = width / 2 - (y > cy ? dy / 4 : 0);
                    if (dx <= maxDx)
                    {
                        colors[y * size + x] = color;
                    }
                }
            }
        }
    }
}
