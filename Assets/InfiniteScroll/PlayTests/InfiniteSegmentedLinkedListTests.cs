using System.Collections;
using NUnit.Framework;
using Rabbit.DataStructure;
using Rabbit.Loaders;
using UnityEngine;
using UnityEngine.TestTools;

namespace Rabbit.UI
{
    public sealed class InfiniteSegmentedLinkedListTests
    {
        private readonly int[] expectedListValues =
        {
            0, 1,
            2, 3,
            4, 5,
            6, 7,
            8, 9,
        };

        [UnityTest]
        public IEnumerator List_Can_Be_Created()
        {
            var list = CreateList<MockSyncSegmentLoader>();
            Assert.True(list != null);
            yield break;
        }

        private static InfiniteSegmentedLinkedList<int> CreateList<TLoader>(int nodeCapacity = 10)
        {
            var loader = new GameObject(nameof(MockSyncSegmentLoader)).AddComponent<MockSyncSegmentLoader>();
            var list = new InfiniteSegmentedLinkedList<int>(loader, nodeCapacity);
            return list;
        }

        [UnityTest]
        public IEnumerator List_Add_And_Get_Works()
        {
            var list = CreateList<MockSyncSegmentLoader>(2);
            Assert.True(list != null);

            for (var i = 0; i < expectedListValues.Length; i++)
            {
                Assert.IsTrue(list.ElementAt(i).Reference == expectedListValues[i]);
            }

            yield break;
        }

        [UnityTest]
        public IEnumerator List_Preloading_Works()
        {
            var list = CreateList<MockSyncSegmentLoaderWithPreloading>(2);
            Assert.True(list != null);

            for (var i = 0; i < expectedListValues.Length; i++)
            {
                Assert.IsTrue(list.ElementAt(i).Reference == expectedListValues[i]);
            }

            yield break;
        }

        [Test]
        public void Is_Loading_Good()
        {
            var list = CreateList<MockSyncSegmentLoader>(2);
            Assert.True(list != null);

            var expected = new[]
            {
                0, 1,
                2, 3,
                4, 5,
                6, 7,
                8,
            };

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(list.ElementAt(i).Reference == expected[i]);
            }
        }

        [Test]
        public void Is_Loading_Good_With_Max_Count()
        {
            var list = CreateList<MockSyncSegmentLoader>(2);
            Assert.True(list != null);

            var expected = new[]
            {
                0, 1,
                2, 3,
                4, 5,
                6, 7,
                8,
            };

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(list.ElementAt(i).Reference == expected[i]);
            }

            for (var i = expected.Length - 1; i >= 0; i--)
            {
                Assert.IsTrue(list.ElementAt(i).Reference == expected[i]);
            }
        }
    }
}