namespace Rabbit.DataStructure
{
    public interface ISegmentConsumer<in T>
    {
        public void OnSegmentLoadStarted(int index);
        public void OnSegmentLoadFinished(int index, T nextLoadedElement);
    }
}