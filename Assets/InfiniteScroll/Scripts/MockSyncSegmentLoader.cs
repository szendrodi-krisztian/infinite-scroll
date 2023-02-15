using System;
using System.Collections.Generic;
using System.Threading;

namespace Rabbit.UI
{
    public class MockSyncSegmentLoader : ISegmentLoader<int>
    {
        public void LoadElement(int i, Action<int, int> onElementDone)
        {
            onElementDone.Invoke(i, i);
        }

        public int TotalCount => int.MaxValue;
    }
}