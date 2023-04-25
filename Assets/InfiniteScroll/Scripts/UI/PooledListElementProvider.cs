using UnityEngine;
using UnityEngine.Pool;

namespace Rabbit.UI
{
    public sealed class PooledListElementProvider : MonoBehaviour, IInfiniteListElementProvider
    {
        [SerializeField] private GameObject elementPrefab;

        private ObjectPool<IInfiniteScrollViewElement> pool;
        private IInfiniteScrollView scrollViewCore;

        public float ElementHeight => elementPrefab.GetComponent<IInfiniteScrollViewElement>().ElementHeight;

        public IInfiniteScrollViewElement Create() => pool.Get();
        public void Destroy(IInfiniteScrollViewElement element) => pool.Release(element);

        private void Awake()
        {
            scrollViewCore = GetComponent<IInfiniteScrollView>();
            pool = new ObjectPool<IInfiniteScrollViewElement>(OnPoolCreate, OnPoolGet, OnPoolRelease, OnPoolDestroy, collectionCheck: true, defaultCapacity: 20);
        }

        private static void OnPoolDestroy(IInfiniteScrollViewElement e) => Destroy(((MonoBehaviour) e).gameObject);
        private static void OnPoolRelease(IInfiniteScrollViewElement e) => e.OnPoolRelease();
        private static void OnPoolGet(IInfiniteScrollViewElement e) => e.OnPoolGet();
        private IInfiniteScrollViewElement OnPoolCreate() => Instantiate(elementPrefab, scrollViewCore.ParentRect).GetComponent<IInfiniteScrollViewElement>();
    }
}