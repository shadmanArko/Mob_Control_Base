using UnityEngine;

namespace PlayerSystem
{
    public interface IPlayerModel
    {
        void TakeDamage(int damageAmount);
        void SetCheckPointReached(bool reached);
        void SetTarget(Vector3 target);
        void SetReducedSpeed();
        void ResetSpeed();
        bool IsAlive();
        float HealthPercentage();
    }
}