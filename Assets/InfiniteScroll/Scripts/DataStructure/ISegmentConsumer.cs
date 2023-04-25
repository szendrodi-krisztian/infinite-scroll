namespace Rabbit.DataStructure
{
    public interface ISegmentConsumer<in T>
    {
        public int Count { get; }
        public void OnSegmentLoadStarted(int index);
        public void OnSegmentLoadFinished(int index, T nextLoadedElement);
    }
}