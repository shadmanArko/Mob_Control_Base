using UnityEngine;

namespace PlayerSystem
{
    [CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig", order = 0)]
    public class PlayerConfig : ScriptableObject
    {
        [Header("Health Settings")]
        public int maxHealth = 100;
        public int damage = 10;
    
        [Header("Movement Settings")]
        public float moveSpeed = 5f;
        public float reducedSpeed = 6f;
    
        [Header("Combat Settings")]
        public float fireCooldown = 1f;
    
        [Header("Visual Settings")]
        public bool isBig = false;
    
        [Header("Score Settings")]
        public int enemyKillScore = 2;
    }
}