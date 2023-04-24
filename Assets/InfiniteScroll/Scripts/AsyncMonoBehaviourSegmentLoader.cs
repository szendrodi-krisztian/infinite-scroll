using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Rabbit.UI
{
    public abstract class AsyncMonoBehaviourSegmentLoader<T> : MonoBehaviour, ISegmentLoader<T>
    {
        private readonly Dictionary<int, Action<int, T>> indexMemoizeMap = new Dictionary<int, Action<int, T>>();

        private readonly List<ElementLoadResult> loaded = new List<ElementLoadResult>();
        protected abstract bool UseRealThread { get; }

        public abstract int TotalCount { get; }

        public void LoadElement(int index, Action<int, T> onElementDone)
        {
            Monitor.Enter(this);

            {
                if (indexMemoizeMap.ContainsKey(index))
                {
                    indexMemoizeMap[index] += onElementDone;
                }
                else
                {
                    indexMemoizeMap[index] = onElementDone;

                    if (UseRealThread)
                    {
                        ThreadPool.QueueUserWorkItem(LoadSegmentThreaded, index);
                    }
                    else
                    {
                        LoadSegmentThreaded(index);
                    }
                }
            }

            Monitor.Exit(this);
        }

        protected virtual void Update()
        {
            if (!Monitor.TryEnter(this))
                return;

            {
                foreach (var l in loaded)
                {
                    indexMemoizeMap[l.index](l.index, l.result);
                    indexMemoizeMap.Remove(l.index);
                }

                loaded.Clear();
            }

            Monitor.Exit(this);
        }

        protected abstract void LoadOnThread(int idx);

        private void LoadSegmentThreaded(object state)
        {
            var idx = (int) state;

            LoadOnThread(idx);
        }

        protected void OnElementLoaded(int idx, T result)
        {
            Monitor.Enter(this);

            {
                loaded.Add(new ElementLoadResult(idx, result));
            }

            Monitor.Exit(this);
        }

        private struct ElementLoadResult
        {
            public readonly int index;
            public readonly T result;

            public ElementLoadResult(int index, T result)
            {
                this.index = index;
                this.result = result;
            }
        }
    }
}