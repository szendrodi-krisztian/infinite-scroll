using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rabbit.UI
{
    public class InfiniteScrollView : MonoBehaviour
    {
    }

    public interface IScrollContentProvider : IEnumerable<IScrollContentElement>
    {
        public IScrollContentElement GetElementAt(int index);
    }

    public abstract class ScrollContentProviderBase : IScrollContentProvider
    {
        public IEnumerator<IScrollContentElement> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public IScrollContentElement GetElementAt(int index)
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public interface IScrollContentElement
    {
    }
}