using UnityEngine;
using UnityEngine.UI;
public class Shooting : MonoBehaviour
{
    public UIManager uIManager;
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject ultBulletPrefab;
    [SerializeField] private CannonRecoil cannonRecoil;

    [SerializeField] float fireCd;
    private float fireTimer;

    [SerializeField] Image chargeBar;
    [SerializeField] float chargeAmount;
    private float currentCharge = 0;

    void Start()
    {
        fireTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        updateChargeBar();
        fireTimer -= Time.deltaTime;
    
        if (Input.GetMouseButton(0))
        {
            uIManager.FirstTouch(); // intro hand kapanması için

            if (fireTimer <= 0)
            {
                if (currentCharge < chargeAmount) { currentCharge++; }
            
                // Trigger recoil when shooting
                Shoot();
                TriggerShootRecoil(); // Add recoil effect
            
                fireTimer = fireCd;
            }
        }
    
        if (Input.GetMouseButtonUp(0) && (currentCharge >= chargeAmount))
        {
            // Trigger enhanced recoil for ultimate shot
            ShootUlt();
            TriggerUltimateRecoil(); // Add stronger recoil for ultimate
        
            currentCharge = 0;
        }
    }
    
    private void TriggerShootRecoil()
    {
        if (cannonRecoil != null)
        {
            cannonRecoil.TriggerRecoil(); // Simple recoil for regular shots
        }
    }
    
    private void TriggerUltimateRecoil()
    {
        if (cannonRecoil != null)
        {
            
            _ = cannonRecoil.TriggerEnhancedRecoilAsync();
        
            
            // await cannonRecoil.TriggerEnhancedRecoilAsync();
        }
    }
    private void updateChargeBar()
    {
        if (currentCharge < chargeAmount) { chargeBar.color = new Color32(33, 128, 231, 255); }
        else { chargeBar.color = new Color32(247, 166, 2, 255); }
        chargeBar.fillAmount = (currentCharge / chargeAmount);
    }
    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        // bullet.GetComponent<Rigidbody>().AddForce(new Vector3(0,0,10),ForceMode.Impulse);
    }
    private void ShootUlt()
    {
        GameObject bullet = Instantiate(ultBulletPrefab, firePoint.position, Quaternion.identity);
        //bullet.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 30), ForceMode.Impulse);
    }
}
