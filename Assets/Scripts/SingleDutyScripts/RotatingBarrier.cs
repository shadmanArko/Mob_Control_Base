using UnityEngine;

public class RotatingBarrier : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f;
    
    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0,Space.Self);
    }
}