using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Rabbit.DataStructure;
using Rabbit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rabbit.Demo
{
    public sealed class DynamicStringElement : MonoBehaviour, IInfiniteScrollViewElement
    {
        [SerializeField] private float elementOpenedHeight = 200f;
        [SerializeField] private float elementClosedHeight = 50f;
        [SerializeField] private int elementIndex;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Image bg;

        [SerializeField] private Color color1 = new Color(r: 0.1f, g: 0.1f, b: 0.1f);
        [SerializeField] private Color color2 = new Color(r: 0.2f, g: 0.2f, b: 0.2f);

        [SerializeField] private float elementHeight = 50f;
        private bool isOpened;
        private IInfiniteScrollView scrollView;

        public RectTransform RectTransform => transform as RectTransform;

        public float ElementHeight => elementHeight;

        public int ElementIndex
        {
            get => elementIndex;
            set => elementIndex = value;
        }

        public void Initialize(IInfiniteScrollView newScrollViewParent)
        {
            scrollView = newScrollViewParent;
            elementHeight = elementClosedHeight;
        }
        public void OnPoolGet()
        {
            gameObject.SetActive(true);
            isOpened = false;
            elementHeight = elementClosedHeight;
            DisplayLoading();
        }

        public void OnPoolRelease() => gameObject.SetActive(false);

        public void MoveBy(float delta) => RectTransform.position += new Vector3(x: 0, delta, z: 0);
        public void UpdateDisplay(IDataSource data) => data.GetItem<string>(elementIndex).WhenComplete(UpdateUI);
        public void DisplayLoading() => UpdateUI("Loading....");

        public void OnButtonClicked()
        {
            isOpened = !isOpened;
            UpdateUI(label.text);
        }

        private void UpdateUI(string s)
        {
            label.text = s;
            bg.color = elementIndex % 2 == 0 ? color1 : color2;
            StartAnimation();
        }

        private TweenerCore<float, float, FloatOptions> StartAnimation()
        {
            this.DOKill();
            var h = isOpened ? elementOpenedHeight : elementClosedHeight;

            return DOHeight(h, duration: 0.3f).SetEase(Ease.InOutQuad).OnUpdate(() =>
            {
                RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, elementHeight);
                scrollView.ScrollBy(0);
            });
        }

        private TweenerCore<float, float, FloatOptions> DOHeight(float endValue, float duration) => DOTween.To(getter: () => elementHeight, setter: x => elementHeight = x, endValue, duration);
    }
}