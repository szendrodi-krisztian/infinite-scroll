using Rabbit.DataStructure;
using Rabbit.UI;
using TMPro;
using UnityEngine;

namespace Rabbit.Demo
{
    public sealed class DemoStringElementBase : InfiniteScrollViewElementBase
    {
        [SerializeField] private TMP_Text label;

        private void UpdateUI<T>(T element) => label.text = element.ToString();

        public override void UpdateDisplay(IDataSource data) => data.GetItem<string>(elementIndex).WhenComplete(UpdateUI);
    }
}