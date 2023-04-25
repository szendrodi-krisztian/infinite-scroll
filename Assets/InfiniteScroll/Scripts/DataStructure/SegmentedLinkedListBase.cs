using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rabbit.DataStructure
{
    public abstract class SegmentedLinkedListBase<T, TImpl> where TImpl : SegmentedLinkedListBase<T, TImpl>, new()
    {
        protected const int DefaultNodeCapacity = 64;

        private T[] data;
        private TImpl nextNode;
        private int nodeCapacity;
        private int nodeCount;
        private TImpl prevNode;
        private int startIndex;

        protected TImpl NextNode => nextNode;
        protected TImpl PrevNode => prevNode;
        protected bool IsNodeFull => nodeCount == nodeCapacity;
        public int Count => nodeCount + (nextNode?.Count ?? 0);
        public int StartIndex => startIndex;

        public bool HasIndex(int index)
        {
            if (IsIndexInPrevNode(index))
                return prevNode?.HasIndex(index) == true;

            if (IsIndexInNextNode(index))
                return nextNode?.HasIndex(index) == true;

            return !EqualityComparer<T>.Default.Equals(data[index - startIndex], y: default);
        }

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

        public void Clear()
        {
            NextNode?.Clear();
            nodeCount = 0;
        }

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

                RebaseIndices(startIndex);
            }
        }

        private void RebaseIndices(int newStartIndex)
        {
            startIndex = newStartIndex;
            nextNode?.RebaseIndices(startIndex + nodeCapacity);
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
                RebaseIndices(startIndex);
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

            RebaseIndices(startIndex);

            return true;
        }
    }
}