using System;

namespace Rabbit.Utils
{
    public sealed class Future<T>
    {
        private bool isCompleted;
        private Action<T> onComplete = delegate { };
        private T reference;

        public T Reference => reference;
        public bool IsCompleted => isCompleted;

        public void Complete(T data)
        {
            reference = data;
            isCompleted = true;
            onComplete(reference);
        }

        public void WhenComplete(Action<T> action)
        {
            if (isCompleted)
                action(reference);
            else
                onComplete += action;
        }
    }
}