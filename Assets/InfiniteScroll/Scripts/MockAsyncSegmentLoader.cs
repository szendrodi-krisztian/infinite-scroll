using System;
using System.Threading;

namespace Rabbit.UI
{
    public sealed class MockAsyncSegmentLoader : AsyncMonoBehaviourSegmentLoader<string>
    {
        private readonly Random random = new Random();

        public override int TotalCount => 20;

        protected override bool UseRealThread => true;

        protected override void LoadOnThread(int idx)
        {
            var delay = random.Next(minValue: 1000, maxValue: 3000);
            Thread.Sleep(delay);

            OnElementLoaded(idx, $"{idx}");
        }
    }
}