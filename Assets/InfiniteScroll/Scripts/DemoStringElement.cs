using TMPro;
using UnityEngine;

namespace Rabbit.UI
{
    public sealed class DemoStringElement : InfiniteScrollViewElement
    {
        [SerializeField] private TMP_Text label;

        public override void UpdateDisplay<T>(InfiniteSegmentedLinkedList<T> data) => data[elementIndex].WhenComplete(element => label.text = element.ToString());
    }
}