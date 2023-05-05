using Rabbit.Loaders;
using UnityEngine;

namespace Rabbit.UI
{
    public interface IInfiniteScrollView : IMonoBehaviour, IInvalidateable
    {
        RectTransform ParentRect { get; }
        void ScrollBy(float delta);
    }
}