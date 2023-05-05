using System;
using UnityEngine;
using UnityEngine.Events;

namespace Knife
{
    public class CustomSorterButton : MonoBehaviour
    {
        [SerializeField] private CustomLevelsFiltering filtering;
        [SerializeField] private CustomSorterGroup group;

        [SerializeField] private TwoStateEvent onNoOrder;
        [SerializeField] private TwoStateEvent onOrderAsc;
        [SerializeField] private TwoStateEvent onOrderDesc;

        private TwoStateEvent currentState;

        private void Awake()
        {
            ClearState();
        }

        public void ClearState()
        {
            currentState?.OnDisabled.Invoke();

            onNoOrder.NextState = onOrderAsc;
            onOrderAsc.NextState = onOrderDesc;
            onOrderDesc.NextState = onNoOrder;

            currentState = onNoOrder;
            currentState.OnEnabled.Invoke();
        }

        public void OnClick()
        {
            group.ResetAllExcept(this);
            currentState.OnDisabled.Invoke();
            currentState = currentState.NextState;
            currentState.OnEnabled.Invoke();

            filtering.AddSorter(currentState.Sorter);
        }
    }

    [Serializable]
    public class TwoStateEvent
    {
        [SerializeField] private Sorter sorter;
        [SerializeField] private UnityEvent onEnabled;
        [SerializeField] private UnityEvent onDisabled;

        private TwoStateEvent nextState;

        public TwoStateEvent NextState
        {
            get => nextState;
            set => nextState = value;
        }

        public Sorter Sorter => sorter;
        public UnityEvent OnEnabled => onEnabled;
        public UnityEvent OnDisabled => onDisabled;
    }
}