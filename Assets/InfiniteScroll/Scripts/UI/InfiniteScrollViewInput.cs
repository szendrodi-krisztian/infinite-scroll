using Rabbit.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rabbit.UI
{
    public sealed class InfiniteScrollViewInput : MonoBehaviour, IScrollHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IMonoBehaviour
    {
        [SerializeField] private VelocitySampler velocitySampler;

        private Canvas rootCanvas;
        private float scaleFactor;
        private IInfiniteScrollView scrollViewCore;

        public void OnBeginDrag(PointerEventData eventData) { }

        public void OnDrag(PointerEventData eventData)
        {
            var dragDelta = eventData.delta.y * scaleFactor;
            scrollViewCore.ScrollBy(dragDelta);
            velocitySampler.PushSample(dragDelta / Time.unscaledDeltaTime);
        }

        public void OnEndDrag(PointerEventData eventData) { }

        public void OnScroll(PointerEventData eventData) => scrollViewCore.ScrollBy(-eventData.scrollDelta.y);

        private void Awake()
        {
            scrollViewCore = GetComponent<IInfiniteScrollView>();
            rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
            RefreshScaleFactor();
        }
        private void Start() => RefreshScaleFactor();
        private void Update()
        {
            RefreshScaleFactor();

            if (!Input.GetMouseButton(0))
            {
                scrollViewCore.ScrollBy(velocitySampler.GetSample() * Time.deltaTime);
            }
        }

        private void RefreshScaleFactor() => scaleFactor = rootCanvas.transform.localScale.x * (1080f / Screen.height);
    }
}