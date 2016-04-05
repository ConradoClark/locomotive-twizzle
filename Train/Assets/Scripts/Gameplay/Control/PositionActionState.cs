using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Control
{
    public class PositionActionState
    {
        public Vector2 Position { get; private set; }
        public bool IsHovering { get; private set; }
        public bool IsMainActionPressedOnObject { get; private set; }
        public bool IsMainActionReleasedOnObject { get; private set; }
        public bool IsMainActionHeldDown { get; private set; }
        public MapActor[] AffectedObjects{ get; private set; }

        public PositionActionState(Vector2 position, bool isHovering, bool isMainActionPressedOnObject, bool isMainActionReleasedOnObject, bool isMainActionHeldDown, MapActor[] affectedObjects)
        {
            this.Position = position;
            this.IsHovering = isHovering;
            this.IsMainActionPressedOnObject = isMainActionPressedOnObject;
            this.IsMainActionReleasedOnObject = isMainActionReleasedOnObject;
            this.IsMainActionHeldDown = isMainActionHeldDown;
            this.AffectedObjects = affectedObjects;
        }
    }
}
