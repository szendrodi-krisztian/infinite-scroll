using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rabbit.UI
{
    public class Future<T>
    {
        private T reference;
        private bool isCompleted;

        public T Reference => reference;
        public bool IsCompleted => isCompleted;

        public void Complete(T data)
        {
            reference = data;
            isCompleted = true;
        }
    }

    public interface ISegmentLoader<out T>
    {
        // NEEDS TO MEMOIZE WITH A DICTIONARY!!!
        public void LoadSegment(int startIndex, int count, Action<IEnumerable<T>> onDone);
    }

    public class MockSegmentLoader : ISegmentLoader<int>
    {
        public void LoadSegment(int startIndex, int count, Action<IEnumerable<int>> onDone)
        {
            var list = new List<int>();
            for (var i = startIndex; i < startIndex + count; i++)
            {
                list.Add(i);
            }

            onDone.Invoke(list);
        }
    }

    public class InfiniteSegmentedLinkedList<T>
    {
        private const int DefaultMaxLoadedElementCount = 100;

        private SegmentedLinkedList<T> data;
        private readonly int maxLoadedElementCount;
        private readonly int nodeCapacity;

        private readonly ISegmentLoader<T> loader;


        public InfiniteSegmentedLinkedList(ISegmentLoader<T> loader, int nodeCapacity = 10,
            int maxLoadedElementCount = DefaultMaxLoadedElementCount)
        {
            this.loader = loader;
            this.nodeCapacity = nodeCapacity;
            this.maxLoadedElementCount = maxLoadedElementCount;
            data = new SegmentedLinkedList<T>(nodeCapacity);
        }

        public void AddRange(IEnumerable<T> source)
        {
            data.AddRange(source);
            while (data.Count > maxLoadedElementCount && data.NextNode != null)
            {
                data = data.NextNode;
                data.PrevNode = null;
            }
        }

        public Future<T> ElementAt(int index)
        {
            var future = new Future<T>();

            if (index < data.StartIndex)
            {
                var loadStart = Mathf.CeilToInt(-(index - data.StartIndex) / nodeCapacity) * nodeCapacity;
                var loadCount = data.StartIndex - loadStart;
                loader.LoadSegment(loadStart, loadCount, loadedData =>
                {
                    var newHead = new SegmentedLinkedList<T>(nodeCapacity);
                    newHead.AddRange(loadedData);
                    newHead.AppendList(data);

                    data = newHead;

                    future.Complete(data.ElementAt(index));
                });
            }
            else if (index > data.StartIndex + data.Count - 1)
            {
                var loadStart = data.StartIndex + data.Count;
                var loadCount = index - loadStart + 1;
                loader.LoadSegment(loadStart, loadCount, loadedData =>
                {
                    foreach (var element in loadedData)
                        data.AddLast(element);
                    future.Complete(data.ElementAt(index));
                });
            }
            else
            {
                future.Complete(data.ElementAt(index));
            }

            return future;
        }
    }
}