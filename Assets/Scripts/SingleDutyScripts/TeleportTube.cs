using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class TeleportTube : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform tubeHead;
    [SerializeField] private Transform tubeTail;
    [SerializeField] private float disappearTime = 1f;
    
    [Header("Vibration Settings")]
    [SerializeField] private float vibrationIntensity = 0.3f;
    [SerializeField] private float vibrationDuration = 0.5f;
    
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            VibrateHead();
            _ = TeleportUnit(other.gameObject);
        }
    }
    
    private async Task TeleportUnit(GameObject unit)
    {
        if (unit == null) return;
        
        unit.SetActive(false);
        
        await Task.Delay((int)(disappearTime * 1000));
        
        unit.transform.position = tubeTail.position;
        //unit.transform.rotation = tubeTail.rotation;
        
        unit.SetActive(true);
    }
    
    private void VibrateHead()
    {
        tubeHead.DOShakeScale(vibrationDuration, vibrationIntensity);
    }
    
}