using System;
using System.Linq;
using UnityEngine;

namespace Rabbit.UI
{
    public abstract class SegmentedLinkedListBase<T, TImpl> where TImpl : SegmentedLinkedListBase<T, TImpl>, new()
    {
        protected const int DefaultNodeCapacity = 64;
        private T[] data;
        protected TImpl nextNode;
        private int nodeCapacity;
        protected int nodeCount;
        protected TImpl prevNode;
        protected int startIndex;

        protected bool IsNodeFull => nodeCount == nodeCapacity;
        public int Count => nodeCount + (nextNode?.Count ?? 0);

        protected TImpl Init(int newNodeCapacity = SegmentedLinkedListBase<T, TImpl>.DefaultNodeCapacity, int newStartIndex = 0, TImpl newPrevNode = null)
        {
            nodeCapacity = newNodeCapacity;
            nodeCount = 0;
            data = new T[newNodeCapacity];
            startIndex = newStartIndex;
            prevNode = newPrevNode;

            return this as TImpl;
        }

        protected bool IsIndexInNextNode(int index) => index >= startIndex + nodeCapacity;
        protected bool IsIndexInPrevNode(int index) => index < startIndex;

        protected void CreateNextNodeIfNeeded() => nextNode ??= new TImpl().Init(nodeCapacity, startIndex + nodeCapacity, this as TImpl);
        protected void CreatePrevNodeIfNeeded()
        {
            if (prevNode == null)
            {
                prevNode = new TImpl().Init(nodeCapacity, startIndex - nodeCapacity);
                prevNode.AppendList(this as TImpl);
            }
        }

        public void RemoveAt(int index)
        {
            if (index > startIndex + nodeCapacity - 1)
                nextNode.RemoveAt(index);
            else
            {
                if (nodeCapacity - 1 == 0)
                {
                    data = Array.Empty<T>();
                    nodeCount = 0;
                    nodeCapacity = 0;
                }
                else
                {

                    var reallocatedData = new T[nodeCapacity - 1];

                    for (int oldIndex = 0, newIndex = 0; oldIndex < nodeCount; oldIndex++)
                    {
                        if (oldIndex == index - startIndex)
                            continue;

                        reallocatedData[newIndex] = data[oldIndex];

                        newIndex++;
                    }

                    nodeCapacity = reallocatedData.Length;
                    data = reallocatedData;
                    nodeCount -= 1;
                }

                SetStartIndex(startIndex);
            }
        }

        private void SetStartIndex(int newStartIndex)
        {
            startIndex = newStartIndex;
            nextNode?.SetStartIndex(startIndex + nodeCapacity);
        }

        public T ElementAt(int index)
        {
            if (IsIndexInNextNode(index))
            {
                CreateNextNodeIfNeeded();
                return nextNode.ElementAt(index);
            }

            if (IsIndexInPrevNode(index))
            {
                CreatePrevNodeIfNeeded();
                return prevNode.ElementAt(index);
            }

            return data[index - startIndex];
        }

        public void SetElementAt(int idx, T element)
        {
            if (IsIndexInNextNode(idx))
            {
                CreateNextNodeIfNeeded();
                nextNode.SetElementAt(idx, element);
            }
            else if (IsIndexInPrevNode(idx))
            {
                CreatePrevNodeIfNeeded();
                prevNode.SetElementAt(idx, element);
            }
            else
            {
                data[idx - startIndex] = element;
                nodeCount = Mathf.Max(idx - startIndex + 1, nodeCount);
            }
        }

        protected void InsertAndShiftForward(int index, T element)
        {
            var list = data.ToList();

            Debug.Log($"insert index {index - startIndex} count {list.Count}");

            if (list.Count == 0)
                list.Add(element);
            else
                list.Insert(index - startIndex, element);

            if (!IsNodeFull)
            {
                data = list.ToArray();
            }
            else
            {
                data = list.SkipLast(1).ToArray();
                CreateNextNodeIfNeeded();
                nextNode.InsertAndShiftForward(nextNode.startIndex, list.Last());
            }
        }

        public void AppendList(TImpl added)
        {
            if (nextNode == null)
            {
                nextNode = added;
                added.prevNode = this as TImpl;
                SetStartIndex(startIndex);
            }
            else
            {
                nextNode.AppendList(added);
            }
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
    }
}