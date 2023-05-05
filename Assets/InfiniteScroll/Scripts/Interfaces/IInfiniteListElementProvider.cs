using Rabbit.Loaders;

namespace Rabbit.UI
{
    public interface IInfiniteListElementProvider : IMonoBehaviour, IInvalidateable
    {
        public float ElementHeight { get; }
        public IInfiniteScrollViewElement Create();
        public void Destroy(IInfiniteScrollViewElement element);
    }
}