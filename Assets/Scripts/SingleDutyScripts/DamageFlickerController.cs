using System.Threading.Tasks;
using UnityEngine;

public class DamageFlickerController : MonoBehaviour
{
    [Header("Flicker Settings")]
    [SerializeField] private float flickerDuration = 0.5f;
    [SerializeField] private Color flickerColor = Color.red;
    [SerializeField] private float flickerSpeed = 25f;
    [SerializeField] private Material flickerMaterial;
    
    private Renderer objectRenderer;
    private bool isFlickering = false;
    
    // Shader property IDs for better performance
    private static readonly int FlickerAmount = Shader.PropertyToID("_FlickerAmount");
    private static readonly int FlickerColor = Shader.PropertyToID("_FlickerColor");
    private static readonly int FlickerSpeed = Shader.PropertyToID("_FlickerSpeed");
    
    void Start()
    {
        
        if (flickerMaterial != null)
        {
            // Create instance of material to avoid affecting other objects
            // Set initial flicker properties
            flickerMaterial.SetFloat(FlickerAmount, 0f);
            flickerMaterial.SetColor(FlickerColor, flickerColor);
            flickerMaterial.SetFloat(FlickerSpeed, flickerSpeed);
        }
        else
        {
            Debug.LogError("DamageFlickerController: No Renderer component found!");
        }
    }
    
    public void TriggerDamageFlicker()
    {
        if (!isFlickering)
        {
            _ = StartFlicker();
        }
    }
    
    private async Task StartFlicker()
    {
        if (flickerMaterial == null || isFlickering) return;
        
        isFlickering = true;
        
        // Start flicker effect
        flickerMaterial.SetFloat(FlickerAmount, 1f);
        
        // Wait for flicker duration
        await Task.Delay((int)(flickerDuration * 1000));
        
        // Stop flicker effect
        flickerMaterial.SetFloat(FlickerAmount, 0f);
        
        isFlickering = false;
    }
    
    // Public methods for customization
    public void SetFlickerColor(Color color)
    {
        flickerColor = color;
        if (flickerMaterial != null)
            flickerMaterial.SetColor(FlickerColor, color);
    }
    
    public void SetFlickerSpeed(float speed)
    {
        flickerSpeed = speed;
        if (flickerMaterial != null)
            flickerMaterial.SetFloat(FlickerSpeed, speed);
    }
    
    public void SetFlickerDuration(float duration)
    {
        flickerDuration = duration;
    }
    
    public bool IsFlickering()
    {
        return isFlickering;
    }
    
    void OnDestroy()
    {
        // Clean up material instance
        if (flickerMaterial != null)
        {
            DestroyImmediate(flickerMaterial);
        }
    }
}