using UnityEngine;

namespace Rabbit.UI
{
    public class RectUpdateListener : MonoBehaviour
    {
        [SerializeField] private InfiniteScrollView parent;

        private void OnRectTransformDimensionsChange()
        {
            parent.ScrollBy(0);
        }
    }
}