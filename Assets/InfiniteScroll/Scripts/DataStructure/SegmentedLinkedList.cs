using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.DataStructure
{
    public sealed class SegmentedLinkedList<T> : SegmentedLinkedListBase<T, SegmentedLinkedList<T>>, ICollection<T>
    {
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

        public SegmentedLinkedList() : this(SegmentedLinkedListBase<T, SegmentedLinkedList<T>>.DefaultNodeCapacity) { }
        public SegmentedLinkedList(int nodeCapacity = SegmentedLinkedListBase<T, SegmentedLinkedList<T>>.DefaultNodeCapacity) => Init(nodeCapacity);
        public bool IsReadOnly => false;

        public void Add(T item) => AddLast(item);
        public bool Contains(T item) => Enumerable.Contains(this, item);

        public IEnumerator<T> GetEnumerator()
        {
            var count = Count;

            for (var i = 0; i < count; i++)
            {
                yield return ElementAt(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void CopyTo(T[] array, int arrayIndex) => array[arrayIndex] = ElementAt(arrayIndex);
        public void AddLast(T newElement) => SetElementAt(Count, newElement);
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

        public void Insert(int index, T element)
        {
            if (IsIndexInPrevNode(index))
            {
                CreatePrevNodeIfNeeded();
                PrevNode.Insert(index, element);
            }
            else if (IsIndexInNextNode(index))
            {
                CreateNextNodeIfNeeded();
                NextNode.Insert(index, element);
            }
            else
            {
                InsertAndShiftForward(index, element);
            }
        }
    }
}