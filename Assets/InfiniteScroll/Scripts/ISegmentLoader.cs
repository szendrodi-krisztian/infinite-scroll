namespace Rabbit.UI
{
    public interface ISegmentLoader<out T>
    {
        int TotalCount { get; }

        public void LoadElement(int index);
        public void AddConsumer(ISegmentConsumer<T> consumer);
    }
}