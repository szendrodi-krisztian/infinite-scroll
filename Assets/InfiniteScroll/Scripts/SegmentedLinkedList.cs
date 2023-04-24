using System.Collections;
using System.Collections.Generic;

namespace Rabbit.UI
{
    public sealed class SegmentedLinkedList<T> : SegmentedLinkedListBase<T, SegmentedLinkedList<T>>, ICollection<T>
    {
        public int StartIndex => startIndex;
        private SegmentedLinkedList<T> NextNode => nextNode;

        public IEnumerable<SegmentedLinkedList<T>> Segments
        {
            get
            {
                yield return this;

                if (NextNode != null)
                {
                    foreach (var segment in NextNode.Segments)
                    {
                        yield return segment;
                    }
                }
            }
        }
        public T this[int i] => ElementAt(i);

        public SegmentedLinkedList() : this(SegmentedLinkedListBase<T, SegmentedLinkedList<T>>.DefaultNodeCapacity) { }
        public SegmentedLinkedList(int nodeCapacity = SegmentedLinkedListBase<T, SegmentedLinkedList<T>>.DefaultNodeCapacity) => Init(nodeCapacity);
        public bool IsReadOnly => false;

        public void Add(T item) => AddLast(item);

        public void Clear()
        {
            NextNode?.Clear();
            nodeCount = 0;
        }

        public bool Contains(T item)
        {
            for (var i = 0; i < nodeCount; i++)
            {
                if (Equals(ElementAt(i), item))
                {
                    return true;
                }
            }

            return nextNode?.Contains(item) == true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
            {
                yield return ElementAt(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void CopyTo(T[] array, int arrayIndex) => array[arrayIndex] = ElementAt(arrayIndex);

        public void AddLast(T newElement) => SetElementAt(Count, newElement);

        public void Insert(int index, T element)
        {
            if (IsIndexInPrevNode(index))
            {
                CreatePrevNodeIfNeeded();
                prevNode.Insert(index, element);
            }
            else if (IsIndexInNextNode(index))
            {
                CreateNextNodeIfNeeded();
                nextNode.Insert(index, element);
            }
            else
            {
                InsertAndShiftForward(index, element);
            }
        }

        public void AddRange(IEnumerable<T> elements)
        {
            if (!IsNodeFull)
            {
                foreach (var e in elements)
                {
                    AddLast(e);
                }

                return;
            }

            NextNode.AddRange(elements);
        }
    }
}