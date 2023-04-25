using Rabbit.DataStructure;
using UnityEngine;

namespace Rabbit.UI
{
    public interface IInfiniteScrollViewElement : IMonoBehaviour
    {
        float ElementHeight { get; }
        int ElementIndex { get; set; }
        RectTransform RectTransform { get; }
        void OnPoolGet();
        void OnPoolRelease();
        void MoveBy(float delta);
        void UpdateDisplay(IDataSource data);
    }
}