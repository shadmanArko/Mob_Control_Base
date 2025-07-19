using System;
using UniRx;
using UnityEngine;

namespace PlayerSystem
{
    public class PlayerPresenter : IPlayerPresenter, IDisposable
    {
        private readonly PlayerModel _model;
        private IPoolablePlayerView _view;
        private CompositeDisposable _disposables;
        private bool _isInitialized;

        public PlayerPresenter(PlayerModel model)
        {
            _model = model;
        }

        public void SetView(IPlayerView view)
        {
            _view = view as IPoolablePlayerView;
            if (_view == null)
            {
                Debug.LogError("View must implement IPoolablePlayerView for pooling");
            }
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                // Reset for reuse
                ResetForReuse();
                return;
            }

            _disposables = new CompositeDisposable();
            SetupSubscriptions();
            _isInitialized = true;
        }

        private void ResetForReuse()
        {
            // Reset model state for reuse
            // Model will handle its own reset logic
        }

        private void SetupSubscriptions()
        {
            Observable.EveryUpdate()
                .Where(_ => _view.IsActive)
                .Subscribe(_ => HandleUpdate())
                .AddTo(_disposables);

            Observable.EveryFixedUpdate()
                .Where(_ => _view.IsActive)
                .Subscribe(_ => HandleFixedUpdate())
                .AddTo(_disposables);

            _model.Health
                .Subscribe(_ => UpdateViewScale())
                .AddTo(_disposables);

            _model.IsDead
                .Where(isDead => isDead && _view.IsActive)
                .Subscribe(_ => _view.DestroyView())
                .AddTo(_disposables);

            _view.OnCollisionEntered
                .Where(_ => _view.IsActive)
                .Subscribe(collision => HandleCollision(collision))
                .AddTo(_disposables);

            _view.OnTriggerEntered
                .Where(_ => _view.IsActive)
                .Subscribe(collider => HandleTrigger(collider))
                .AddTo(_disposables);
        }

        private void HandleUpdate()
        {
            _model.UpdateTargeting(_view.Position);
        }

        private void HandleFixedUpdate()
        {
            _view.SetVelocity(Vector3.zero);
            var newPosition = _model.CalculateNewPosition(_view.Position, Time.fixedDeltaTime);
            _view.SetPosition(newPosition);
        }

        private void UpdateViewScale()
        {
            if (_view.IsActive)
            {
                var newScale = _model.CalculateScale(_view.StartScale);
                _view.SetScale(newScale);
            }
        }

        private void HandleCollision(Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                _model.HandleCollisionWithEnemy(collision.gameObject);
            }
        }

        private void HandleTrigger(Collider other)
        {
            switch (other.tag)
            {
                case "SpeedReducePoint":
                    _model.HandleSpeedReduction();
                    break;
                case "CheckPoint":
                    _model.HandleCheckPointReached();
                    break;
                case "EnemyCastle":
                    _model.HandleCollisionWithCastle(other.gameObject);
                    break;
            }
        }

        public void TakeDamage(int damage)
        {
            _model.TakeDamage(damage);
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _model?.Dispose();
        }
    }
}