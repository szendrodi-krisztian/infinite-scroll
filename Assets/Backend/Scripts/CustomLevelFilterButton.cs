using UnityEngine;

namespace Knife
{
    public class CustomLevelFilterButton : MonoBehaviour
    {
        [SerializeField] private CustomLevelsFiltering filtering;
        [SerializeField] private Filter filter;

        public void SetFilterActive(bool value)
        {
            filtering.AddFilter(filter);
        }
    }
}