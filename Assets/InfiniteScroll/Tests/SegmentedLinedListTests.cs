using NUnit.Framework;

namespace Rabbit.UI
{
    public class SegmentedLinedListTests
    {
        private readonly int[] samples =
        {
            42, 84, 96, 85, 24, 65, 78,
            42, 84, 96, 85, 24, 65, 78,
            42, 84, 96, 85, 24, 65, 78,
            42, 84, 96, 85, 24, 65, 78,
            42, 84, 96, 85, 24, 65, 78,
            42, 84, 96, 85, 24, 65, 78,
            42, 84, 96, 85, 24, 65, 78,
            42, 84, 96, 85, 24, 65, 78,
            42, 84, 96, 85, 24, 65, 78,
            42, 84, 96, 85, 24, 65, 78,
            42, 84, 96, 85, 24, 65, 78,
            42, 84, 96, 85, 24, 65, 78,
            42, 84, 96, 85, 24, 65, 78,
        };

        [Test]
        public void Can_Be_Created()
        {
            var list = new SegmentedLinkedList<int>();
            Assert.True(list != null);
        }

        [Test]
        public void Can_Be_Added_To_And_Received()
        {
            var list = new SegmentedLinkedList<int>(3);

            AddSamples(list);

            for (var i = 0; i < samples.Length; i++)
            {
                var isGood = list.ElementAt(i) == samples[i];
                Assert.IsTrue(isGood);
            }
        }

        private void AddSamples(SegmentedLinkedList<int> list)
        {
            for (var i = 0; i < samples.Length; i++)
            {
                list.Add(samples[i]);
            }
        }

        [Test]
        public void Can_Be_Removed_From()
        {
            for (var capacity = 1; capacity < 10; capacity++)
            {
                for (var removeIndex = 0; removeIndex < 7; removeIndex++)
                {
                    RemoveTest(capacity, removeIndex);
                }
            }
        }

        private void RemoveTest(int capacity, int removeIndex)
        {
            var list = new SegmentedLinkedList<int>(capacity);
            AddSamples(list);

            list.RemoveAt(removeIndex);

            for (var i = 0; i < samples.Length - 1; i++)
            {
                var isSame = list.ElementAt(i) == samples[i];
                var isSameAsNext = list.ElementAt(i) == samples[i + 1];

                Assert.IsTrue(i < removeIndex ? isSame : isSameAsNext);
            }
            
            Assert.IsTrue(list.Count == samples.Length - 1);
        }

        [Test]
        public void Is_Enumerable()
        {
            var list = new SegmentedLinkedList<int>(5);
            AddSamples(list);

            var index = 0;
            foreach (var i in list)
            {
                Assert.IsTrue(i == samples[index]);
                index++;
            }
        }

        [Test]
        public void Is_Remove_All_Good()
        {
            var list = new SegmentedLinkedList<int>(5);
            var l = 10;

            for (int i = 0; i < l; i++)
            {
                list.Add(1);
                list.Add(2);
                list.Add(3);
            }

            Assert.IsTrue(list.Count == l * 3);

            list.Remove(2);

            Assert.IsTrue(list.Count == l * 2);

            for (var i = 0; i < l; i++)
            {
                var value = i % 2 == 0 ? 1 : 3;
                Assert.IsTrue(list[i] == value);
            }

            Assert.IsTrue(list[0] == 1);
            Assert.IsTrue(list[1] == 3);
            Assert.IsTrue(list[2] == 1);
            Assert.IsTrue(list[3] == 3);
        }
    }
}