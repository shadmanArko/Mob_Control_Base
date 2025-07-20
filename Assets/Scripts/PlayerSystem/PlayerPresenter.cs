using System;
using UniRx;
using UnityEngine;

namespace PlayerSystem
{
    public class PlayerPresenter : IPlayerPresenter, IDisposable
    {
        private PlayerModel _model;
        private IPoolablePlayerView _view;
        private CompositeDisposable _disposables;
        private bool _isInitialized;
        
        private readonly PlayerModel.Factory _modelFactory;

        public PlayerPresenter(PlayerModel.Factory modelFactory)
        {
            _modelFactory = modelFactory;
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

            // Create the model when initializing
            _model = _modelFactory.Create();
            _disposables = new CompositeDisposable();
            SetupSubscriptions();
            _isInitialized = true;
        }

        private void ResetForReuse()
        {
            // Dispose old model and create a new one for reuse
            _model?.Dispose();
            _model = _modelFactory.Create();
            
            // Clear old subscriptions and setup new ones
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();
            SetupSubscriptions();
        }

        private void SetupSubscriptions()
        {
            if (_model == null || _view == null) return;

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
            if (_model == null) return;
            _model.UpdateTargeting(_view.Position);
        }

        private void HandleFixedUpdate()
        {
            if (_model == null) return;
            
            _view.SetVelocity(Vector3.zero);
            var newPosition = _model.CalculateNewPosition(_view.Position, Time.fixedDeltaTime);
            _view.SetPosition(newPosition);
        }

        private void UpdateViewScale()
        {
            if (_view.IsActive && _model != null)
            {
                var newScale = _model.CalculateScale(_view.StartScale);
                _view.SetScale(newScale);
            }
        }

        private void HandleCollision(Collision collision)
        {
            if (_model == null) return;
            
            if (collision.gameObject.CompareTag("Enemy"))
            {
                _model.HandleCollisionWithEnemy(collision.gameObject);
            }
        }

        private void HandleTrigger(Collider other)
        {
            if (_model == null) return;
            
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
            _model?.TakeDamage(damage);
        }

        public GameObject GetCloneSource()
        {
            return _model.CloneGameObject;
        }
        
        public void SetCloneSource(GameObject clone)
        {
            _model.CloneGameObject = clone;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
            _model?.Dispose();
            _isInitialized = false;
        }
    }
}