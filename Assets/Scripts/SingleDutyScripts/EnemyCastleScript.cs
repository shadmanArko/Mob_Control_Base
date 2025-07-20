using DefaultNamespace.EnemySystem;
using UnityEngine;
using TMPro;
using Zenject;

public class EnemyCastleScript : MonoBehaviour
{
    [SerializeField] private ParticleSystem castleParticular;
    [SerializeField] private TextMeshProUGUI health_text;
    [SerializeField] private int health = 100;
    [SerializeField] private DamageFlickerController damageFlickerController;
    [SerializeField] private Transform spawnPoint;

    [Header("Spawn Events: Time-NormalEnemyAmount-BigEnemyAmount")]
    [SerializeField] private Vector3[] spawnEvents;
    
    [Header("Spawn Settings")]
    [SerializeField] private Vector2 spawnRangeX = new Vector2(-2, 2);
    [SerializeField] private Vector2 spawnRangeZ = new Vector2(-3, 3);
    
    private float spawnTimer;
    private EnemyManager enemyManager;

    [Inject]
    public void Initialize(EnemyManager manager)
    {
        enemyManager = manager;
    }

    private void Start()
    {
        spawnTimer = 0;
        
        // If not injected, try to find in scene
        if (enemyManager == null)
        {
            enemyManager = FindObjectOfType<EnemyManager>();
        }
    }

    private void Update()
    {
        if (health <= 0) 
        { 
            Destroy(gameObject); 
        }
        
        spawnTimer += Time.deltaTime;
        ProcessSpawnEvents();
        UpdateHealthUI();
    }

    private void ProcessSpawnEvents()
    {
        for (int i = 0; i < spawnEvents.Length; i++)
        {
            if (spawnEvents[i].x > 0 && spawnEvents[i].x <= spawnTimer)
            {
                SpawnEnemies((int)spawnEvents[i].y, (int)spawnEvents[i].z);
                spawnEvents[i] = new Vector3(0, spawnEvents[i].y, spawnEvents[i].z); // Mark as used
            }
        }
    }

    private void SpawnEnemies(int normalEnemyAmount, int bigEnemyAmount)
    {
        if (enemyManager == null)
        {
            Debug.LogError("EnemyManager not found! Cannot spawn enemies.");
            return;
        }

        // Spawn normal enemies
        for (int i = 0; i < normalEnemyAmount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            enemyManager.SpawnEnemy(spawnPosition, EnemyType.Normal);
        }

        // Spawn big enemies
        for (int i = 0; i < bigEnemyAmount; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            enemyManager.SpawnEnemy(spawnPosition, EnemyType.Big);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 newSpawnPoint = spawnPoint.position;
        
        float spawnX = Random.Range(spawnRangeX.x, spawnRangeX.y);
        float spawnZ = Random.Range(spawnRangeZ.x, spawnRangeZ.y);
        
        newSpawnPoint.x += spawnX;
        newSpawnPoint.z += spawnZ;
        
        return newSpawnPoint;
    }

    private void UpdateHealthUI()
    {
        if (health_text != null)
        {
            health_text.text = health.ToString();
        }
    }

    public void GetHit(int damage)
    {
        health -= damage;
        CastleHitEffect();
    }

    private void CastleHitEffect()
    {
        if (castleParticular != null)
        {
            castleParticular.Play();
        }
        
        if (damageFlickerController != null)
        {
            damageFlickerController.TriggerDamageFlicker();
        }
    }

    // Editor helper to visualize spawn range
    private void OnDrawGizmosSelected()
    {
        if (spawnPoint == null) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            spawnPoint.position, 
            new Vector3(spawnRangeX.y - spawnRangeX.x, 1, spawnRangeZ.y - spawnRangeZ.x)
        );
    }
}
