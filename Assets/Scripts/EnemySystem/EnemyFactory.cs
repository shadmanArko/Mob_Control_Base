using UnityEngine;
using ObjectPoolSystem;
using Zenject;
using System.Collections.Generic;
using System;
using UniRx;
using Object = UnityEngine.Object;

namespace DefaultNamespace.EnemySystem
{
    public interface IEnemyFactory
    {
        EnemyPresenter SpawnEnemy(Vector3 position, EnemyType enemyType);
        void ReturnEnemy(EnemyPresenter enemy);
        void PrewarmPool(EnemyType enemyType, int count);
        void ClearPools();
    }

    public enum EnemyType
    {
        Normal,
        Big
    }

    public class EnemyFactory : IEnemyFactory, IDisposable
    {
        private readonly GameObject _normalEnemyPrefab;
        private readonly GameObject _bigEnemyPrefab;
        private readonly EnemyConfig _normalEnemyConfig;
        private readonly EnemyConfig _bigEnemyConfig;
        private readonly Transform _poolParent;
        private readonly EnemyModel.Factory _modelFactory;
        private readonly DiContainer _container;

        private readonly Dictionary<EnemyType, IObjectPool<EnemyPresenter>> _enemyPools;
        private Vector3 _targetPosition;

        public EnemyFactory(
            GameObject normalEnemyPrefab,
            GameObject bigEnemyPrefab,
            EnemyConfig normalEnemyConfig,
            EnemyConfig bigEnemyConfig,
            Transform poolParent,
            EnemyModel.Factory modelFactory,
            DiContainer container)
        {
            _normalEnemyPrefab = normalEnemyPrefab;
            _bigEnemyPrefab = bigEnemyPrefab;
            _normalEnemyConfig = normalEnemyConfig;
            _bigEnemyConfig = bigEnemyConfig;
            _poolParent = poolParent;
            _modelFactory = modelFactory;
            _container = container;

            _enemyPools = new Dictionary<EnemyType, IObjectPool<EnemyPresenter>>();
            
            InitializePools();
            SetTargetPosition();
        }

        private void InitializePools()
        {
            _enemyPools[EnemyType.Normal] = new ObjectPool<EnemyPresenter>(
                () => CreateEnemy(EnemyType.Normal),
                OnGetFromPool,
                OnReturnToPool,
                OnDestroyPoolObject,
                maxSize: 100
            );

            _enemyPools[EnemyType.Big] = new ObjectPool<EnemyPresenter>(
                () => CreateEnemy(EnemyType.Big),
                OnGetFromPool,
                OnReturnToPool,
                OnDestroyPoolObject,
                maxSize: 50
            );
        }

        private void SetTargetPosition()
        {
            var playerCastle = GameObject.FindGameObjectWithTag("PlayerCastle");
            if (playerCastle != null)
            {
                _targetPosition = playerCastle.transform.position;
                _targetPosition.y = 0; // Keep enemies on ground level
            }
        }

        private EnemyPresenter CreateEnemy(EnemyType enemyType)
        {
            GameObject prefab = enemyType == EnemyType.Normal ? _normalEnemyPrefab : _bigEnemyPrefab;
            EnemyConfig config = enemyType == EnemyType.Normal ? _normalEnemyConfig : _bigEnemyConfig;

            GameObject enemyObject = Object.Instantiate(prefab, _poolParent);
            EnemyView view = enemyObject.GetComponent<EnemyView>();
            
            if (view == null)
            {
                view = enemyObject.AddComponent<EnemyView>();
            }

            EnemyModel model = _modelFactory.Create(config);
            EnemyPresenter presenter = new EnemyPresenter(model, view);

            // Subscribe to death event for automatic pooling
            model.IsDead
                .Where(isDead => isDead)
                .Subscribe(_ => ReturnEnemy(presenter))
                .AddTo(presenter);

            return presenter;
        }

        public EnemyPresenter SpawnEnemy(Vector3 position, EnemyType enemyType)
        {
            if (!_enemyPools.ContainsKey(enemyType))
                return null;

            var enemy = _enemyPools[enemyType].Get();
            EnemyConfig config = enemyType == EnemyType.Normal ? _normalEnemyConfig : _bigEnemyConfig;
            
            enemy.Initialize(position, _targetPosition, config);
            
            return enemy;
        }

        public void ReturnEnemy(EnemyPresenter enemy)
        {
            if (enemy == null || !enemy.IsActive) return;

            EnemyType type = enemy.Model.IsBig ? EnemyType.Big : EnemyType.Normal;
            
            if (_enemyPools.ContainsKey(type))
            {
                _enemyPools[type].Return(enemy);
            }
        }

        public void PrewarmPool(EnemyType enemyType, int count)
        {
            if (_enemyPools.ContainsKey(enemyType))
            {
                _enemyPools[enemyType].PreWarm(count);
            }
        }

        public void ClearPools()
        {
            foreach (var pool in _enemyPools.Values)
            {
                pool.Clear();
            }
        }

        private void OnGetFromPool(EnemyPresenter enemy)
        {
            enemy.View.SetActive(true);
        }

        private void OnReturnToPool(EnemyPresenter enemy)
        {
            enemy.ResetForPooling();
        }

        private void OnDestroyPoolObject(EnemyPresenter enemy)
        {
            enemy.Dispose();
            if (enemy.View != null && enemy.View.gameObject != null)
            {
                Object.Destroy(enemy.View.gameObject);
            }
        }

        public void Dispose()
        {
            ClearPools();
            _enemyPools.Clear();
        }
    }
}