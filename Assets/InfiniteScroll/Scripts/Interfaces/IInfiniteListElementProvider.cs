namespace Rabbit.UI
{
    public interface IInfiniteListElementProvider
    {
        public float ElementHeight { get; }
        public IInfiniteScrollViewElement Create();
        public void Destroy(IInfiniteScrollViewElement element);
    }
}