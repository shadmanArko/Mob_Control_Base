using UniRx;
using UnityEngine;

namespace PlayerSystem
{
    public class PlayerModel
    {
        private readonly PlayerConfig _config;
        
        private readonly ReactiveProperty<int> _health = new ReactiveProperty<int>();
        private readonly ReactiveProperty<bool> _isReachedCheckPoint = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<Vector3> _currentTarget = new ReactiveProperty<Vector3>();
        private readonly ReactiveProperty<float> _currentMoveSpeed = new ReactiveProperty<float>();
        private readonly ReactiveProperty<bool> _isDead = new ReactiveProperty<bool>(false);
    
        public IReadOnlyReactiveProperty<int> Health => _health;
        public IReadOnlyReactiveProperty<bool> IsReachedCheckPoint => _isReachedCheckPoint;
        public IReadOnlyReactiveProperty<Vector3> CurrentTarget => _currentTarget;
        public IReadOnlyReactiveProperty<float> CurrentMoveSpeed => _currentMoveSpeed;
        public IReadOnlyReactiveProperty<bool> IsDead => _isDead;

        public PlayerModel(PlayerConfig config)
        {
            _config = config;
            Initialize();
        }

        private void Initialize()
        {
            _health.Value = _config.maxHealth;
            _currentMoveSpeed.Value = _config.moveSpeed;
            _isDead.Value = false;
        }
    
        public void TakeDamage(int damageAmount)
        {
            _health.Value = Mathf.Max(0, _health.Value - damageAmount);
            if (_health.Value <= 0)
            {
                _isDead.Value = true;
            }
        }
    
        public void SetCheckPointReached(bool reached)
        {
            _isReachedCheckPoint.Value = reached;
        }
    
        public void SetTarget(Vector3 target)
        {
            _currentTarget.Value = target;
        }
    
        public void SetReducedSpeed()
        {
            _currentMoveSpeed.Value = _config.reducedSpeed;
        }
    
        public void ResetSpeed()
        {
            _currentMoveSpeed.Value = _config.moveSpeed;
        }
    
        public bool IsAlive => _health.Value > 0;
        public float HealthPercentage => (float)_health.Value / _config.maxHealth;
    }
}