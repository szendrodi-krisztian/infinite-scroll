using System;

namespace Rabbit.UI
{
    public sealed class Future<T>
    {
        private bool isCompleted;
        private Action onComplete = delegate { };
        private T reference;

        public T Reference => reference;
        public bool IsCompleted => isCompleted;

        public void Complete(T data)
        {
            reference = data;
            isCompleted = true;
            onComplete();
        }

        public void WhenComplete(Action action)
        {
            if (isCompleted)
                action();
            else
                onComplete = action;
        }
    }
}