using UniRx;

namespace PlayerSystem
{
    public class ScoreService : IScoreService
    {
        private readonly ReactiveProperty<int> _currentScore = new ReactiveProperty<int>(0);
    
        public IReadOnlyReactiveProperty<int> CurrentScore => _currentScore;
    
        public void AddScore(int points)
        {
            _currentScore.Value += points;
        }
    }
}