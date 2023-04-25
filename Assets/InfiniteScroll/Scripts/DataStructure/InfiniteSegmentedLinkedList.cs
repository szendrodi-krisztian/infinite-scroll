using System;
using Rabbit.Loaders;
using Rabbit.Utils;

namespace Rabbit.DataStructure
{
    public sealed class InfiniteSegmentedLinkedList<T> : ISegmentConsumer<T>
    {
        private readonly SegmentedLinkedList<Future<T>> data;

        private readonly ISegmentLoader<T> loader;

        public int Count => loader.TotalCount;

        public Future<T> this[int index] => ElementAt(index);

        public InfiniteSegmentedLinkedList(ISegmentLoader<T> loader, int nodeCapacity = 10)
        {
            this.loader = loader;
            data = new SegmentedLinkedList<Future<T>>(nodeCapacity);
            loader.AddConsumer(this);
        }

        public void OnSegmentLoadStarted(int index)
        {
            if (!data.HasIndex(index))
            {
                data[index] = new Future<T>();
            }
        }

        public void OnSegmentLoadFinished(int index, T nextLoadedElement)
        {
            if (data.HasIndex(index))
            {
                data[index].Complete(nextLoadedElement);
                return;
            }

            var future = new Future<T>();
            future.Complete(nextLoadedElement);
            data[index] = future;
        }

        public Future<T> ElementAt(int index)
        {
            if (data.HasIndex(index))
                return data[index];

            loader.LoadElement(index);
            return data.HasIndex(index) ? data[index] : throw new ArgumentException("Loader is not filtering out redundant loading!");

        }
    }
}