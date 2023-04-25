using UnityEngine;

namespace Rabbit.UI
{
    public interface IInfiniteScrollView
    {
        RectTransform ParentRect { get; }
        void ScrollBy(float delta);
    }
}