using System.Collections.Generic;
using System.Linq;
using Rabbit.DataStructure;
using UnityEngine;

namespace Rabbit.Loaders
{
    public sealed class MockSyncSegmentLoaderWithPreloading : MonoBehaviour, ISegmentLoader<int>
    {
        private readonly List<ISegmentConsumer<int>> consumers = new List<ISegmentConsumer<int>>();
        public int TotalCount => int.MaxValue;
        public void LoadElement(int index)
        {
            foreach (var consumer in consumers.Where(t => t != null))
            {
                for (var i = index; i < index + 6; i++)
                {
                    consumer.OnSegmentLoadFinished(i, i);
                }
            }
        }

        public void Invalidate()
        {
        }

        public void AddConsumer(ISegmentConsumer<int> consumer) => consumers.Add(consumer);
        public void RemoveConsumer(ISegmentConsumer<int> consumer) => consumers.Remove(consumer);
    }
}