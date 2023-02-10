using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.UI
{
    public class SegmentedLinkedList<T> : ICollection<T>
    {
        private T[] data;

        private int nodeCapacity;
        private int nodeCount;

        private SegmentedLinkedList<T> nextNode;

        private const int DefaultNodeCapacity = 64;

        public SegmentedLinkedList(int nodeCapacity = DefaultNodeCapacity)
        {
            this.nodeCapacity = nodeCapacity;
            nodeCount = 0;
            data = new T[nodeCapacity];
        }

        public void Add(T newElement)
        {
            if (nodeCount == nodeCapacity)
            {
                AddToNextNode(newElement);
            }
            else
            {
                data[nodeCount++] = newElement;
            }
        }

        public void Clear()
        {
            nextNode?.Clear();
            nodeCount = 0;
        }

        public bool Contains(T item)
        {
            for (var i = 0; i < nodeCount; i++)
            {
                if (Equals(data[i], item))
                {
                    return true;
                }
            }

            if (nextNode != null)
                return nextNode.Contains(item);

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            array[arrayIndex] = ElementAt(arrayIndex);
        }

        public bool Remove(T item)
        {
            var nextRemoved = nextNode?.Remove(item) ?? false;

            var countInThis = data.Count(x => Equals(x, item));

            if (countInThis == 0)
                return nextRemoved;

            var reAllocated = new T[nodeCapacity - countInThis];

            var j = 0;
            for (var i = 0; i < nodeCount; i++)
            {
                if (Equals(data[i], item))
                    continue;

                reAllocated[j++] = data[i];
            }

            data = reAllocated;
            nodeCapacity = reAllocated.Length;
            nodeCount = nodeCount - countInThis;

            return true;
        }

        public int Count => nodeCount + (nextNode?.Count ?? 0);

        public bool IsReadOnly => false;

        private void AddToNextNode(T newElement)
        {
            nextNode ??= new SegmentedLinkedList<T>(nodeCapacity);
            nextNode.Add(newElement);
        }

        public T ElementAt(int index)
        {
            if (index >= nodeCapacity)
                return nextNode.ElementAt(index - nodeCapacity);
            else
                return data[index];
        }

        public void RemoveAt(int index)
        {
            if (index >= nodeCapacity)
                nextNode.RemoveAt(index - nodeCapacity);
            else
            {
                var reallocatedData = new T[nodeCapacity - 1];

                for (int oldIndex = 0, newIndex = 0; oldIndex < nodeCount; oldIndex++)
                {
                    if (oldIndex == index) continue;

                    reallocatedData[newIndex] = data[oldIndex];

                    newIndex++;
                }

                nodeCapacity = reallocatedData.Length;
                data = reallocatedData;
                nodeCount -= 1;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < nodeCount; i++)
            {
                yield return data[i];
            }

            if (nextNode != null)
            {
                foreach (var elem in nextNode)
                {
                    yield return elem;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T this[int i] => ElementAt(i);
    }
}