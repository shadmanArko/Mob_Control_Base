using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System.Threading;

public class CannonRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField] private float squishAmount = 0.7f; // How much to squish (0.7 = 70% of original size)
    [SerializeField] private float squishDuration = 0.08f; // How long the squish lasts
    [SerializeField] private float returnDuration = 0.25f; // How long to return to normal
    [SerializeField] private Ease squishEase = Ease.OutQuad;
    [SerializeField] private Ease returnEase = Ease.OutElastic;
    
    [Header("Position Recoil")]
    [SerializeField] private bool enablePositionRecoil = true;
    [SerializeField] private float recoilDistance = 0.3f; // How far back the cannon moves
    [SerializeField] private Vector3 recoilDirection = Vector3.back; // Local direction for recoil
    
    [Header("Advanced Effects")]
    [SerializeField] private bool enableRotationRecoil = true;
    [SerializeField] private Vector3 rotationRecoil = new Vector3(-15f, 0f, 0f); // Pitch back slightly
    [SerializeField] private bool enableStretchEffect = true;
    [SerializeField] private float stretchAmount = 1.1f; // Stretch before squish for anticipation
    
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Vector3 originalRotation;
    private CancellationTokenSource cancellationTokenSource;
    
    void Start()
    {
        // Store original transform values
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
        originalRotation = transform.localEulerAngles;
    }
    
    void OnDestroy()
    {
        // Cancel any running animations when object is destroyed
        cancellationTokenSource?.Cancel();
        transform.DOKill();
    }
    
    /// <summary>
    /// Trigger recoil effect with multiple animation layers
    /// </summary>
    public async Task TriggerRecoilAsync()
    {
        // Cancel any existing animation
        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();
        
        try
        {
            await PerformRecoilAnimation(cancellationTokenSource.Token);
        }
        catch (System.OperationCanceledException)
        {
            // Animation was cancelled, reset to original state
            ResetToOriginalState();
        }
    }
    
    /// <summary>
    /// Non-async version for compatibility
    /// </summary>
    public void TriggerRecoil()
    {
        _ = TriggerRecoilAsync(); // Fire and forget
        TriggerMuzzleFlash();
    }
    
    private async Task PerformRecoilAnimation(CancellationToken cancellationToken)
    {
        // Kill any existing tweens
        transform.DOKill();
        
        // Create sequence for complex animation
        Sequence recoilSequence = DOTween.Sequence();
        
        // Optional: Quick stretch for anticipation (makes the squish more satisfying)
        if (enableStretchEffect)
        {
            recoilSequence.Append(
                transform.DOScale(
                    new Vector3(originalScale.x, originalScale.y, originalScale.z * stretchAmount),
                    squishDuration * 0.3f
                ).SetEase(Ease.OutQuad)
            );
        }
        
        // Main recoil animations happening simultaneously
        var squishTween = transform.DOScale(
            new Vector3(
                originalScale.x * 0.95f, // Slight width compression
                originalScale.y * 0.9f,  // Slight height compression  
                originalScale.z * squishAmount // Main depth compression
            ),
            squishDuration
        ).SetEase(squishEase);
        
        recoilSequence.Append(squishTween);
        
        // Position recoil
        if (enablePositionRecoil)
        {
            Vector3 recoilTarget = originalPosition + Vector3.Scale(recoilDirection, new Vector3(recoilDistance, recoilDistance, recoilDistance));
            var positionTween = transform.DOLocalMove(recoilTarget, squishDuration).SetEase(squishEase);
            recoilSequence.Join(positionTween);
        }
        
        // Rotation recoil (cannon kicks back)
        if (enableRotationRecoil)
        {
            Vector3 rotationTarget = originalRotation + rotationRecoil;
            var rotationTween = transform.DOLocalRotate(rotationTarget, squishDuration).SetEase(squishEase);
            recoilSequence.Join(rotationTween);
        }
        
        // Return to original state with bounce
        var returnScaleTween = transform.DOScale(originalScale, returnDuration).SetEase(returnEase);
        recoilSequence.Append(returnScaleTween);
        
        if (enablePositionRecoil)
        {
            var returnPositionTween = transform.DOLocalMove(originalPosition, returnDuration).SetEase(returnEase);
            recoilSequence.Join(returnPositionTween);
        }
        
        if (enableRotationRecoil)
        {
            var returnRotationTween = transform.DOLocalRotate(originalRotation, returnDuration).SetEase(returnEase);
            recoilSequence.Join(returnRotationTween);
        }
        
        // Wait for animation to complete
        await recoilSequence.AsyncWaitForCompletion();
        
        // Ensure we end at exactly the original values
        ResetToOriginalState();
    }
    
    private void ResetToOriginalState()
    {
        transform.localScale = originalScale;
        transform.localPosition = originalPosition;
        transform.localEulerAngles = originalRotation;
    }
    
    /// <summary>
    /// Enhanced recoil with additional effects (particles, screen shake, etc.)
    /// </summary>
    public async Task TriggerEnhancedRecoilAsync()
    {
        // Trigger multiple effects simultaneously
        var recoilTask = TriggerRecoilAsync();
        
        // Add particle effects if you have them
        TriggerMuzzleFlash();
        
        // Add screen shake if you have camera shake
        TriggerScreenShake();
        
        // Wait for recoil animation to complete
        await recoilTask;
    }
    
    private void TriggerMuzzleFlash()
    {
        // Example: Find and trigger particle system
        var muzzleFlash = GetComponentInChildren<ParticleSystem>();
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        
        // Or trigger custom muzzle flash effect
        // MuzzleFlashManager.Instance?.TriggerFlash(transform.position + transform.forward);
    }
    
    private void TriggerScreenShake()
    {
        // Example screen shake (assuming you have a camera shake system)
        // CameraShake.Instance?.Shake(0.1f, 0.2f);
        
        // Or use DOTween to shake the camera directly
        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.DOShakePosition(1f, 1f, 10, 90f, false, true);
        }
    }
    
    /// <summary>
    /// Chain multiple recoil effects (for rapid fire)
    /// </summary>
    public async Task TriggerRapidFireRecoil(int shotCount, float delayBetweenShots = 0.1f)
    {
        for (int i = 0; i < shotCount; i++)
        {
            var recoilTask = TriggerRecoilAsync();
            
            if (i < shotCount - 1) // Don't wait after the last shot
            {
                await Task.Delay((int)(delayBetweenShots * 1000));
            }
        }
    }
    
    /// <summary>
    /// Test method for debugging in editor
    /// </summary>
    [ContextMenu("Test Recoil")]
    private void TestRecoil()
    {
        TriggerRecoil();
    }
    
    [ContextMenu("Test Enhanced Recoil")]
    private void TestEnhancedRecoil()
    {
        _ = TriggerEnhancedRecoilAsync();
    }
}
