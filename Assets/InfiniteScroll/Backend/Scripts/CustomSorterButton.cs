using System;
using UnityEngine;

namespace Knife
{
    public class CustomSorterButton : MonoBehaviour
    {
        [SerializeField] private CustomLevelsFiltering filtering;
        [SerializeField] private CustomSorterGroup group;

        [SerializeField] private SorterState state;
        [SerializeField] private Sorter ascending;
        [SerializeField] private Sorter descending;

        [SerializeField] private GameObject iconAscending;
        [SerializeField] private GameObject iconDescending;

        private void Awake()
        {
            if (group != null)
            {
                group.Add(this);
            }

            ClearState();
        }

        public void ClearState()
        {
            state = SorterState.None;
            UpdateUI();
        }

        private void UpdateUI()
        {
            switch (state)
            {
                case SorterState.None:
                    iconAscending.SetActive(false);
                    iconDescending.SetActive(false);
                    filtering.AddSorter(null);
                    break;
                case SorterState.Ascending:
                    iconAscending.SetActive(true);
                    iconDescending.SetActive(false);
                    filtering.AddSorter(ascending);
                    break;
                case SorterState.Descending:
                    iconAscending.SetActive(false);
                    iconDescending.SetActive(true);
                    filtering.AddSorter(descending);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnClick()
        {
            group.ResetAllExcept(this);
            state = state.Next();
            UpdateUI();
        }
    }

    public enum SorterState
    {
        None,
        Ascending,
        Descending,
    }

    public static class SorterStateExt
    {
        public static SorterState Next(this SorterState state)
        {
            return state switch
            {
                SorterState.None => SorterState.Ascending,
                SorterState.Ascending => SorterState.Descending,
                SorterState.Descending => SorterState.None,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
            };
        }
    }
}