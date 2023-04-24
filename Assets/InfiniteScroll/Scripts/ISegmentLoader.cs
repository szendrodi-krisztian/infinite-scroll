using System;

namespace Rabbit.UI
{
    public interface ISegmentLoader<out T>
    {
        int TotalCount { get; }

        // NEEDS TO MEMOIZE WITH A DICTIONARY!!!
        public void LoadElement(int index, Action<int, T> onElementDone);
    }
}