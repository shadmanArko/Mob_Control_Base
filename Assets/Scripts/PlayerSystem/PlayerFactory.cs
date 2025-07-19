using System;
using ObjectPoolSystem;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerSystem
{
    public class PlayerFactory : IPlayerFactory, IDisposable
{
    private readonly DiContainer _container;
    private readonly GameObject _playerPrefab;
    private readonly Transform _poolParent;
    private readonly IObjectPool<IPoolablePlayerView> _playerPool;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    
    public int ActivePlayerCount => _playerPool.ActiveCount;
    public int InactivePlayerCount => _playerPool.InactiveCount;
    
    public PlayerFactory(
        DiContainer container,
        GameObject playerPrefab,
        Transform poolParent = null)
    {
        _container = container;
        _playerPrefab = playerPrefab;
        _poolParent = poolParent;
        
        _playerPool = new ObjectPool<IPoolablePlayerView>(
            createFunc: CreateNewPlayer,
            onGet: OnGetPlayer,
            onReturn: OnReturnPlayer,
            onDestroy: OnDestroyPlayer,
            maxSize: 1000
        );
    }
    
    public IPoolablePlayerView CreatePlayer(Vector3 position, Quaternion rotation)
    {
        var player = _playerPool.Get();
        player.Activate(position, rotation);
        
        // Subscribe to return event
        player.OnReturnToPool
            .Take(1) // Only take the first return event
            .Subscribe(p => ReturnPlayer(p))
            .AddTo(_disposables);
        
        return player;
    }
    
    public void ReturnPlayer(IPoolablePlayerView player)
    {
        if (player != null && player.IsActive)
        {
            _playerPool.Return(player);
        }
    }
    
    public void PreWarmPool(int count)
    {
        _playerPool.PreWarm(count);
    }
    
    private IPoolablePlayerView CreateNewPlayer()
    {
        var playerGO = _container.InstantiatePrefab(_playerPrefab, _poolParent);
        var poolablePlayer = playerGO.GetComponent<IPoolablePlayerView>();
        
        if (poolablePlayer == null)
        {
            Debug.LogError("Player prefab must have a component implementing IPoolablePlayerView");
            return null;
        }
        
        // Initially deactivate
        playerGO.SetActive(false);
        
        return poolablePlayer;
    }
    
    private void OnGetPlayer(IPoolablePlayerView player)
    {
        // Player is being retrieved from pool
        // Additional setup if needed
    }
    
    private void OnReturnPlayer(IPoolablePlayerView player)
    {
        // Player is being returned to pool
        player.Deactivate();
    }
    
    private void OnDestroyPlayer(IPoolablePlayerView player)
    {
        // Pool is full, actually destroy the player
        if (player != null && player.Transform != null)
        {
            UnityEngine.Object.Destroy(player.Transform.gameObject);
        }
    }
    
    public void Dispose()
    {
        _playerPool?.Clear();
        _disposables?.Dispose();
    }
}
}