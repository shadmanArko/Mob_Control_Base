using UnityEngine;
using DG.Tweening;

public class UIPopupAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.6f;
    [SerializeField] private float bounceStrength = 1.3f;
    [SerializeField] private Ease easeType = Ease.OutBounce;
    
    private RectTransform rectTransform;
    private Vector3 originalScale;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        
        // Start hidden
        rectTransform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
    
    [ContextMenu("Show Popup")]
    public void ShowPopup()
    {
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);
        
        // Reset scale to zero
        rectTransform.localScale = Vector3.zero;
        
        // Animate to original size with bounce
        rectTransform.DOScale(originalScale, animationDuration)
            .SetEase(easeType);
    }
    
    public void HidePopup()
    {
        rectTransform.DOScale(Vector3.zero, animationDuration * 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() => gameObject.SetActive(false));
    }
    
    [ContextMenu("Show Popup with Overshoot")]
    public void ShowPopupWithOvershoot()
    {
        if (gameObject.activeSelf) return;
        gameObject.SetActive(true);
        rectTransform.localScale = Vector3.zero;
        
        // First scale up beyond target, then settle to normal size
        Sequence bounceSequence = DOTween.Sequence();
        bounceSequence.Append(rectTransform.DOScale(originalScale * bounceStrength, animationDuration * 0.6f)
            .SetEase(Ease.OutQuart));
        bounceSequence.Append(rectTransform.DOScale(originalScale, animationDuration * 0.4f)
            .SetEase(Ease.OutBounce));
    }
}