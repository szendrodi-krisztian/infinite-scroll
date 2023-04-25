using UnityEngine;

namespace Rabbit.UI
{
    public interface IInfiniteScrollView : IMonoBehaviour
    {
        RectTransform ParentRect { get; }
        void ScrollBy(float delta);
    }
}