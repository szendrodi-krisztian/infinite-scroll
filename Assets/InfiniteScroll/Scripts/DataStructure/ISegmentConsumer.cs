namespace Rabbit.UI
{
    public interface ISegmentConsumer<in T>
    {
        public void OnSegmentLoadStarted(int index);
        public void OnSegmentLoadFinished(int index, T nextLoadedElement);
    }
}