namespace Rabbit.UI
{
    public interface ISegmentConsumer<in T>
    {
        public void OnSegmentLoading(int index);
        public void ConsumeSegment(int index, T nextLoadedElement);
    }
}