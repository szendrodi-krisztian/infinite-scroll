using UnityEngine;

namespace Knife
{
    public class CustomSorterGroup : MonoBehaviour
    {
        [SerializeField] private CustomSorterButton[] buttons;

        private void Awake()
        {
            buttons = GetComponentsInChildren<CustomSorterButton>();
        }
        public void ResetAllExcept(CustomSorterButton except)
        {
            for (var i = 0; i < buttons.Length; i++)
            {
                var b = buttons[i];
                if (b == except) continue;

                b.ClearState();
            }
        }
    }
}