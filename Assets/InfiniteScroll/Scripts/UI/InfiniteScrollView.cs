using System.Collections.Generic;
using System.Linq;
using Rabbit.DataStructure;
using UnityEngine;

namespace Rabbit.UI
{
    public class InfiniteScrollView : MonoBehaviour, IInfiniteScrollView
    {
        [SerializeField] private RectTransform listParent;

        // limits should follow declaration order:
        [SerializeField] private float topDisappearLimit;
        [SerializeField] private float topAppearLimit;
        [SerializeField] private float bottomAppearLimit;
        [SerializeField] private float bottomDisappearLimit;

        private readonly List<IInfiniteScrollViewElement> activeElements = new();
        private IDataSource dataSource;
        private IInfiniteListElementProvider listItemProvider;

        protected virtual void Awake()
        {
            listItemProvider = GetComponent<IInfiniteListElementProvider>();
            dataSource = GetComponent<IDataSource>();
            dataSource.Initialize();

            InitStarterElements();
        }

        protected virtual void Update() => AdjustPositionsForSize();

        public RectTransform ParentRect => listParent;

        public void ScrollBy(float delta)
        {
            if (dataSource == null) return;
            if (Mathf.Abs(delta) <= 0.02f) return;

            // subdivide dragDelta so only a single element can change its visibility in a single frame!
            const float maxStepSize = 80f;

            var dragDelta = ClampDragDelta(delta);
            if (Mathf.Abs(dragDelta) <= 0.01f) return;

            var stepCount = Mathf.CeilToInt(Mathf.Abs(dragDelta / maxStepSize));
            var stepSize = dragDelta / stepCount;

            for (var i = 0; i < stepCount; i++)
            {
                var topElement = activeElements.FirstOrDefault();
                var bottomElement = activeElements.LastOrDefault();

                MoveAllElementsBy(ClampDragDelta(stepSize));

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
                    newTopElement.UpdateDisplay(dataSource);
                    newTopElement.RectTransform.localPosition = topElement.RectTransform.localPosition + new Vector3(0, newTopElement.ElementHeight, 0);
                    activeElements.Insert(0, newTopElement);

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
                while (bottomElement.RectTransform.localPosition.y > bottomAppearLimit && bottomElement.ElementIndex < dataSource.Count)
                {
                    var newBottomElement = listItemProvider.Create();
                    newBottomElement.ElementIndex = bottomElement.ElementIndex + 1;
                    newBottomElement.UpdateDisplay(dataSource);
                    newBottomElement.RectTransform.localPosition = bottomElement.RectTransform.localPosition - new Vector3(0, newBottomElement.ElementHeight, 0);
                    activeElements.Add(newBottomElement);
                    bottomElement = newBottomElement;
                }
            }

            AdjustPositionsForSize();
        }

        public void Invalidate()
        {
            activeElements.Clear();
            InitStarterElements();
        }

        private void InitStarterElements()
        {
            var size = listParent.rect.size.y;

            var starterCount = size / listItemProvider.ElementHeight + 10;

            for (var i = 0; i < starterCount; i++)
            {
                var nextElement = listItemProvider.Create();
                nextElement.ElementIndex = i;
                nextElement.UpdateDisplay(dataSource);
                activeElements.Add(nextElement);
            }

            AdjustPositionsForSize();
        }

        private void AdjustPositionsForSize()
        {
            var first = activeElements.First();
            var startPos = first.RectTransform.localPosition;
            if (first.ElementIndex == 0 && startPos.y < 0)
            {
                startPos.y = 0;
            }

            UpdateAllRectPositionWithFirstAt(startPos);

            var last = activeElements.Last();
            if (last.ElementIndex == dataSource.Count)
            {
                var y = last.RectTransform.localPosition.y;
                var a = -listParent.rect.size.y + last.ElementHeight;
                if (y > a)
                {
                    var overScroll = y - a;
                    UpdateAllRectPositionWithFirstAt(startPos - new Vector3(0, overScroll, 0));
                }
            }
        }
        private void UpdateAllRectPositionWithFirstAt(Vector3 startPos)
        {
            var offset = 0f;
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

                if (y + delta > -listParent.rect.size.y + bottomElement.ElementHeight)
                    return 0;
            }

            return delta;
        }
    }
}