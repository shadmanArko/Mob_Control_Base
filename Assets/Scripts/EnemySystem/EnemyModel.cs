using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace DefaultNamespace.EnemySystem
{
    public class EnemyModel : IDisposable
    {
        private readonly EnemyConfig _config;
        
        private readonly ReactiveProperty<int> _health = new ReactiveProperty<int>();
        private readonly ReactiveProperty<Vector3> _position = new ReactiveProperty<Vector3>();
        private readonly ReactiveProperty<bool> _isDead = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<float> _currentScale = new ReactiveProperty<float>(1f);
        
        private float _fireTimer;
        private Vector3 _targetPosition;
        private float _currentMoveSpeed;
        private Vector3 _startScale;
        private bool _isActive;

        public IReadOnlyReactiveProperty<int> Health => _health;
        public IReadOnlyReactiveProperty<Vector3> Position => _position;
        public IReadOnlyReactiveProperty<bool> IsDead => _isDead;
        public IReadOnlyReactiveProperty<float> CurrentScale => _currentScale;
        
        public Vector3 TargetPosition => _targetPosition;
        public float CurrentMoveSpeed => _currentMoveSpeed;
        public bool CanFire => _fireTimer <= 0;
        public int Damage => _config.damage;
        public bool IsBig => _config.isBig;
        public bool IsActive => _isActive;

        public EnemyModel(EnemyConfig config)
        {
            _config = config;
            Initialize();
        }

        public void Initialize()
        {
            _health.Value = _config.maxHealth;
            _isDead.Value = false;
            _fireTimer = 0;
            _currentMoveSpeed = _config.moveSpeed;
            _currentScale.Value = 1f;
            _isActive = true;
        }

        public void SetTargetPosition(Vector3 target)
        {
            _targetPosition = target;
        }

        public void SetStartScale(Vector3 scale)
        {
            _startScale = scale;
        }

        public void UpdatePosition(Vector3 newPosition)
        {
            _position.Value = newPosition;
        }

        public void UpdateFireTimer(float deltaTime)
        {
            _fireTimer -= deltaTime;
        }

        public void TakeDamage(int damage)
        {
            if (!_isActive || _isDead.Value) return;
            
            _health.Value = Mathf.Max(0, _health.Value - damage);
            
            if (_config.isBig && _health.Value > 0)
            {
                _currentScale.Value = (float)_health.Value / _config.maxHealth;
            }
            
            if (_health.Value <= 0)
            {
                _isDead.Value = true;
                _isActive = false;
            }
        }

        public void OnCollisionWithDamageable()
        {
            _fireTimer = _config.fireCooldown;
        }

        public void ReduceSpeed()
        {
            _currentMoveSpeed = _config.reducedSpeed;
        }

        public void ResetForPooling()
        {
            _isActive = false;
        }

        public void ActivateFromPool()
        {
            Initialize();
        }

        public Vector3 CalculateNextPosition(Vector3 currentPosition, float deltaTime)
        {
            if (!_isActive) return currentPosition;
            
            return Vector3.MoveTowards(currentPosition, _targetPosition, deltaTime * _currentMoveSpeed);
        }

        public void Dispose()
        {
            _health?.Dispose();
            _position?.Dispose();
            _isDead?.Dispose();
            _currentScale?.Dispose();
        }

        public class Factory : PlaceholderFactory<EnemyConfig, EnemyModel> { }
    }
}