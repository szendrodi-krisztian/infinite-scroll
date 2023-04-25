using Rabbit.DataStructure;
using UnityEngine;

namespace Rabbit.UI
{
    public abstract class InfiniteScrollViewElementBase : MonoBehaviour, IInfiniteScrollViewElement
    {
        [SerializeField] protected float elementHeight;
        [SerializeField] protected int elementIndex;

        public RectTransform RectTransform => transform as RectTransform;

        public float ElementHeight => Mathf.Max(elementHeight, b: 1);

        public int ElementIndex
        {
            get => elementIndex;
            set => elementIndex = value;
        }

        public virtual void OnPoolGet() => gameObject.SetActive(true);

        public virtual void OnPoolRelease() => gameObject.SetActive(false);

        public void MoveBy(float delta) => RectTransform.position += new Vector3(x: 0, delta, z: 0);
        public abstract void UpdateDisplay(IDataSource data);
    }
}