using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rabbit.UI
{
    public class SegmentedLinkedList<T> : ICollection<T>
    {
        private T[] data;

        private int nodeCapacity;
        private int nodeCount;

        private int startIndex;

        private SegmentedLinkedList<T> nextNode;
        private SegmentedLinkedList<T> prevNode;

        private const int DefaultNodeCapacity = 64;
        public int StartIndex => startIndex;
        public int Count => nodeCount + (nextNode?.Count ?? 0);
        public bool IsReadOnly => false;

        public SegmentedLinkedList<T> NextNode
        {
            get => nextNode;
            set => nextNode = value;
        }

        public SegmentedLinkedList<T> PrevNode
        {
            get => prevNode;
            set => prevNode = value;
        }

        public SegmentedLinkedList(int nodeCapacity = DefaultNodeCapacity, int startIndex = 0,
            SegmentedLinkedList<T> prevNode = null)
        {
            this.nodeCapacity = nodeCapacity;
            nodeCount = 0;
            data = new T[nodeCapacity];
            this.startIndex = startIndex;
            this.prevNode = prevNode;
        }

        public void Insert(int index, T element)
        {
            if (index < startIndex)
            {
                CreatePrevNodeIfNeeded();
                prevNode.Insert(index, element);
            }
            else if (index > startIndex + nodeCapacity - 1)
            {
                CreateNextNodeIfNeeded();
                nextNode.Insert(index, element);
            }
            else
            {
                if (nodeCount < nodeCapacity)
                {
                    var list = data.ToList();
                    list.Insert(index - startIndex, element);
                    data = list.ToArray();
                }
                else
                {
                    var list = data.ToList();
                    list.Insert(index - startIndex, element);
                    data = list.SkipLast(1).ToArray();
                    if (nextNode == null)
                    {
                        AddToNextNode(list.Last());
                    }
                    else
                    {
                        nextNode.ShiftForward(list.Last());
                    }
                }
            }
        }

        private void ShiftForward(T newFirst)
        {
            var list = data.ToList();
            list.Insert(0, newFirst);
            data = list.SkipLast(1).ToArray();
            if (nextNode == null)
            {
                AddToNextNode(list.Last());
            }
            else
            {
                nextNode.ShiftForward(list.Last());
            }
        }

        public void AddRange(IEnumerable<T> elements)
        {
            if (nodeCount == nodeCapacity)
            {
                AddRange(elements);
            }
            else
            {
                foreach (var e in elements)
                {
                    AddLast(e);
                }
            }
        }

        public void AddLast(T newElement)
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

        public IEnumerable<SegmentedLinkedList<T>> Segments
        {
            get
            {
                yield return this;
                if (nextNode != null)
                {
                    foreach (var segment in nextNode.Segments)
                    {
                        yield return segment;
                    }
                }
            }
        }

        public void Add(T item) => AddLast(item);

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

            SetStartIndex(startIndex);

            return true;
        }

        private void AddToNextNode(T newElement)
        {
            CreateNextNodeIfNeeded();
            nextNode.AddLast(newElement);
        }

        private void CreateNextNodeIfNeeded()
        {
            nextNode ??= new SegmentedLinkedList<T>(nodeCapacity, startIndex + nodeCapacity, this);
        }

        private void CreatePrevNodeIfNeeded()
        {
            if (prevNode == null)
            {
                prevNode = new SegmentedLinkedList<T>(nodeCapacity, startIndex - nodeCapacity);
                prevNode.AppendList(this);
            }
        }

        public T ElementAt(int index)
        {
            if (index >= startIndex + nodeCapacity)
            {
                CreateNextNodeIfNeeded();
                return nextNode.ElementAt(index);
            }
            else if (index < startIndex)
            {
                CreatePrevNodeIfNeeded();
                return prevNode.ElementAt(index);
            }
            else
            {
                return data[index - startIndex];
            }
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

                SetStartIndex(startIndex);
            }
        }

        private void SetStartIndex(int newStartIndex)
        {
            startIndex = newStartIndex;
            nextNode?.SetStartIndex(startIndex + nodeCapacity);
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

        public void AppendList(SegmentedLinkedList<T> added)
        {
            if (nextNode == null)
            {
                nextNode = added;
                added.prevNode = this;
                SetStartIndex(startIndex);
            }
            else
            {
                nextNode.AppendList(added);
            }
        }

        public void SetElementAt(int idx, T element)
        {
            if (idx > startIndex + nodeCapacity - 1)
            {
                CreateNextNodeIfNeeded();
                nextNode.SetElementAt(idx, element);
            }

            else if (idx < startIndex)
            {
                CreatePrevNodeIfNeeded();
                prevNode.SetElementAt(idx, element);
            }
            else
            {
                data[idx - startIndex] = element;
                nodeCount = Mathf.Max(idx, nodeCount);
            }
        }
    }
}