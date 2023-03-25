using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Rabbit.UI
{
    public class InfiniteScrollView<T> : MonoBehaviour, IScrollHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private InfiniteScrollViewElement<T> elementPrefab;
        [SerializeField] private RectTransform listParent;

        // limits should follow declaration order:
        [SerializeField] private float topDisappearLimit;
        [SerializeField] private float topAppearLimit;
        [SerializeField] private float bottomAppearLimit;
        [SerializeField] private float bottomDisappearLimit;

        private float scaleFactor;

        [SerializeField] private List<InfiniteScrollViewElement<T>> activeElements;

        private ObjectPool<InfiniteScrollViewElement<T>> pool;

        private InfiniteSegmentedLinkedList<T> data;

        protected virtual void Awake()
        {
            scaleFactor = GetComponentInParent<Canvas>().rootCanvas.transform.localScale.x * (1080f / Screen.height);
            data = new InfiniteSegmentedLinkedList<T>(gameObject.GetComponent<ISegmentLoader<T>>());
            pool = new ObjectPool<InfiniteScrollViewElement<T>>(
                createFunc: () => Instantiate(elementPrefab, listParent),
                actionOnGet: e => e.OnPoolGet(),
                actionOnRelease: e => e.OnPoolRelease(),
                actionOnDestroy: e => Destroy(e.gameObject),
                true, 10, 1000);

            var size = listParent.rect.size.y;

            var starterCount = (size / elementPrefab.ElementHeight) + 4;

            for (var i = 0; i < starterCount; i++)
            {
                var nextElement = pool.Get();
                nextElement.ElementIndex = i;
                nextElement.UpdateDisplay(data);
                activeElements.Add(nextElement);
            }

            AdjustPositionsForSize();
        }

        protected void Start()
        {
            scaleFactor = GetComponentInParent<Canvas>().rootCanvas.transform.localScale.x * (1080f / Screen.height);
        }

        private void Update()
        {
            AdjustPositionsForSize();
            scaleFactor = GetComponentInParent<Canvas>().rootCanvas.transform.localScale.x * (1080f / Screen.height);
        }

        private void AdjustPositionsForSize()
        {
            var offset = 0f;
            var startPos = activeElements.First().RectTransform.localPosition;

            for (var i = 0; i < activeElements.Count; i++)
            {
                activeElements[i].RectTransform.localPosition = startPos + new Vector3(0, -offset, 0);
                offset += activeElements[i].ElementHeight;
            }
        }

        private void MoveAllElementsBy(float delta)
        {
            for (var i = 0; i < activeElements.Count; i++)
            {
                activeElements[i].MoveBy(delta);
            }
        }

        public void OnScroll(PointerEventData eventData)
        {
            ScrollBy(-eventData.scrollDelta.y);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            // this should be also called, when an element is resized!
            var dragDelta = eventData.delta.y * scaleFactor;

            ScrollBy(dragDelta);
        }

        private void ScrollBy(float delta)
        {
            // subdivide dragDelta so only a single element can change its visibility in a single frame!
            const float maxStepSize = 10f;


            var dragDelta = ClampDragDelta(delta);


            var stepCount = Mathf.CeilToInt(Mathf.Abs(dragDelta / maxStepSize));
            var stepSize = dragDelta / stepCount;

            for (var i = 0; i < stepCount; i++)
            {
                var topElement = activeElements.FirstOrDefault();
                var bottomElement = activeElements.LastOrDefault();


                MoveAllElementsBy(stepSize);

                // top element goes outside
                while (topElement.RectTransform.localPosition.y > topDisappearLimit)
                {
                    pool.Release(topElement);
                    activeElements.RemoveAt(0);
                    topElement = activeElements.FirstOrDefault();
                }

                // top element goes downward
                while (topElement.RectTransform.localPosition.y < topAppearLimit && topElement.ElementIndex > 0)
                {
                    var newTopElement = pool.Get();
                    newTopElement.ElementIndex = topElement.ElementIndex - 1;
                    newTopElement.UpdateDisplay(data);
                    newTopElement.RectTransform.localPosition = topElement.RectTransform.localPosition +
                                                                new Vector3(0, newTopElement.ElementHeight, 0);
                    activeElements.Insert(0, newTopElement);

                    topElement = newTopElement;
                }

                // bottom goes outside
                while (bottomElement.RectTransform.localPosition.y < bottomDisappearLimit)
                {
                    pool.Release(bottomElement);
                    activeElements.RemoveAt(activeElements.Count - 1);
                    bottomElement = activeElements.LastOrDefault();
                }

                // bottom goes upwards
                while (bottomElement.RectTransform.localPosition.y > bottomAppearLimit &&
                       bottomElement.ElementIndex < data.Count)
                {
                    var newBottomElement = pool.Get();
                    newBottomElement.ElementIndex = bottomElement.ElementIndex + 1;
                    newBottomElement.UpdateDisplay(data);
                    newBottomElement.RectTransform.localPosition = bottomElement.RectTransform.localPosition -
                                                                   new Vector3(0, newBottomElement.ElementHeight, 0);
                    activeElements.Add(newBottomElement);
                    bottomElement = newBottomElement;
                }
            }

            AdjustPositionsForSize();
        }

        private float ClampDragDelta(float delta)
        {
            if (delta < 0)
            {
                var topElement = activeElements.FirstOrDefault();

                var y = topElement.RectTransform.localPosition.y;
                if (y + delta < 0)
                    return 0;
            }
            else if (delta > 0)
            {
                var bottomElement = activeElements.LastOrDefault();

                var y = bottomElement.RectTransform.localPosition.y;
                Debug.Log($"CLAMP: y:{y} delta: {delta} limit {listParent.rect.size.y}");
                if (y + delta > -listParent.rect.size.y + bottomElement.ElementHeight)
                    return 0;
            }

            return delta;
        }
    }
}