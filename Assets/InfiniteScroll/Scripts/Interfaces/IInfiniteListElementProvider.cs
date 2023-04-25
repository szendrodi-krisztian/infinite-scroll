namespace Rabbit.UI
{
    public interface IInfiniteListElementProvider : IMonoBehaviour
    {
        public float ElementHeight { get; }
        public IInfiniteScrollViewElement Create();
        public void Destroy(IInfiniteScrollViewElement element);
    }
}