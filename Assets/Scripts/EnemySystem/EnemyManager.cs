using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace DefaultNamespace.EnemySystem
{
    public class EnemyManager : MonoBehaviour, IDisposable
    {
        private readonly List<EnemyPresenter> _activeEnemies = new List<EnemyPresenter>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        
        [Inject] private IEnemyFactory _enemyFactory;

        private void Start()
        {
            // Update all active enemies every frame
            Observable.EveryUpdate()
                .Subscribe(_ => UpdateEnemies())
                .AddTo(_disposables);
        }

        private void UpdateEnemies()
        {
            // Update logic for all active enemies
            for (int i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                var enemy = _activeEnemies[i];
                
                if (enemy == null || !enemy.IsActive)
                {
                    _activeEnemies.RemoveAt(i);
                    continue;
                }
                
                enemy.UpdateLogic();
            }
        }

        public EnemyPresenter SpawnEnemy(Vector3 position, EnemyType enemyType)
        {
            var enemy = _enemyFactory.SpawnEnemy(position, enemyType);
            if (enemy != null)
            {
                _activeEnemies.Add(enemy);
            }
            return enemy;
        }

        public void RemoveEnemy(EnemyPresenter enemy)
        {
            _activeEnemies.Remove(enemy);
            _enemyFactory.ReturnEnemy(enemy);
        }

        public void ClearAllEnemies()
        {
            foreach (var enemy in _activeEnemies)
            {
                _enemyFactory.ReturnEnemy(enemy);
            }
            _activeEnemies.Clear();
        }

        public int ActiveEnemyCount => _activeEnemies.Count;

        public void Dispose()
        {
            ClearAllEnemies();
            _disposables?.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}