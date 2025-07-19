using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerSystem
{
    public class PlayerView : MonoBehaviour, IPoolablePlayerView
    {
        public Transform Transform => transform;
        public Vector3 Position => transform.position;
        public Vector3 StartScale { get; private set; }

        public IObservable<Collision> OnCollisionEntered => _onCollisionEntered.AsObservable();
        public IObservable<Collider> OnTriggerEntered => _onTriggerEntered.AsObservable();
        public IObservable<Unit> OnViewDestroyed => _onViewDestroyed.AsObservable();

        // IPoolablePlayerView Interface
        public bool IsActive { get; private set; }
        public IObservable<IPoolablePlayerView> OnReturnToPool => _onReturnToPool.AsObservable();

        // Private fields
        private Rigidbody _rigidbody;
        private readonly Subject<Collision> _onCollisionEntered = new Subject<Collision>();
        private readonly Subject<Collider> _onTriggerEntered = new Subject<Collider>();
        private readonly Subject<Unit> _onViewDestroyed = new Subject<Unit>();
        private readonly Subject<IPoolablePlayerView> _onReturnToPool = new Subject<IPoolablePlayerView>();

        [Inject] private DiContainer _container;
        private IPlayerPresenter _presenter;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            StartScale = transform.localScale;
            IsActive = false;
        }

        public void Initialize()
        {
            if (_presenter == null)
            {
                // Create a new presenter instance for this pooled object
                _presenter = _container.Instantiate<PlayerPresenter>();
                _presenter.SetView(this);
            }

            _presenter.Initialize();
            IsActive = true;
        }

        public void Activate(Vector3 position, Quaternion rotation)
        {
            gameObject.SetActive(true);
            transform.position = position;
            transform.rotation = rotation;
            transform.localScale = StartScale;

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;

            Initialize();
        }

        public void Deactivate()
        {
            IsActive = false;
            gameObject.SetActive(false);

            // Reset state but keep presenter for reuse
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }

        public void SetVelocity(Vector3 velocity)
        {
            _rigidbody.velocity = velocity;
        }

        public void DestroyView()
        {
            // Instead of destroying, return to pool
            _onReturnToPool.OnNext(this);
        }

        public void Cleanup()
        {
            // Cleanup for pooling - don't dispose presenter
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (IsActive)
                _onCollisionEntered.OnNext(collision);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsActive)
                _onTriggerEntered.OnNext(other);
        }
    }
}