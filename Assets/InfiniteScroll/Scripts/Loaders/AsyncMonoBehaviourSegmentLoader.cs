﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Rabbit.DataStructure;
using UnityEngine;

namespace Rabbit.Loaders
{
    public abstract class AsyncMonoBehaviourSegmentLoader<T> : MonoBehaviour, ISegmentLoader<T>
    {
        private readonly List<ISegmentConsumer<T>> consumers = new();
        private readonly List<ElementLoadResult> loaded = new();

        protected abstract bool UseRealThread { get; }
        protected abstract int PreLoadLength { get; }

        protected virtual void Update()
        {
            if (!Monitor.TryEnter(this))
                return;

            {
                foreach (var l in loaded)
                {
                    foreach (var consumer in consumers.Where(t => t != null))
                    {
                        consumer.OnSegmentLoadFinished(l.index, l.result);
                    }
                }

                loaded.Clear();
            }

            Monitor.Exit(this);
        }

        public abstract int TotalCount { get; }

        public void LoadElement(int index)
        {
            Monitor.Enter(this);

            {
                for (var j = index; j < index + PreLoadLength; j++)
                {
                    foreach (var consumer in consumers.Where(t => t != null))
                    {
                        consumer.OnSegmentLoadStarted(j);
                    }
                }

                if (UseRealThread)
                {
                    ThreadPool.QueueUserWorkItem(LoadSegmentThreaded, index);
                }
                else
                {
                    LoadSegmentThreaded(index);
                }
            }

            Monitor.Exit(this);
        }
        public void AddConsumer(ISegmentConsumer<T> consumer) => consumers.Add(consumer);

        public void RemoveConsumer(ISegmentConsumer<T> consumer) => consumers.Remove(consumer);

        public void Invalidate()
        {
            StopAllCoroutines();
            loaded.Clear();
            consumers.Clear();
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