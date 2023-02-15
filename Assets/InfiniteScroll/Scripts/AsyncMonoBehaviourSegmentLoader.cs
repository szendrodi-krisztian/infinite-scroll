using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Rabbit.UI
{
    public abstract class AsyncMonoBehaviourSegmentLoader<T> : MonoBehaviour, ISegmentLoader<T>
    {
        private struct ElementLoadResult
        {
            public readonly int Index;
            public readonly T Result;

            public ElementLoadResult(int index, T result)
            {
                Index = index;
                Result = result;
            }
        }

        private readonly Dictionary<int, Action<int, T>> indexMemoizeMap = new();

        private readonly List<ElementLoadResult> loaded = new();

        protected abstract T LoadOnThread(int idx);

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
                    ThreadPool.QueueUserWorkItem(LoadSegmentThreaded, index);
                }
            }
            Monitor.Exit(this);
        }

        public abstract int TotalCount { get; }

        private void Update()
        {
            if (!Monitor.TryEnter(this)) return;
            {
                foreach (var l in loaded)
                {
                    indexMemoizeMap[l.Index](l.Index, l.Result);
                    indexMemoizeMap.Remove(l.Index);
                }

                loaded.Clear();
            }
            Monitor.Exit(this);
        }

        private void LoadSegmentThreaded(object state)
        {
            var idx = (int)state;

            var result = LoadOnThread(idx);

            Monitor.Enter(this);
            {
                loaded.Add(new ElementLoadResult(idx, result));
            }
            Monitor.Exit(this);
        }
    }
}