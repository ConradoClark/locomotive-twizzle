using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Gameplay.Helper
{
    public class RectTransformHelper
    {
        public static Rect GetRectInWorldPosition(RectTransform transform)
        {
            var rect = new Rect(transform.position.x - transform.rect.size.x / 2, transform.position.y - transform.rect.size.y / 2, transform.rect.size.x, transform.rect.size.y);
            //Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMax, rect.yMin, 0),Color.green);
            //Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMin, rect.yMax, 0), Color.green);
            //Debug.DrawLine(new Vector3(rect.xMin, rect.yMax, 0), new Vector3(rect.xMax, rect.yMax, 0), Color.green);
            //Debug.DrawLine(new Vector3(rect.xMax, rect.yMin, 0), new Vector3(rect.xMax, rect.yMax, 0), Color.green);
            return rect;
        }
    }
}
