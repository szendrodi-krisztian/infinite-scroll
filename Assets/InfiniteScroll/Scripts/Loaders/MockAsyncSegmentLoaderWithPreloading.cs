using System;
using System.Threading;

namespace Rabbit.Loaders
{
    public sealed class MockAsyncSegmentLoaderWithPreloading : AsyncMonoBehaviourSegmentLoader<string>
    {
        public static int ActualRequestCounter;

        private readonly Random random = new Random();

        public override int TotalCount => 50;

        protected override bool UseRealThread => true;

        protected override void LoadOnThread(int idx)
        {
            var delay = random.Next(minValue: 100, maxValue: 300);
            Thread.Sleep(delay);

            // TODO: call back to the tests how many times this happens.
            // When requesting elements 1..100, it should only happen 5 times.

            for (var i = idx; i < idx + 20; i++)
            {
                OnElementLoaded(i, $"{i}");
            }
        }
    }
}