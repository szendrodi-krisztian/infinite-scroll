using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Knife
{
    public class CustomSorterGroup : MonoBehaviour
    {
        private readonly List<CustomSorterButton> buttons = new();

        public void ResetAllExcept(CustomSorterButton except)
        {
            foreach (var b in buttons.Where(b => b != except))
            {
                b.ClearState();
            }
        }

        public void Add(CustomSorterButton button)
        {
            buttons.Add(button);
        }
    }
}