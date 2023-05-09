using Rabbit.DataStructure;

namespace Rabbit.Loaders
{
    public interface ISegmentLoader : IMonoBehaviour, IInvalidateable
    {
        int TotalCount { get; }
        void LoadElement(int index);
    }

    public interface ISegmentLoader<out T> : ISegmentLoader
    {
        public void AddConsumer(ISegmentConsumer<T> consumer);
        public void RemoveConsumer(ISegmentConsumer<T> consumer);
    }
}