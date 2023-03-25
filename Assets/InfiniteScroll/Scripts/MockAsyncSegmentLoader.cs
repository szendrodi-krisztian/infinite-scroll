using System;
using System.Threading;

namespace Rabbit.UI
{
    public class MockAsyncSegmentLoader : AsyncMonoBehaviourSegmentLoader<string>
    {
        private readonly Random random = new();

        public override int TotalCount => 20;

        protected override void LoadOnThread(int idx)
        {
            var delay = random.Next(1000, 3000);
            Thread.Sleep(delay);

            OnElementLoaded(idx, $"{idx}");
        }

        protected override bool UseRealThread => true;
    }
}