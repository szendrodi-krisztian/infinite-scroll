﻿using System.Collections.Generic;
using System.Linq;
using Rabbit.DataStructure;
using UnityEngine;

namespace Rabbit.Loaders
{
    public sealed class MockSyncSegmentLoader : MonoBehaviour, ISegmentLoader<int>
    {
        private readonly List<ISegmentConsumer<int>> consumers = new List<ISegmentConsumer<int>>();
        public int TotalCount => int.MaxValue;

        public void LoadElement(int index)
        {
            foreach (var consumer in consumers.Where(t => t != null))
            {
                consumer.OnSegmentLoadFinished(index, index);
            }
        }

        public void AddConsumer(ISegmentConsumer<int> consumer) => consumers.Add(consumer);
    }
}