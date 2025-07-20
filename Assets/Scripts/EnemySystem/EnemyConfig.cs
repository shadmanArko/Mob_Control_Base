using UnityEngine;

namespace DefaultNamespace.EnemySystem
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Configs/EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("Health Settings")]
        public int maxHealth = 100;
        public int damage = 10;
    
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
    
        [Header("Combat Settings")]
        public float fireCooldown = 1f;
    
        [Header("Enemy Type")]
        public bool isBig = false;
    
        [Header("Visual Effects")]
        public ParticleSystem hitParticlePrefab;
    
        [Header("Speed Reduction")]
        public float reducedSpeed = 6f;
    }
}