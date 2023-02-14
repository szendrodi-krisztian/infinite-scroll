using TMPro;
using UnityEngine;

namespace Rabbit.UI
{
    public class InfiniteScrollViewElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private float elementHeight;
        [SerializeField] private int elementIndex;

        public RectTransform RectTransform { get; private set; }

        public float ElementHeight => Mathf.Max(elementHeight, 1);

        public int ElementIndex
        {
            get => elementIndex;
            set => elementIndex = value;
        }

        private void Awake()
        {
            RectTransform = (RectTransform)transform;
        }

        public void OnPoolGet()
        {
            gameObject.SetActive(true);
        }

        public void OnPoolRelease()
        {
            gameObject.SetActive(false);
        }

        public void MoveBy(float delta)
        {
            RectTransform.position += new Vector3(0, delta, 0);
        }

        public void UpdateDisplay(InfiniteSegmentedLinkedList<int> data)
        {
            label.text = $"dynamic: index:{elementIndex}";
        }
    }
}