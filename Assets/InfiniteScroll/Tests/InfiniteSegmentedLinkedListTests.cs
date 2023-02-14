using NUnit.Framework;

namespace Rabbit.UI
{
    public class InfiniteSegmentedLinkedListTests
    {
        [Test]
        public void Can_Be_Created()
        {
            var list = new InfiniteSegmentedLinkedList<int>(new MockSegmentLoader());
            Assert.True(list != null);
        }

        [Test]
        public void Is_Add_Get_Good()
        {
            var list = new InfiniteSegmentedLinkedList<int>(new MockSegmentLoader(), 2);
            Assert.True(list != null);

            var expected = new[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9
            };

            list.AddRange(expected);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(list.ElementAt(i).Reference == expected[i]);
            }
        }

        [Test]
        public void Is_Loading_Good()
        {
            var list = new InfiniteSegmentedLinkedList<int>(new MockSegmentLoader(), 2);
            Assert.True(list != null);

            var expected = new[]
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8
            };

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.IsTrue(list.ElementAt(i).Reference == expected[i]);
            }
        }

        [Test]
        public void Is_Loading_Good_With_Max_Count()
        {
            var list = new InfiniteSegmentedLinkedList<int>(new MockSegmentLoader(), 2, 5);
            Assert.True(list != null);

            var expected = new[]
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8
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