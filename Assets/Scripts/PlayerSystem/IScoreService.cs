using UniRx;

namespace PlayerSystem
{
    public interface IScoreService
    {
        void AddScore(int points);
        IReadOnlyReactiveProperty<int> CurrentScore { get; }
    }
}