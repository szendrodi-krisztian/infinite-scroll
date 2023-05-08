using UnityEngine;

namespace Knife
{
    public class CustomLevelFilterButton : MonoBehaviour
    {
        [SerializeField] private Filter filter;

        private CustomLevelsFiltering filtering;

        private void Awake()
        {
            filtering = GetComponentInParent<CustomLevelsFiltering>();
        }

        public void SetFilterActive(bool value)
        {
            filtering.AddFilter(filter);
        }
    }
}