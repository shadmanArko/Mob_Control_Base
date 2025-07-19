using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerSystem
{
    public class PlayerModel
    {
        private readonly PlayerConfig _config;
        private readonly IScoreService _scoreService;

        // Reactive Properties
        private readonly ReactiveProperty<int> _health = new ReactiveProperty<int>();
        private readonly ReactiveProperty<bool> _isReachedCheckPoint = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<Vector3> _currentTarget = new ReactiveProperty<Vector3>();
        private readonly ReactiveProperty<float> _currentMoveSpeed = new ReactiveProperty<float>();
        private readonly ReactiveProperty<bool> _isDead = new ReactiveProperty<bool>(false);
        private readonly ReactiveProperty<float> _fireTimer = new ReactiveProperty<float>(0f);

        // Public Properties
        public IReadOnlyReactiveProperty<int> Health => _health;
        public IReadOnlyReactiveProperty<bool> IsReachedCheckPoint => _isReachedCheckPoint;
        public IReadOnlyReactiveProperty<Vector3> CurrentTarget => _currentTarget;
        public IReadOnlyReactiveProperty<float> CurrentMoveSpeed => _currentMoveSpeed;
        public IReadOnlyReactiveProperty<bool> IsDead => _isDead;
        public IReadOnlyReactiveProperty<float> FireTimer => _fireTimer;

        public bool IsAlive => _health.Value > 0;
        public float HealthPercentage => (float)_health.Value / _config.maxHealth;
        public bool IsBig => _config.isBig;

        private CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerModel(PlayerConfig config, IScoreService scoreService)
        {
            _config = config;
            _scoreService = scoreService;
            Initialize();
        }

        private void Initialize()
        {
            _health.Value = _config.maxHealth;
            _currentMoveSpeed.Value = _config.moveSpeed;
            _isDead.Value = false;
            _fireTimer.Value = 0f;

            // Auto-update fire timer
            Observable.EveryUpdate()
                .Subscribe(_ => UpdateFireTimer())
                .AddTo(_disposables);
        }

        // Business Logic Methods

        public void UpdateTargeting(Vector3 playerPosition)
        {
            var target = FindClosestEnemyCastle(playerPosition);
            _currentTarget.Value = target;
        }

        private Vector3 FindClosestEnemyCastle(Vector3 playerPosition)
        {
            var castles = GameObject.FindGameObjectsWithTag("EnemyCastle");

            if (castles.Length == 0)
                return Vector3.zero;

            var closestCastle = castles
                .OrderBy(castle => Vector3.Distance(playerPosition, castle.transform.position))
                .FirstOrDefault();

            var target = closestCastle.transform.position;
            target.y = 0;
            return target;
        }

        public Vector3 CalculateMovementTarget(Vector3 currentPosition)
        {
            var target = _currentTarget.Value;

            if (_isReachedCheckPoint.Value)
            {
                return target;
            }
            else
            {
                var modifiedTarget = target;
                modifiedTarget.x = currentPosition.x;
                return modifiedTarget;
            }
        }

        public Vector3 CalculateNewPosition(Vector3 currentPosition, float deltaTime)
        {
            var targetPosition = CalculateMovementTarget(currentPosition);
            var speed = _currentMoveSpeed.Value;

            return Vector3.MoveTowards(currentPosition, targetPosition, deltaTime * speed);
        }

        public Vector3 CalculateScale(Vector3 originalScale)
        {
            if (_config.isBig)
            {
                return originalScale * HealthPercentage;
            }

            return originalScale;
        }

        public void TakeDamage(int damageAmount)
        {
            _health.Value = Mathf.Max(0, _health.Value - damageAmount);
            if (_health.Value <= 0)
            {
                _isDead.Value = true;
            }
        }

        public bool CanFire()
        {
            return _fireTimer.Value <= 0;
        }

        private void UpdateFireTimer()
        {
            if (_fireTimer.Value > 0)
            {
                _fireTimer.Value -= Time.deltaTime;
            }
        }

        private void ResetFireTimer()
        {
            _fireTimer.Value = _config.fireCooldown;
        }

        public void HandleCollisionWithEnemy(GameObject enemy)
        {
            if (!CanFire()) return;

            var enemyController = enemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.getHit(_config.damage);
                _scoreService.AddScore(_config.enemyKillScore);
                ResetFireTimer();
            }
        }

        public void HandleCollisionWithCastle(GameObject castle)
        {
            if (!CanFire()) return;

            var castleScript = castle.GetComponent<EnemyCastleScript>();
            if (castleScript != null)
            {
                castleScript.getHit(_config.damage);
                ResetFireTimer();
            }
        }

        public void HandleSpeedReduction()
        {
            _currentMoveSpeed.Value = _config.reducedSpeed;
        }

        public void HandleCheckPointReached()
        {
            _isReachedCheckPoint.Value = true;
        }

        public void ResetSpeed()
        {
            _currentMoveSpeed.Value = _config.moveSpeed;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }
        public class Factory : PlaceholderFactory<PlayerModel>
        {
        }
    }
}