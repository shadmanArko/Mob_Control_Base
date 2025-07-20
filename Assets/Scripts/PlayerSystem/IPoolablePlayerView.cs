using System;
using UnityEngine;

namespace PlayerSystem
{
    public interface IPoolablePlayerView : IPlayerView
    {
        bool IsActive { get; }
        void Activate(Vector3 position, Quaternion rotation);
        void Deactivate();
        IObservable<IPoolablePlayerView> OnReturnToPool { get; }
        void SetCloneSource(GameObject source);
    }
}