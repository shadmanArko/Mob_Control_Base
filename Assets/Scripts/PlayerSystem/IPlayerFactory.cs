using UnityEngine;

namespace PlayerSystem
{
    public interface IPlayerFactory
    {
        IPoolablePlayerView CreatePlayer(Vector3 position, Quaternion rotation);
        void ReturnPlayer(IPoolablePlayerView player);
        void PreWarmPool(int count);
        int ActivePlayerCount { get; }
        int InactivePlayerCount { get; }
    }
}