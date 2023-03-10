using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rabbit.UI;
using UnityEngine;
using UnityEngine.TestTools;

public class AsyncLoadingTests
{
    [UnityTest]
    public IEnumerator AsyncLoadingTestsWithEnumeratorPasses()
    {
        var loader = new GameObject("Loader").AddComponent<MockAsyncSegmentLoader>();
        var list = new InfiniteSegmentedLinkedList<int>(loader, 2, 5);
        Assert.True(list != null);

        var expected = new[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8
        };


        var futures = expected.Select(x => list.ElementAt(x)).ToArray();

        while (futures.Any(f => !f.IsCompleted))
        {
            yield return null;
        }

        for (var i = 0; i < expected.Length; i++)
        {
            Assert.IsTrue(list.ElementAt(i).Reference == expected[i]);
        }

        Object.Destroy(loader.gameObject);
    }
}