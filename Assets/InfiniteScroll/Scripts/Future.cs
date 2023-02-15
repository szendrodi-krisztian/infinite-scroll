using System;

namespace Rabbit.UI
{
    public class Future<T>
    {
        private T reference;
        private bool isCompleted = false;
        private Action onComplete = delegate { };

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