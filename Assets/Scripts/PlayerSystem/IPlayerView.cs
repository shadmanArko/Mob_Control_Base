using System;
using UniRx;
using UnityEngine;

namespace PlayerSystem
{
    public interface IPlayerView
    {
        // Properties for Presenter
        Transform Transform { get; }
        Vector3 Position { get; }
        Vector3 StartScale { get; }
    
        // Events from View to Presenter (Input/Physics only)
        IObservable<Collision> OnCollisionEntered { get; }
        IObservable<Collider> OnTriggerEntered { get; }
        IObservable<Unit> OnViewDestroyed { get; }
    
        // Rendering Commands from Presenter
        void SetPosition(Vector3 position);
        void SetScale(Vector3 scale);
        void SetVelocity(Vector3 velocity);
        void DestroyView();
    
        // Lifecycle
        void Initialize();
        void Cleanup();
    }
}