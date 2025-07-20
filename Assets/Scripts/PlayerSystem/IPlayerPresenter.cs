using UnityEngine;

namespace PlayerSystem
{
    public interface IPlayerPresenter
    {
        void SetView(IPlayerView view);
        void Initialize();
        void TakeDamage(int damage);
        int DoDamage();
        GameObject GetCloneSource();
        void SetCloneSource(GameObject source);
    }
}