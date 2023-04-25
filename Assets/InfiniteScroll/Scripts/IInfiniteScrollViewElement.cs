using UnityEngine;

namespace Rabbit.UI
{
    public interface IInfiniteScrollViewElement
    {
        float ElementHeight { get; }
        int ElementIndex { get; set; }
        RectTransform RectTransform { get; }
        void OnPoolGet();
        void OnPoolRelease();
        void MoveBy(float delta);
        void UpdateDisplay<T>(InfiniteSegmentedLinkedList<T> data);
    }
}