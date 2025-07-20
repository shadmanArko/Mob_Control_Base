using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIBlurAnimation : MonoBehaviour
{
    [Header("Blur Animation Settings")]
    [SerializeField] private float animationDuration = 0.8f;
    [SerializeField] private float maxBlurSize = 5f;
    [SerializeField] private Ease blurEase = Ease.OutQuart;
    
    [Header("Fade Settings")]
    [SerializeField] private bool fadeAlpha = true;
    [SerializeField] private float targetAlpha = 0.8f;
    
    private Material blurMaterial;
    private Image panelImage;
    private CanvasGroup canvasGroup;
    private float originalAlpha;
    
    void Awake()
    {
        panelImage = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null && fadeAlpha)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        if (canvasGroup != null)
        {
            originalAlpha = canvasGroup.alpha;
        }
        
        SetupBlurMaterial();
        
        // Start hidden
        gameObject.SetActive(false);
    }
    
    void SetupBlurMaterial()
    {
        // Create a blur material using Unity's UI Default shader with blur effect
        // You can replace this with a custom blur shader for better results
        if (panelImage != null)
        {
            // Create material instance
            blurMaterial = new Material(Shader.Find("UI/Default"));
            panelImage.material = blurMaterial;
        }
    }
    
    [ContextMenu("Show Blur Panel")]
    public void ShowBlurPanel()
    {
        if (gameObject.activeSelf) return;
        
        gameObject.SetActive(true);
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        
        // Animate blur effect using scale and alpha for blur simulation
        transform.localScale = Vector3.one * 0.95f;
        
        Sequence blurSequence = DOTween.Sequence();
        
        // Scale animation for blur effect simulation
        blurSequence.Append(transform.DOScale(Vector3.one, animationDuration)
            .SetEase(blurEase));
            
        // Fade in animation
        if (canvasGroup != null && fadeAlpha)
        {
            blurSequence.Join(canvasGroup.DOFade(targetAlpha, animationDuration)
                .SetEase(blurEase));
        }
        
        // Simulate blur with multiple overlapping elements (optional enhancement)
        SimulateBlurEffect();
    }
    
    public void HideBlurPanel()
    {
        Sequence hideSequence = DOTween.Sequence();
        
        // Scale down slightly
        hideSequence.Append(transform.DOScale(Vector3.one * 0.95f, animationDuration * 0.5f)
            .SetEase(Ease.InQuart));
            
        // Fade out
        if (canvasGroup != null)
        {
            hideSequence.Join(canvasGroup.DOFade(0f, animationDuration * 0.5f)
                .SetEase(Ease.InQuart));
        }
        
        hideSequence.OnComplete(() => gameObject.SetActive(false));
    }
    
    // Simulate blur by creating a soft fade effect
    void SimulateBlurEffect()
    {
        if (panelImage == null) return;
        
        // Create a soft blur appearance by animating color alpha
        Color startColor = panelImage.color;
        startColor.a = 0f;
        panelImage.color = startColor;
        
        Color targetColor = panelImage.color;
        targetColor.a = targetAlpha;
        
        panelImage.DOColor(targetColor, animationDuration)
            .SetEase(blurEase);
    }
    
    // Advanced blur with custom shader support (optional)
    public void ShowBlurPanelAdvanced()
    {
        gameObject.SetActive(true);
        
        // If you have a custom blur shader, animate its blur property
        if (blurMaterial != null && blurMaterial.HasProperty("_BlurSize"))
        {
            blurMaterial.SetFloat("_BlurSize", 0f);
            
            Sequence advancedBlur = DOTween.Sequence();
            advancedBlur.Append(DOTween.To(() => blurMaterial.GetFloat("_BlurSize"),
                x => blurMaterial.SetFloat("_BlurSize", x), maxBlurSize, animationDuration)
                .SetEase(blurEase));
                
            if (canvasGroup != null && fadeAlpha)
            {
                canvasGroup.alpha = 0f;
                advancedBlur.Join(canvasGroup.DOFade(targetAlpha, animationDuration)
                    .SetEase(blurEase));
            }
        }
        else
        {
            // Fallback to basic blur simulation
            ShowBlurPanel();
        }
    }
    
    void OnDestroy()
    {
        // Clean up material instance
        if (blurMaterial != null)
        {
            DestroyImmediate(blurMaterial);
        }
    }
}