using System.Collections.Generic;

namespace Rabbit.UI
{
    public sealed class InfiniteSegmentedLinkedList<T>
    {
        private const int DefaultMaxLoadedElementCount = 100;

        private readonly ISegmentLoader<T> loader;
        private readonly int maxLoadedElementCount;
        private readonly int nodeCapacity;

        private SegmentedLinkedList<T> data;

        public int Count => loader.TotalCount;

        public InfiniteSegmentedLinkedList(ISegmentLoader<T> loader, int nodeCapacity = 10, int maxLoadedElementCount = InfiniteSegmentedLinkedList<T>.DefaultMaxLoadedElementCount)
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
                loader.LoadElement(index, onElementDone: (loadedIndex, loadedData) =>
                {
                    data.SetElementAt(loadedIndex, loadedData);
                    future.Complete(data.ElementAt(index));
                });
            }
            else if (index > data.StartIndex + data.Count - 1)
            {
                loader.LoadElement(index, onElementDone: (loadedIndex, loadedData) =>
                {
                    data.SetElementAt(loadedIndex, loadedData);
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