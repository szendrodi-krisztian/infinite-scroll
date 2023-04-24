using System.Collections.Generic;
using System.Linq;

namespace Rabbit.UI
{
    public sealed class MockSyncSegmentLoaderWithPreloading : ISegmentLoader<int>
    {
        private readonly List<ISegmentConsumer<int>> consumers = new List<ISegmentConsumer<int>>();
        public int TotalCount => int.MaxValue;

        public void LoadElement(int index)
        {
            foreach (var consumer in consumers.Where(t => t != null))
            {
                for (var i = 0; i < 6; i++)
                {
                    consumer.ConsumeSegment(index + i, index + i);
                }
            }
        }

        public void AddConsumer(ISegmentConsumer<int> consumer) => consumers.Add(consumer);
    }
}