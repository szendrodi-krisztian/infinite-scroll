using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Rabbit.UI
{
    public sealed class AsyncLoadingTests
    {
        [UnityTest]
        public IEnumerator AsyncLoadingTestsWithEnumeratorPasses()
        {
            var loader = new GameObject("Loader").AddComponent<MockAsyncSegmentLoader>();
            var list = new InfiniteSegmentedLinkedList<string>(loader, nodeCapacity: 2, maxLoadedElementCount: 5);
            Assert.True(list != null);

            var expected = new[]

            {
                0,
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
            };

            var futures = expected.Select(x => list.ElementAt(x)).ToArray();

            while (futures.Any(f => !f.IsCompleted))
            {
                yield return null;
            }

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(list.ElementAt(i).Reference == expected[i].ToString());
            }

            Object.Destroy(loader.gameObject);
        }
    }
}