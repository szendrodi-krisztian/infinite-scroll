using Rabbit.Loaders;
using Rabbit.Utils;
using UnityEngine;

namespace Rabbit.DataStructure
{
    public abstract class InfiniteDataSourceBase<T> : MonoBehaviour, IDataSource
    {
        private InfiniteSegmentedLinkedList<T> backingData;
        private ISegmentLoader<T> segmentLoader;

        public int Count => backingData.Count;
        public Future<T2> GetItem<T2>(int index) => backingData.GetItem<T2>(index);

        public void Initialize()
        {
            segmentLoader = GetComponent<ISegmentLoader<T>>();
            backingData = new InfiniteSegmentedLinkedList<T>(segmentLoader, nodeCapacity: 10);
        }
    }
}