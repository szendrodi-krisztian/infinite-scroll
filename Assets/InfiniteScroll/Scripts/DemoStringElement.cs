using TMPro;
using UnityEngine;

namespace Rabbit.UI
{
    public class DemoStringElement : InfiniteScrollViewElement<string>
    {
        [SerializeField] private TMP_Text label;

        public override void UpdateDisplay(InfiniteSegmentedLinkedList<string> data)
        {
            var future = data.ElementAt(elementIndex);
            future.WhenComplete(() => { label.text = future.Reference; });
        }
    }
}