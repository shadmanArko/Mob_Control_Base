namespace PlayerSystem
{
    public interface IPlayerPresenter
    {
        void SetView(IPlayerView view);
        void Initialize();
        void TakeDamage(int damage);
    }
}