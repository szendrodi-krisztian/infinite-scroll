using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Knife
{
    public class CustomLevelsFiltering : MonoBehaviour
    {
        [SerializeField] private CustomLevelPageLoader loader;
        [SerializeField] private LayoutElement filteringParent;

        [SerializeField] private Vector2 closedPreferredSize;
        [SerializeField] private Vector2 openedPreferredSize;

        private bool isOpened;

        public void ToggleFilterSetup()
        {
            isOpened = !isOpened;

            var targetSize = isOpened ? openedPreferredSize : closedPreferredSize;

            filteringParent.DOKill();
            filteringParent.DOPreferredSize(targetSize, 0.135f).SetEase(Ease.OutQuad).SetUpdate(true);
        }

        public void AddFilter(Filter filter)
        {
            loader.AddFilter(filter);
        }

        public void AddSorter(Sorter sorter)
        {
            loader.AddSorter(sorter);
        }
    }
}