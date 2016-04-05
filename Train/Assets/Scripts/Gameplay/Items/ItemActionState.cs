using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Gameplay.Items
{
    public class ItemActionState
    {
        public bool IsHovering { get; private set; }
        public bool IsUsingMainAction { get; private set; }

        public ItemActionState(bool isHovering, bool isUsingMainAction)
        {
            this.IsHovering = isHovering;
            this.IsUsingMainAction = isUsingMainAction;
        }
    }
}
