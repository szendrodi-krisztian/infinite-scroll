using System;
using System.Threading;

namespace Rabbit.UI
{
    public class MockAsyncSegmentLoader : AsyncMonoBehaviourSegmentLoader<int>
    {
        private readonly Random random = new();
        
        public override int TotalCount => int.MaxValue;

        protected override int LoadOnThread(int idx)
        {
            var delay = random.Next(1000, 3000);
            Thread.Sleep(delay);

            return idx;
        }
    }
}