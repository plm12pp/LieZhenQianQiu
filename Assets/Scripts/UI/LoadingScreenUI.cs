using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenUI : MonoBehaviour
{
    public static LoadingScreenUI instance;
    
    public GameObject loadingPanel;
    public Image loadingBarFill;
    public Text loadingText;
    public Text tipText;
    public Image logoImage;
    
    public float minimumDisplayTime = 1.5f;
    
    private string[] loadingTips = {
        "枪兵克制骑兵，骑兵克制弓兵，弓兵克制步兵",
        "善用阵型可以大幅提升战斗力",
        "3格军阵虽然强大，但移动缓慢",
        "火攻对藤甲兵和象兵有奇效",
        "河道上的渡口是进攻的关键",
        "升级兵种可以大幅提升属性",
        "远程部队要放在后排保护",
        "重骑兵冲锋伤害极高",
        "盾兵可以有效抵挡远程攻击",
        "合理搭配兵种才能百战百胜"
    };
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void ShowLoadingScreen()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }
        
        if (tipText != null)
        {
            tipText.text = loadingTips[Random.Range(0, loadingTips.Length)];
        }
        
        if (loadingBarFill != null)
        {
            loadingBarFill.fillAmount = 0f;
        }
    }
    
    public void HideLoadingScreen()
    {
        StartCoroutine(HideRoutine());
    }
    
    System.Collections.IEnumerator HideRoutine()
    {
        yield return new WaitForSeconds(minimumDisplayTime);
        
        if (loadingBarFill != null)
        {
            float fillDuration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < fillDuration)
            {
                elapsed += Time.deltaTime;
                loadingBarFill.fillAmount = Mathf.Lerp(loadingBarFill.fillAmount, 1f, elapsed / fillDuration);
                yield return null;
            }
        }
        
        yield return new WaitForSeconds(0.2f);
        
        if (loadingPanel != null)
        {
            CanvasGroup canvasGroup = loadingPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = loadingPanel.AddComponent<CanvasGroup>();
            
            float fadeDuration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = 1f - (elapsed / fadeDuration);
                yield return null;
            }
            
            loadingPanel.SetActive(false);
            canvasGroup.alpha = 1f;
        }
    }
    
    public void LoadScene(string sceneName)
    {
        ShowLoadingScreen();
        StartCoroutine(LoadSceneRoutine(sceneName));
    }
    
    System.Collections.IEnumerator LoadSceneRoutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        while (!asyncLoad.isDone)
        {
            if (loadingBarFill != null)
            {
                loadingBarFill.fillAmount = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            }
            
            yield return null;
        }
        
        HideLoadingScreen();
    }
    
    public void SetLoadingProgress(float progress)
    {
        if (loadingBarFill != null)
        {
            loadingBarFill.fillAmount = Mathf.Clamp01(progress);
        }
    }
    
    public void SetLoadingText(string text)
    {
        if (loadingText != null)
        {
            loadingText.text = text;
        }
    }
}
