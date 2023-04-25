using System;
using System.Threading;

namespace Rabbit.Loaders
{
    public sealed class MockAsyncSegmentLoader : AsyncMonoBehaviourSegmentLoader<string>
    {
        private readonly Random random = new Random();

        public override int TotalCount => 50;

        protected override bool UseRealThread => true;

        protected override void LoadOnThread(int idx)
        {
            var delay = random.Next(minValue: 100, maxValue: 300);
            Thread.Sleep(delay);

            OnElementLoaded(idx, $"{idx}");
        }
    }
}