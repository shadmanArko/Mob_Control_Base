using System;
using UnityEngine;

namespace DefaultNamespace.EnemySystem
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private ParticleSystem enemyParticle;
        [SerializeField] private Rigidbody rigidBody;
        
        public event Action<int> OnDamageableCollision;
        public event Action OnSpeedReduceCollision;
        public event Action OnEndGameCollision;
        
        private Vector3 _startScale;
        private EnemyConfig _config;

        private void Awake()
        {
            if (rigidBody == null)
                rigidBody = GetComponent<Rigidbody>();
            
            _startScale = transform.localScale;
        }

        public void Initialize(EnemyConfig config)
        {
            _config = config;
            transform.localScale = _startScale;
            gameObject.SetActive(true);
        }

        public void UpdatePosition(Vector3 position)
        {
            transform.position = position;
        }

        public void UpdateScale(float scaleMultiplier)
        {
            transform.localScale = _startScale * scaleMultiplier;
        }

        public void PlayHitEffect()
        {
            if (enemyParticle != null)
                enemyParticle.Play();
        }

        public void SetVelocityZero()
        {
            if (rigidBody != null)
                rigidBody.velocity = Vector3.zero;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public void ResetForPooling()
        {
            transform.localScale = _startScale;
            gameObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                var takeDamageFromPlayer = damageable.TakeDamageFromPlayer();
                OnDamageableCollision?.Invoke(takeDamageFromPlayer);
                damageable.DoDamageToPlayer(_config?.damage ?? 0);
                 
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("SpeedReducePoint"))
            {
                OnSpeedReduceCollision?.Invoke();
            }
            else if (other.gameObject.CompareTag("EndGame"))
            {
                OnEndGameCollision?.Invoke();
            }
        }

        private void OnDestroy()
        {
            OnDamageableCollision = null;
            OnSpeedReduceCollision = null;
            OnEndGameCollision = null;
        }
    }
}