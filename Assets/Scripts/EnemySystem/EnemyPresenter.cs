using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace.EnemySystem
{
    public class EnemyPresenter : IDisposable, ICollection<IDisposable>
    {
        private readonly EnemyModel _model;
        private readonly EnemyView _view;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public EnemyModel Model => _model;
        public EnemyView View => _view;
        public bool IsActive => _model.IsActive;

        // ICollection<IDisposable> implementation for AddTo support
        public int Count => _disposables.Count;
        public bool IsReadOnly => false;

        public EnemyPresenter(EnemyModel model, EnemyView view)
        {
            _model = model;
            _view = view;
            
            SubscribeToModel();
            SubscribeToView();
        }

        private void SubscribeToModel()
        {
            _model.Health
                .Where(health => health <= 0)
                .Subscribe(_ => OnEnemyDeath())
                .AddTo(_disposables);

            _model.Position
                .Subscribe(position => _view.UpdatePosition(position))
                .AddTo(_disposables);

            _model.CurrentScale
                .Subscribe(scale => _view.UpdateScale(scale))
                .AddTo(_disposables);
        }

        private void SubscribeToView()
        {
            _view.OnDamageableCollision += OnDamageableCollision;
            _view.OnSpeedReduceCollision += OnSpeedReduceCollision;
            _view.OnEndGameCollision += OnEndGameCollision;
        }

        public void Initialize(Vector3 position, Vector3 targetPosition, EnemyConfig config)
        {
            _model.Initialize();
            _model.SetTargetPosition(targetPosition);
            _model.SetStartScale(_view.transform.localScale);
            _model.UpdatePosition(position);
            
            _view.Initialize(config);
            _view.transform.position = position;
            _view.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        public void UpdateLogic()
        {
            if (!_model.IsActive) return;

            _model.UpdateFireTimer(Time.deltaTime);
            _view.SetVelocityZero();
            
            Vector3 nextPosition = _model.CalculateNextPosition(_view.transform.position, Time.deltaTime);
            _model.UpdatePosition(nextPosition);
        }

        public void TakeDamage(int damage)
        {
            _model.TakeDamage(damage);
            _view.PlayHitEffect();
        }

        private void OnDamageableCollision(int damage)
        {
            _model.OnCollisionWithDamageable();
            TakeDamage(damage);
        }

        private void OnSpeedReduceCollision()
        {
            _model.ReduceSpeed();
        }

        private void OnEndGameCollision()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnEnemyDeath()
        {
            // This will be handled by the factory/pool system
        }

        public void ResetForPooling()
        {
            _model.ResetForPooling();
            _view.ResetForPooling();
        }

        // ICollection<IDisposable> implementation methods
        public void Add(IDisposable item)
        {
            _disposables.Add(item);
        }

        public void Clear()
        {
            _disposables.Clear();
        }

        public bool Contains(IDisposable item)
        {
            return _disposables.Contains(item);
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            _disposables.CopyTo(array, arrayIndex);
        }

        public bool Remove(IDisposable item)
        {
            return _disposables.Remove(item);
        }

        public System.Collections.Generic.IEnumerator<IDisposable> GetEnumerator()
        {
            return _disposables.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _disposables.GetEnumerator();
        }

        public void Dispose()
        {
            _view.OnDamageableCollision -= OnDamageableCollision;
            _view.OnSpeedReduceCollision -= OnSpeedReduceCollision;
            _view.OnEndGameCollision -= OnEndGameCollision;
            
            _disposables?.Dispose();
            _model?.Dispose();
        }
    }
}