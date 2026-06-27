using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 战斗特效系统 - 管理游戏中各种视觉特效
/// </summary>
public class BattleEffectsManager : MonoBehaviour
{
    public static BattleEffectsManager instance;

    public GameObject particlePrefab;
    public Material defaultParticleMaterial;
    
    private Dictionary<string, ParticleSystem> effectPool;
    private Transform effectContainer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            effectPool = new Dictionary<string, ParticleSystem>();
            
            // 创建特效容器
            GameObject container = new GameObject("BattleEffects");
            effectContainer = container.transform;
            DontDestroyOnLoad(container);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 播放攻击特效
    /// </summary>
    public void PlayAttackEffect(Transform target, string effectType = "normal")
    {
        Vector3 position = target != null ? target.position : Vector3.zero;
        PlayEffect(effectType, position);
    }

    /// <summary>
    /// 播放特效
    /// </summary>
    public void PlayEffect(string effectName, Vector3 position, float duration = 1f)
    {
        StartCoroutine(PlayEffectCoroutine(effectName, position, duration));
    }

    private IEnumerator PlayEffectCoroutine(string effectName, Vector3 position, float duration)
    {
        GameObject effectObj = CreateEffectObject(effectName);
        if (effectObj == null) yield break;

        effectObj.transform.position = position;
        effectObj.SetActive(true);

        ParticleSystem ps = effectObj.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Play();
            yield return new WaitForSeconds(duration);
            ps.Stop();
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(effectObj);
    }

    private GameObject CreateEffectObject(string effectName)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Renderer renderer = obj.GetComponent<Renderer>();
        
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Sprites/Default"));
            
            switch (effectName.ToLower())
            {
                case "fire":
                case "burn":
                    mat.color = new Color(1f, 0.4f, 0f, 0.8f);
                    obj.AddComponent<FireEffect>();
                    break;
                case "ice":
                case "freeze":
                    mat.color = new Color(0.4f, 0.8f, 1f, 0.8f);
                    obj.AddComponent<IceEffect>();
                    break;
                case "lightning":
                    mat.color = new Color(1f, 1f, 0.2f, 0.9f);
                    obj.AddComponent<LightningEffect>();
                    break;
                case "critical":
                case "crit":
                    mat.color = new Color(1f, 0.8f, 0f, 0.9f);
                    obj.AddComponent<CriticalEffect>();
                    break;
                case "charge":
                    mat.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
                    obj.AddComponent<ChargeEffect>();
                    break;
                case "splash":
                    mat.color = new Color(0.6f, 0.6f, 0.6f, 0.7f);
                    obj.AddComponent<SplashEffect>();
                    break;
                case "heal":
                    mat.color = new Color(0.2f, 1f, 0.2f, 0.8f);
                    obj.AddComponent<HealEffect>();
                    break;
                case "shield":
                    mat.color = new Color(0.3f, 0.5f, 1f, 0.6f);
                    obj.AddComponent<ShieldEffect>();
                    break;
                default:
                    mat.color = new Color(1f, 1f, 1f, 0.5f);
                    obj.AddComponent<NormalHitEffect>();
                    break;
            }
            
            renderer.material = mat;
        }

        return obj;
    }

    /// <summary>
    /// 显示伤害数字
    /// </summary>
    public void ShowDamageNumber(Vector3 position, int damage, bool isCritical = false)
    {
        StartCoroutine(ShowDamageNumberCoroutine(position, damage, isCritical));
    }

    private IEnumerator ShowDamageNumberCoroutine(Vector3 position, int damage, bool isCritical)
    {
        GameObject textObj = new GameObject("DamageNumber");
        textObj.transform.position = position + Vector3.up * 0.5f;
        
        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = damage.ToString();
        textMesh.fontSize = isCritical ? 36 : 24;
        textMesh.color = isCritical ? Color.yellow : Color.red;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        
        // 添加浮动动画
        float duration = 1f;
        float elapsed = 0f;
        Vector3 startPos = textObj.transform.position;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            textObj.transform.position = startPos + Vector3.up * t * 1f;
            
            // 淡出效果
            Color c = textMesh.color;
            c.a = 1f - t;
            textMesh.color = c;
            
            yield return null;
        }
        
        Destroy(textObj);
    }

    /// <summary>
    /// 显示状态图标
    /// </summary>
    public void ShowStatusIcon(Vector3 position, string statusType)
    {
        GameObject iconObj = new GameObject("StatusIcon");
        iconObj.transform.position = position + Vector3.up * 1.5f;
        
        SpriteRenderer sr = iconObj.AddComponent<SpriteRenderer>();
        sr.sprite = CreateStatusIcon(statusType);
        sr.sortingOrder = 100;
    }

    private Sprite CreateStatusIcon(string statusType)
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color[] colors = new Color[size * size];
        
        Color iconColor;
        switch (statusType.ToLower())
        {
            case "burn":
            case "fire":
                iconColor = new Color(1f, 0.3f, 0f);
                break;
            case "freeze":
            case "ice":
                iconColor = new Color(0.3f, 0.7f, 1f);
                break;
            case "poison":
                iconColor = new Color(0.5f, 0.2f, 0.5f);
                break;
            case "stun":
                iconColor = new Color(1f, 1f, 0.2f);
                break;
            default:
                iconColor = Color.white;
                break;
        }
        
        // 绘制圆形图标
        int center = size / 2;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                if (dist <= size / 2 - 2)
                {
                    colors[y * size + x] = iconColor;
                }
                else if (dist <= size / 2)
                {
                    colors[y * size + x] = new Color(iconColor.r * 0.7f, iconColor.g * 0.7f, iconColor.b * 0.7f);
                }
            }
        }
        
        tex.SetPixels(colors);
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    /// <summary>
    /// 播放屏幕震动效果
    /// </summary>
    public void ScreenShake(float intensity = 0.3f, float duration = 0.2f)
    {
        StartCoroutine(ScreenShakeCoroutine(intensity, duration));
    }

    private IEnumerator ScreenShakeCoroutine(float intensity, float duration)
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;

        Vector3 originalPos = cam.transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;
            
            cam.transform.position = originalPos + new Vector3(x, y, 0);
            
            yield return null;
        }

        cam.transform.position = originalPos;
    }
}

// 特效脚本组件
public class FireEffect : MonoBehaviour
{
    void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * 10f) * 0.2f;
        transform.localScale = Vector3.one * scale;
        
        Color c = GetComponent<Renderer>().material.color;
        c.a = 0.5f + Mathf.Sin(Time.time * 8f) * 0.3f;
        GetComponent<Renderer>().material.color = c;
    }
}

public class IceEffect : MonoBehaviour
{
    void Update()
    {
        float scale = 1f + Mathf.Sin(Time.time * 5f) * 0.1f;
        transform.localScale = Vector3.one * scale;
    }
}

public class LightningEffect : MonoBehaviour
{
    private float flashInterval = 0.1f;
    private float nextFlash;
    
    void Update()
    {
        if (Time.time > nextFlash)
        {
            GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
            nextFlash = Time.time + flashInterval;
        }
    }
}

public class CriticalEffect : MonoBehaviour
{
    void Update()
    {
        float scale = 1f + Time.time * 0.5f;
        transform.localScale = Vector3.one * scale;
        
        Color c = GetComponent<Renderer>().material.color;
        c.a = 1f - (scale - 1f);
        GetComponent<Renderer>().material.color = c;
    }
}

public class ChargeEffect : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.forward, 360f * Time.deltaTime);
    }
}

public class SplashEffect : MonoBehaviour
{
    private float maxScale = 3f;
    private float growSpeed = 3f;
    
    void Update()
    {
        transform.localScale += Vector3.one * growSpeed * Time.deltaTime;
        
        Color c = GetComponent<Renderer>().material.color;
        c.a -= Time.deltaTime * 2f;
        GetComponent<Renderer>().material.color = c;
        
        if (c.a <= 0) Destroy(gameObject);
    }
}

public class HealEffect : MonoBehaviour
{
    void Update()
    {
        transform.position += Vector3.up * Time.deltaTime;
        
        Color c = GetComponent<Renderer>().material.color;
        c.a -= Time.deltaTime;
        GetComponent<Renderer>().material.color = c;
        
        if (c.a <= 0) Destroy(gameObject);
    }
}

public class ShieldEffect : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(Vector3.forward, -45f * Time.deltaTime);
        
        float pulse = 1f + Mathf.Sin(Time.time * 4f) * 0.1f;
        transform.localScale = Vector3.one * pulse;
    }
}

public class NormalHitEffect : MonoBehaviour
{
    private float lifetime = 0.3f;
    private float elapsed;
    
    void Update()
    {
        elapsed += Time.deltaTime;
        
        float scale = 1f + elapsed * 2f;
        transform.localScale = Vector3.one * scale;
        
        Color c = GetComponent<Renderer>().material.color;
        c.a = 1f - (elapsed / lifetime);
        GetComponent<Renderer>().material.color = c;
        
        if (elapsed >= lifetime) Destroy(gameObject);
    }
}
