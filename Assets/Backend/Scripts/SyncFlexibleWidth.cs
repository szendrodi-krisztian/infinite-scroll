using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Knife
{
    [ExecuteAlways]
    public class SyncFlexibleWidth : MonoBehaviour
    {
        [SerializeField] private LayoutElement src;
        [SerializeField] private LayoutElement[] dst;

#if UNITY_EDITOR
        private void Update()
        {
            src = GetComponent<LayoutElement>();

            foreach (var d in dst)
            {
                if (d == null) continue;

                if (Math.Abs(d.flexibleWidth - src.flexibleWidth) > 0.00001f)
                {
                    if (PrefabUtility.IsAnyPrefabInstanceRoot(d.gameObject))
                    {
                        d.flexibleWidth = src.flexibleWidth;
                        PrefabUtility.RecordPrefabInstancePropertyModifications(d);
                    }
                    else
                    {
                        d.flexibleWidth = src.flexibleWidth;
                        EditorUtility.SetDirty(d.gameObject);
                    }
                }
            }
        }
#endif
    }
}