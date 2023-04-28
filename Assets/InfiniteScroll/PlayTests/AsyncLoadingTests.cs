using System.Collections;
using System.Linq;
using NUnit.Framework;
using Rabbit.DataStructure;
using Rabbit.Loaders;
using UnityEngine;
using UnityEngine.TestTools;

namespace Rabbit.UI
{
    public sealed class AsyncLoadingTests
    {
        [UnityTest]
        public IEnumerator Future_Gets_Completed_For_Preloaded_Too()
        {
            var loader = new GameObject("Loader").AddComponent<MockAsyncSegmentLoaderWithPreloading>();
            var list = new InfiniteSegmentedLinkedList<string>(loader, nodeCapacity: 2);
            Assert.True(list != null);

            var expected = new[] {1, 2, 3, 4, 5};

            var firstElement = list.ElementAt(expected[0]);

            while (!firstElement.IsCompleted)
                yield return null;

            Assert.AreEqual(expected[0].ToString(), firstElement.Reference);

            var secondElement = list.ElementAt(expected[1]);

            Assert.IsTrue(secondElement.IsCompleted);
            Assert.AreEqual(expected[1].ToString(), secondElement.Reference);

            Object.Destroy(loader.gameObject);
        }

        [UnityTest]
        public IEnumerator Async_Loading_Gives_Expected_Result()
        {
            var loader = new GameObject("Loader").AddComponent<MockAsyncSegmentLoader>();
            var list = new InfiniteSegmentedLinkedList<string>(loader, nodeCapacity: 2);
            Assert.True(list != null);

            var expected = new[] {1, 5, 12, 34, 42};

            var futures = expected.Select(x => list.ElementAt(x)).ToArray();

            while (futures.Any(f => !f.IsCompleted))
            {
                yield return null;
            }

            for (var i = 0; i < expected.Length; i++)
            {
                var loadedValue = futures[i].Reference;
                var expectedValue = expected[i].ToString();

                if (loadedValue != expectedValue)
                {
                    Debug.LogError($"expected: [{expectedValue}] but got [{loadedValue}]");
                    Assert.Fail();
                }
            }

            Object.Destroy(loader.gameObject);
        }

        [UnityTest]
        public IEnumerator Async_Loading_Parallel_Works()
        {
            var loader = new GameObject("Loader").AddComponent<MockAsyncSegmentLoaderWithPreloading>();

            lock( loader )
            {
                loader.actualRequestCounter = 0;
            }

            var list = new InfiniteSegmentedLinkedList<string>(loader, nodeCapacity: 2);
            Assert.True(list != null);

            var expected = Enumerable.Range(start: 0, count: 100).ToArray();

            var futures = expected.Select(x => list.ElementAt(x)).ToArray();

            while (futures.Any(f => !f.IsCompleted))
            {
                yield return null;
            }

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(futures[i].Reference == expected[i].ToString());
            }

            lock( loader )
            {
                Assert.AreEqual(expected: 5, loader.actualRequestCounter);
            }

            Object.Destroy(loader.gameObject);
        }
    }
}