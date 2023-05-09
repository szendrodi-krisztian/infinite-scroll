using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Knife
{
    public static class LayoutUtils
    {
        [MenuItem("Helpers/Convert Flexible layout to anchors")]
        public static void ConvertFlexibleToAnchor()
        {
            var root = Selection.activeGameObject;
            Assert.IsNotNull(root);

            var layoutGroupRoot = root.GetComponent<HorizontalOrVerticalLayoutGroup>();
            Assert.IsNotNull(layoutGroupRoot);

            var enumChildIndices = Enumerable.Range(0, layoutGroupRoot.transform.childCount);
            var layoutChildren = enumChildIndices.Select(layoutGroupRoot.transform.GetChild).Select(x => x.GetComponent<LayoutElement>()).ToArray();
            var rects = layoutChildren.Select(x => x.GetComponent<RectTransform>());

            var flexibleHeights = layoutChildren.Select(x => x.flexibleHeight);
            var flexibleWidths = layoutChildren.Select(x => x.flexibleWidth);

            var heightSum = flexibleHeights.Sum();
            var widthSum = flexibleWidths.Sum();

            var minXes = enumChildIndices.Select(i => flexibleWidths.Take(i).Sum() / widthSum);
            var maxXes = enumChildIndices.Select(i => (flexibleWidths.Take(i).Sum() + flexibleWidths.ElementAt(i)) / widthSum);

            var minYs = enumChildIndices.Select(i => flexibleHeights.Take(i).Sum() / heightSum);
            var maxYs = enumChildIndices.Select(i => (flexibleHeights.Take(i).Sum() + flexibleHeights.ElementAt(i)) / heightSum);

            var minAnchors = minXes.Zip(minYs, (x, y) =>
            {
                if (layoutGroupRoot is VerticalLayoutGroup)
                    return new Vector2(0, y);

                return new Vector2(x, 0);
            });

            var maxAnchors = maxXes.Zip(maxYs, (x, y) =>
            {
                if (layoutGroupRoot is VerticalLayoutGroup)
                    return new Vector2(1, y);

                return new Vector2(x, 1);
            });

            foreach (var _ in rects.Zip(minAnchors, (rt, ma) => rt.anchorMin = ma)) ;
            foreach (var _ in rects.Zip(maxAnchors, (rt, ma) => rt.anchorMax = ma)) ;
        }
    }
}