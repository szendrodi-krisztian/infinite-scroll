using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;

namespace Rabbit.UI
{
    public class InfiniteScrollView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler,
        IScrollHandler
    {
        [SerializeField] private InfiniteScrollViewElement elementPrefab;
        [SerializeField] private RectTransform listParent;

        // limits should follow declaration order:
        [SerializeField] private float topDisappearLimit;
        [SerializeField] private float topAppearLimit;
        [SerializeField] private float bottomAppearLimit;
        [SerializeField] private float bottomDisappearLimit;

        [SerializeField] private List<InfiniteScrollViewElement> activeElements;

        private ObjectPool<InfiniteScrollViewElement> pool;

        private InfiniteSegmentedLinkedList<int> data;

        [SerializeField] private int pressedPointerCount;
        private bool isDragging => pressedPointerCount == 1;

        private void Awake()
        {
            data = new InfiniteSegmentedLinkedList<int>(gameObject.AddComponent<MockAsyncSegmentLoader>());
            pool = new ObjectPool<InfiniteScrollViewElement>(
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

        public void OnPointerDown(PointerEventData eventData)
        {
            pressedPointerCount++;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pressedPointerCount--;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (!isDragging) return;

            // this should be also called, when an element is resized!

            var dragDelta = eventData.delta.y;

            // subdivide dragDelta so only a single element can change its visibility in a single frame!
            const float maxStepSize = 20f;

            var stepCount = Mathf.CeilToInt(Mathf.Abs(dragDelta / maxStepSize));
            var stepSize = dragDelta / stepCount;

            for (var i = 0; i < stepCount; i++)
            {
                MoveAllElementsBy(stepSize);

                var topElement = activeElements.FirstOrDefault();
                var bottomElement = activeElements.LastOrDefault();

                // top element goes outside
                if (topElement.RectTransform.localPosition.y > topDisappearLimit)
                {
                    pool.Release(topElement);
                    activeElements.RemoveAt(0);
                }

                // top element goes downward
                if (topElement.RectTransform.localPosition.y < topAppearLimit && topElement.ElementIndex > 0)
                {
                    var newTopElement = pool.Get();
                    newTopElement.ElementIndex = topElement.ElementIndex - 1;
                    newTopElement.UpdateDisplay(data);
                    newTopElement.RectTransform.anchoredPosition = topElement.RectTransform.anchoredPosition +
                                                                   new Vector2(0, newTopElement.ElementHeight);
                    activeElements.Insert(0, newTopElement);
                }

                // bottom goes outside
                if (bottomElement.RectTransform.localPosition.y < bottomDisappearLimit)
                {
                    pool.Release(bottomElement);
                    activeElements.RemoveAt(activeElements.Count - 1);
                }

                // bottom goes upwards
                if (bottomElement.RectTransform.localPosition.y > bottomAppearLimit && bottomElement.ElementIndex < data.Count)
                {
                    var newBottomElement = pool.Get();
                    newBottomElement.ElementIndex = bottomElement.ElementIndex + 1;
                    newBottomElement.UpdateDisplay(data);
                    newBottomElement.RectTransform.anchoredPosition = bottomElement.RectTransform.anchoredPosition -
                                                                      new Vector2(0, newBottomElement.ElementHeight);
                    activeElements.Add(newBottomElement);
                }
            }

            AdjustPositionsForSize();
        }

        private void Update()
        {
            AdjustPositionsForSize();
        }

        private void AdjustPositionsForSize()
        {
            var offset = 0f;
            var startPos = activeElements.First().RectTransform.anchoredPosition;

            for (var i = 0; i < activeElements.Count; i++)
            {
                activeElements[i].RectTransform.anchoredPosition = startPos + new Vector2(0, -offset);
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
        }
    }
}