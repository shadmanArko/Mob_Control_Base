namespace ObjectPoolSystem
{
    public interface IObjectPool<T> where T : class
    {
        T Get();
        void Return(T item);
        void PreWarm(int count);
        void Clear();
        int ActiveCount { get; }
        int InactiveCount { get; }
    }
}