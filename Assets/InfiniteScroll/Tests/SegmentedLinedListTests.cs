using System;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

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
                list.AddLast(samples[i]);
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
                list.AddLast(1);
                list.AddLast(2);
                list.AddLast(3);
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

        [Test]
        public void Is_Indexing_Correct_After_RemoveAt()
        {
            var list = new SegmentedLinkedList<int>(5);
            for (var i = 0; i < 17; i++)
            {
                list.AddLast(i);
            }

            var segments = list.Segments.Select(x => x.StartIndex).ToArray();
            var expected = new[] { 0, 5, 10, 15 };

            for (var i = 0; i < segments.Length; i++)
            {
                Assert.IsTrue(expected[i] == segments[i]);
            }

            list.RemoveAt(2);

            segments = list.Segments.Select(x => x.StartIndex).ToArray();
            expected = new[] { 0, 4, 9, 14 };
            for (var i = 0; i < segments.Length; i++)
            {
                Assert.IsTrue(expected[i] == segments[i]);
            }
        }

        [Test]
        public void Is_Indexing_Correct_After_RemoveAll1()
        {
            var list = new SegmentedLinkedList<int>(5);
            for (var i = 0; i < 17; i++)
            {
                list.AddLast(i / 2);
            }

            var segments = list.Segments.Select(x => x.StartIndex).ToArray();
            var expected = new[] { 0, 5, 10, 15 };

            for (var i = 0; i < segments.Length; i++)
            {
                Assert.IsTrue(expected[i] == segments[i]);
            }

            list.Remove(1);

            segments = list.Segments.Select(x => x.StartIndex).ToArray();
            expected = new[] { 0, 3, 8, 13 };
            for (var i = 0; i < segments.Length; i++)
            {
                Assert.IsTrue(expected[i] == segments[i]);
            }
        }

        [Test]
        public void Is_Indexing_Correct_After_RemoveAll2()
        {
            var list = new SegmentedLinkedList<int>(5);
            for (var i = 0; i < 17; i++)
            {
                list.AddLast(i / 2);
            }

            var segments = list.Segments.Select(x => x.StartIndex).ToArray();
            var expected = new[] { 0, 5, 10, 15 };

            for (var i = 0; i < segments.Length; i++)
            {
                Assert.IsTrue(expected[i] == segments[i]);
            }

            list.Remove(2);

            segments = list.Segments.Select(x => x.StartIndex).ToArray();
            expected = new[] { 0, 4, 8, 13 };
            for (var i = 0; i < segments.Length; i++)
            {
                Assert.IsTrue(expected[i] == segments[i]);
            }
        }

        [Test]
        public void AddRange_Works()
        {
            var list = new SegmentedLinkedList<int>(2);

            var expected = new[] { 0, 5, 10, 15 };

            list.AddRange(expected);

            for (var i = 0; i < list.Count; i++)
            {
                Assert.IsTrue(expected[i] == list[i]);
            }
        }

        [Test]
        public void Append_Works()
        {
            var list1 = new SegmentedLinkedList<int>(2);
            var expected1 = new[] { 0, 5, 10, 15 };
            list1.AddRange(expected1);

            var list2 = new SegmentedLinkedList<int>(2);
            var expected2 = new[] { 1, 3, 8, 27 };
            list2.AddRange(expected2);

            list1.AppendList(list2);

            var expected = new[] { 0, 5, 10, 15, 1, 3, 8, 27 };

            for (var i = 0; i < list1.Count; i++)
            {
                Assert.IsTrue(expected[i] == list1[i]);
            }
        }

        [Test]
        public void Insert_Works1()
        {
            InsertTest(1);
        }

        [Test]
        public void Insert_Works2()
        {
            InsertTest(4);
        }

        [Test]
        public void Insert_Works3()
        {
            InsertTest(5);
        }
        
        [Test]
        public void Insert_Works4()
        {
            for (int i = 0; i < 100; i++)
            {
                InsertTest(i);
            }
        }

        private static void InsertTest(int insertionIndex)
        {
            var list = new SegmentedLinkedList<int>(3);

            var initial = new[]
            {
                0, 5, 10,
                15, 42, 206,
                78, 54, 25,
                988, 45, 72,
                9
            };

            list.AddRange(initial);

            for (var i = 0; i < list.Count; i++)
            {
                Assert.IsTrue(initial[i] == list[i]);
            }

            var ins = 9999999;

            list.Insert(insertionIndex, ins);

            var expected = initial.ToList();
            while (expected.Count < insertionIndex)
            {
                expected.Add(0);
            }
            expected.Insert(insertionIndex, ins);

            for (var i = 0; i < list.Count; i++)
            {
                Assert.IsTrue(expected[i] == list[i]);
            }
        }
    }
}