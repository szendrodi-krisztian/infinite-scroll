using System;
using System.Threading;

namespace Rabbit.Loaders
{
    public sealed class MockAsyncSegmentLoaderWithPreloading : AsyncMonoBehaviourSegmentLoader<string>
    {
        public int actualRequestCounter;

        private readonly Random random = new Random();

        protected override int PreLoadLength => 20;
        public override int TotalCount => 100;

        protected override bool UseRealThread => true;

        protected override void LoadOnThread(int idx)
        {
            var delay = random.Next(minValue: 100, maxValue: 300);
            Thread.Sleep(delay);

            lock( this )
            {
                actualRequestCounter++;
            }

            for (var i = idx; i < idx + 20; i++)
            {
                OnElementLoaded(i, $"{i}");
            }
        }
    }
}