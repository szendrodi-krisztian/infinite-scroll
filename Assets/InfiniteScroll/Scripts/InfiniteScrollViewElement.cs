using UnityEngine;

namespace Rabbit.UI
{
    public abstract class InfiniteScrollViewElement<T> : MonoBehaviour
    {
        [SerializeField] protected float elementHeight;
        [SerializeField] protected int elementIndex;

        public RectTransform RectTransform { get; private set; }

        public float ElementHeight => Mathf.Max(elementHeight, b: 1);

        public int ElementIndex
        {
            get => elementIndex;
            set => elementIndex = value;
        }

        protected virtual void Awake() => RectTransform = (RectTransform) transform;

        public virtual void OnPoolGet() => gameObject.SetActive(true);

        public virtual void OnPoolRelease() => gameObject.SetActive(false);

        public void MoveBy(float delta) => RectTransform.position += new Vector3(x: 0, delta, z: 0);

        public virtual void UpdateDisplay(InfiniteSegmentedLinkedList<T> data) { }

        public float ClampStepSizeTop(float value)
        {
            return value;
            return RectTransform.localPosition.y + value > 0 ? value : -RectTransform.localPosition.y;
        }

        public float ClampStepSizeBottom(float value) => RectTransform.localPosition.y + value > 0 ? value : -RectTransform.localPosition.y;
    }
}