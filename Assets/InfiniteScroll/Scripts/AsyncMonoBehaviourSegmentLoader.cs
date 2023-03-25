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

        protected abstract void LoadOnThread(int idx);
        protected abstract bool UseRealThread { get; }

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

        public abstract int TotalCount { get; }

        protected virtual void Update()
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
    }
}