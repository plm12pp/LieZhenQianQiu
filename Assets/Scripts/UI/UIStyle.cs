using UnityEngine;
using UnityEngine.UI;

public static class UIStyle
{
    public static readonly Color32 PrimaryRed = new Color32(0xC0, 0x39, 0x2B, 0xFF);
    public static readonly Color32 PrimaryBlue = new Color32(0x29, 0x80, 0xB9, 0xFF);
    public static readonly Color32 Gold = new Color32(0xF1, 0xC4, 0x0F, 0xFF);
    public static readonly Color32 DarkGold = new Color32(0xB7, 0x95, 0x0B, 0xFF);
    public static readonly Color32 Cream = new Color32(0xFE, 0xF9, 0xE7, 0xFF);
    public static readonly Color32 LightBeige = new Color32(0xF5, 0xE6, 0xD0, 0xFF);
    public static readonly Color32 DarkBrown = new Color32(0x5D, 0x40, 0x37, 0xFF);
    public static readonly Color32 InkBlack = new Color32(0x1A, 0x1A, 0x2E, 0xFF);
    public static readonly Color32 JadeGreen = new Color32(0x00, 0x9B, 0x77, 0xFF);
    public static readonly Color32 Purple = new Color32(0x8E, 0x44, 0xAD, 0xFF);
    public static readonly Color32 Gray = new Color32(0x9C, 0xA3, 0xAF, 0xFF);
    public static readonly Color32 DarkGray = new Color32(0x4A, 0x55, 0x68, 0xFF);
    
    public static readonly Color RarityCommon = new Color32(0x9C, 0xA3, 0xAF, 0xFF);
    public static readonly Color RarityRare = new Color32(0x3B, 0x82, 0xF6, 0xFF);
    public static readonly Color RarityEpic = new Color32(0x8B, 0x5C, 0xF6, 0xFF);
    public static readonly Color RarityLegendary = new Color32(0xF5, 0x9E, 0x0B, 0xFF);
    
    public static readonly Color PlayerBlue = new Color32(0x3B, 0x82, 0xF6, 0xFF);
    public static readonly Color EnemyRed = new Color32(0xEF, 0x44, 0x44, 0xFF);
    
    public static Color GetRarityColor(string rarity)
    {
        switch (rarity)
        {
            case "普通": return RarityCommon;
            case "稀有": return RarityRare;
            case "史诗": return RarityEpic;
            case "传说": return RarityLegendary;
            default: return RarityCommon;
        }
    }
    
    public static string GetRarityName(string rarity)
    {
        return rarity;
    }
    
    public static Color Lerp(Color a, Color b, float t)
    {
        return new Color(
            Mathf.Lerp(a.r, b.r, t),
            Mathf.Lerp(a.g, b.g, t),
            Mathf.Lerp(a.b, b.b, t),
            Mathf.Lerp(a.a, b.a, t)
        );
    }
}

public class RoundedRect
{
    public static Sprite CreateRoundedSprite(int width, int height, int radius, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool isInside = true;
                
                if (x < radius && y < radius)
                {
                    float dx = radius - x;
                    float dy = radius - y;
                    if (dx * dx + dy * dy > radius * radius)
                        isInside = false;
                }
                else if (x >= width - radius && y < radius)
                {
                    float dx = x - (width - radius - 1);
                    float dy = radius - y;
                    if (dx * dx + dy * dy > radius * radius)
                        isInside = false;
                }
                else if (x < radius && y >= height - radius)
                {
                    float dx = radius - x;
                    float dy = y - (height - radius - 1);
                    if (dx * dx + dy * dy > radius * radius)
                        isInside = false;
                }
                else if (x >= width - radius && y >= height - radius)
                {
                    float dx = x - (width - radius - 1);
                    float dy = y - (height - radius - 1);
                    if (dx * dx + dy * dy > radius * radius)
                        isInside = false;
                }
                
                colors[y * width + x] = isInside ? color : Color.clear;
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
    
    public static Sprite CreateRoundedGradientSprite(int width, int height, int radius, 
        Color topColor, Color bottomColor)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / height;
            Color rowColor = Color.Lerp(topColor, bottomColor, t);
            
            for (int x = 0; x < width; x++)
            {
                bool isInside = true;
                
                if (x < radius && y < radius)
                {
                    float dx = radius - x;
                    float dy = radius - y;
                    if (dx * dx + dy * dy > radius * radius)
                        isInside = false;
                }
                else if (x >= width - radius && y < radius)
                {
                    float dx = x - (width - radius - 1);
                    float dy = radius - y;
                    if (dx * dx + dy * dy > radius * radius)
                        isInside = false;
                }
                else if (x < radius && y >= height - radius)
                {
                    float dx = radius - x;
                    float dy = y - (height - radius - 1);
                    if (dx * dx + dy * dy > radius * radius)
                        isInside = false;
                }
                else if (x >= width - radius && y >= height - radius)
                {
                    float dx = x - (width - radius - 1);
                    float dy = y - (height - radius - 1);
                    if (dx * dx + dy * dy > radius * radius)
                        isInside = false;
                }
                
                colors[y * width + x] = isInside ? rowColor : Color.clear;
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
}

public class BorderEffect
{
    public static Sprite CreateBorderedSprite(int width, int height, int borderWidth, 
        int borderRadius, Color borderColor, Color fillColor)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Color pixelColor = Color.clear;
                
                if (IsInsideRoundedRect(x, y, width, height, borderRadius))
                {
                    if (IsInsideBorder(x, y, width, height, borderWidth, borderRadius))
                    {
                        pixelColor = fillColor;
                    }
                    else
                    {
                        pixelColor = borderColor;
                    }
                }
                
                colors[y * width + x] = pixelColor;
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
    
    public static Sprite CreateGlowSprite(int width, int height, Color glowColor, float glowStrength = 0.5f)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] colors = new Color[width * height];
        
        Vector2 center = new Vector2(width / 2f, height / 2f);
        float maxDist = Mathf.Min(width, height) / 2f;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float alpha = Mathf.Clamp01(1f - dist / maxDist) * glowStrength;
                colors[y * width + x] = new Color(glowColor.r, glowColor.g, glowColor.b, alpha);
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
    
    static bool IsInsideRoundedRect(int x, int y, int width, int height, int radius)
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
    
    static bool IsInsideBorder(int x, int y, int width, int height, int borderWidth, int radius)
    {
        int innerX = x - borderWidth;
        int innerY = y - borderWidth;
        int innerWidth = width - 2 * borderWidth;
        int innerHeight = height - 2 * borderWidth;
        int innerRadius = Mathf.Max(0, radius - borderWidth);
        
        if (innerX < 0 || innerY < 0 || innerX >= innerWidth || innerY >= innerHeight)
            return false;
        
        if (innerX < innerRadius && innerY < innerRadius)
        {
            float dx = innerRadius - innerX;
            float dy = innerRadius - innerY;
            return dx * dx + dy * dy <= innerRadius * innerRadius;
        }
        if (innerX >= innerWidth - innerRadius && innerY < innerRadius)
        {
            float dx = innerX - (innerWidth - innerRadius - 1);
            float dy = innerRadius - innerY;
            return dx * dx + dy * dy <= innerRadius * innerRadius;
        }
        if (innerX < innerRadius && innerY >= innerHeight - innerRadius)
        {
            float dx = innerRadius - innerX;
            float dy = innerY - (innerHeight - innerRadius - 1);
            return dx * dx + dy * dy <= innerRadius * innerRadius;
        }
        if (innerX >= innerWidth - innerRadius && innerY >= innerHeight - innerRadius)
        {
            float dx = innerX - (innerWidth - innerRadius - 1);
            float dy = innerY - (innerHeight - innerRadius - 1);
            return dx * dx + dy * dy <= innerRadius * innerRadius;
        }
        
        return true;
    }
}

public class IconGenerator
{
    public static Sprite CreateTroopIcon(int size, string troopType, Color primaryColor, Color secondaryColor)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                colors[y * size + x] = Color.clear;
            }
        }
        
        int center = size / 2;
        int bodyRadius = size / 3;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Mathf.Sqrt((x - center) * (x - center) + (y - center) * (y - center));
                if (dist <= bodyRadius)
                {
                    float t = dist / bodyRadius;
                    colors[y * size + x] = Color.Lerp(primaryColor, secondaryColor, t * 0.5f);
                }
            }
        }
        
        int headY = center + bodyRadius / 2;
        int headRadius = bodyRadius / 2;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Mathf.Sqrt((x - center) * (x - center) + (y - headY) * (y - headY));
                if (dist <= headRadius)
                {
                    colors[y * size + x] = secondaryColor;
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
    
    public static Sprite CreateStarIcon(int size, Color color)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] colors = new Color[size * size];
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                colors[y * size + x] = Color.clear;
            }
        }
        
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
                    float alpha = 1f - (dist / outerRadius) * 0.3f;
                    colors[y * size + x] = new Color(color.r, color.g, color.b, alpha);
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
