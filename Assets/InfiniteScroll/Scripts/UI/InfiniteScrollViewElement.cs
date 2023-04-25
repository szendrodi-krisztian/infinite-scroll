using Rabbit.DataStructure;
using UnityEngine;

namespace Rabbit.UI
{
    public abstract class InfiniteScrollViewElement : MonoBehaviour, IInfiniteScrollViewElement
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

        public virtual void OnPoolGet() => gameObject.SetActive(true);

        public virtual void OnPoolRelease() => gameObject.SetActive(false);

        public void MoveBy(float delta) => RectTransform.position += new Vector3(x: 0, delta, z: 0);

        protected virtual void Awake() => RectTransform = (RectTransform) transform;

        public abstract void UpdateDisplay<T>(InfiniteSegmentedLinkedList<T> data);
    }
}