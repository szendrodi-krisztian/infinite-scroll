using System;
using System.Collections.Generic;

namespace Rabbit.UI
{
    public interface ISegmentLoader<out T>
    {
        // NEEDS TO MEMOIZE WITH A DICTIONARY!!!
        public void LoadElement(int index, Action<int, T> onElementDone);
        int TotalCount { get; }
    }
}