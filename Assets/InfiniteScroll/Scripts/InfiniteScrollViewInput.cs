using UnityEngine;
using UnityEngine.EventSystems;

namespace Rabbit.UI
{
    public sealed class InfiniteScrollViewInput : MonoBehaviour, IScrollHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        private float scaleFactor;
        private IInfiniteScrollView scrollViewCore;

        public void OnBeginDrag(PointerEventData eventData) { }

        public void OnDrag(PointerEventData eventData)
        {
            var dragDelta = eventData.delta.y * scaleFactor;
            scrollViewCore.ScrollBy(dragDelta);
        }

        public void OnEndDrag(PointerEventData eventData) { }

        public void OnScroll(PointerEventData eventData) => scrollViewCore.ScrollBy(-eventData.scrollDelta.y);

        private void Awake()
        {
            scrollViewCore = GetComponent<IInfiniteScrollView>();
            RefreshScaleFactor();
        }
        private void Start() => RefreshScaleFactor();
        private void Update() => RefreshScaleFactor();

        private void RefreshScaleFactor() => scaleFactor = GetComponentInParent<Canvas>().rootCanvas.transform.localScale.x * (1080f / Screen.height);
    }
}