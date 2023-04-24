using System;

namespace Rabbit.UI
{
    public sealed class MockSyncSegmentLoader : ISegmentLoader<int>
    {
        public void LoadElement(int i, Action<int, int> onElementDone) => onElementDone.Invoke(i, i);

        public int TotalCount => int.MaxValue;
    }
}