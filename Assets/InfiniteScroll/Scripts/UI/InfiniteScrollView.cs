using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rabbit.UI
{
    public class InfiniteScrollView<T> : MonoBehaviour, IInfiniteScrollView
    {
        [SerializeField] private RectTransform listParent;

        // limits should follow declaration order:
        [SerializeField] private float topDisappearLimit;
        [SerializeField] private float topAppearLimit;
        [SerializeField] private float bottomAppearLimit;
        [SerializeField] private float bottomDisappearLimit;

        private readonly List<IInfiniteScrollViewElement> activeElements = new List<IInfiniteScrollViewElement>();

        private InfiniteSegmentedLinkedList<T> backingData;
        private IInfiniteListElementProvider listItemProvider;

        public RectTransform ParentRect => listParent;

        public void ScrollBy(float delta)
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
                    listItemProvider.Destroy(topElement);
                    activeElements.RemoveAt(0);
                    topElement = activeElements.FirstOrDefault();
                }

                // top element goes downward
                while (topElement.RectTransform.localPosition.y < topAppearLimit && topElement.ElementIndex > 0)
                {
                    var newTopElement = listItemProvider.Create();
                    newTopElement.ElementIndex = topElement.ElementIndex - 1;
                    newTopElement.UpdateDisplay(backingData);
                    newTopElement.RectTransform.localPosition = topElement.RectTransform.localPosition + new Vector3(x: 0, newTopElement.ElementHeight, z: 0);
                    activeElements.Insert(index: 0, newTopElement);

                    topElement = newTopElement;
                }

                // bottom goes outside
                while (bottomElement.RectTransform.localPosition.y < bottomDisappearLimit)
                {
                    listItemProvider.Destroy(bottomElement);
                    activeElements.RemoveAt(activeElements.Count - 1);
                    bottomElement = activeElements.LastOrDefault();
                }

                // bottom goes upwards
                while (bottomElement.RectTransform.localPosition.y > bottomAppearLimit && bottomElement.ElementIndex < backingData.Count)
                {
                    var newBottomElement = listItemProvider.Create();
                    newBottomElement.ElementIndex = bottomElement.ElementIndex + 1;
                    newBottomElement.UpdateDisplay(backingData);
                    newBottomElement.RectTransform.localPosition = bottomElement.RectTransform.localPosition - new Vector3(x: 0, newBottomElement.ElementHeight, z: 0);
                    activeElements.Add(newBottomElement);
                    bottomElement = newBottomElement;
                }
            }

            AdjustPositionsForSize();
        }

        protected virtual void Awake()
        {
            backingData = new InfiniteSegmentedLinkedList<T>(gameObject.GetComponent<ISegmentLoader<T>>());
            listItemProvider = GetComponent<IInfiniteListElementProvider>();

            var size = listParent.rect.size.y;

            var starterCount = size / listItemProvider.ElementHeight + 4;

            for (var i = 0; i < starterCount; i++)
            {
                var nextElement = listItemProvider.Create();
                nextElement.ElementIndex = i;
                nextElement.UpdateDisplay(backingData);
                activeElements.Add(nextElement);
            }

            AdjustPositionsForSize();
        }

        protected virtual void Update() => AdjustPositionsForSize();

        private void AdjustPositionsForSize()
        {
            var offset = 0f;
            var startPos = activeElements.First().RectTransform.localPosition;

            for (var i = 0; i < activeElements.Count; i++)
            {
                activeElements[i].RectTransform.localPosition = startPos + new Vector3(x: 0, -offset, z: 0);
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