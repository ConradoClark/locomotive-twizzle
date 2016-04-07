using Assets.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Control
{
    public interface IInputControl
    {
        ItemActionState GetInputStateOnItem(ItemRender item);
        PositionActionState<T>[] GetInputStatesOnPosition<T>(GameObject boundaries) where T : MonoBehaviour;
    }
}
