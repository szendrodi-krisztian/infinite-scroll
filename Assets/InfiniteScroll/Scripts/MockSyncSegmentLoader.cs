﻿using System.Collections.Generic;
using System.Linq;

namespace Rabbit.UI
{
    public sealed class MockSyncSegmentLoader : ISegmentLoader<int>
    {
        private readonly List<ISegmentConsumer<int>> consumers = new List<ISegmentConsumer<int>>();
        public int TotalCount => int.MaxValue;

        public void LoadElement(int index)
        {
            foreach (var consumer in consumers.Where(t => t != null))
            {
                consumer.ConsumeSegment(index, index);
            }
        }

        public void AddConsumer(ISegmentConsumer<int> consumer) => consumers.Add(consumer);
    }
}