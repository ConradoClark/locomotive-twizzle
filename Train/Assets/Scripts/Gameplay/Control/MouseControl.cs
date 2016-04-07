using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Gameplay.Control;
using System;
using Assets.Scripts.Gameplay.Items;
using System.Collections.Generic;
using Assets.Scripts.Gameplay.Helper;

public class MouseControl : MonoBehaviour, IInputControl
{
    private GameManager gameManager;
    private GameInventory inventory;

    public bool IsMouseEnabled;

    // Use this for initialization
    void Start()
    {
#if UNITY_ANDROID
        //this.IsMouseEnabled = false;
#endif
        this.gameManager = GameManager.GetMainGame().GetComponent<GameManager>();
        this.inventory = gameManager.GetComponent<GameInventory>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2 GetWorldMousePosition()
    {
        if (!IsMouseEnabled) return Vector2.zero;
        var mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        return mousePosition;
    }

    public Vector2 GetScreenMousePosition()
    {
        if (!IsMouseEnabled) return Vector2.zero;
        return Input.mousePosition;
    }

    public ItemActionState GetInputStateOnItem(ItemRender item)
    {
        RectTransform transform = (RectTransform)item.transform;
        var mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

        var position = new Vector2(transform.position.x, transform.position.y - item.Rect.size.y / 2); // Map part based on center, so it's size/2 to get the right pos
        var rect = new Rect(position, item.Rect.size);

        bool isHovering = rect.Contains(mousePosition);
        bool isUsingMainAction = isHovering && Input.GetMouseButtonDown(Constants.Input.LeftMouseButton);

        return new ItemActionState(isHovering, isUsingMainAction);
    }

    public PositionActionState<T>[] GetInputStatesOnPosition<T>(GameObject boundaries) where T:MonoBehaviour
    {
        var rectTransform = boundaries.GetComponent<RectTransform>();
        if (rectTransform == null) return new PositionActionState<T>[0];

        var mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        var actualRect = RectTransformHelper.GetRectInWorldPosition(rectTransform);

        bool isHovering = actualRect.Contains(mousePosition);
        bool isUsingMainAction = isHovering && Input.GetMouseButtonDown(Constants.Input.LeftMouseButton);
        bool isReleasingMainAction = isHovering && Input.GetMouseButtonUp(Constants.Input.LeftMouseButton);
        bool isMainActionHeldDown = Input.GetMouseButton(Constants.Input.LeftMouseButton);

        if (isReleasingMainAction)
        {
            Debug.Log("Released Main Action Button! " + boundaries.name);
        }

        T[] objects = boundaries.transform.OfType<RectTransform>()
                                                   .Concat(new[] { rectTransform })
                                                   .Where(child => RectTransformHelper.GetRectInWorldPosition(child).Contains(mousePosition))
                                                   .Select(t => t.gameObject.GetComponent<T>())
                                                   .Where(actor=>actor!= null)
                                                   .ToArray();

        foreach (var obj in objects.Select(obj => obj.GetComponent<RectTransform>()))
        {
            var rect = RectTransformHelper.GetRectInWorldPosition(obj);
            Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMax, rect.yMin, 0), Color.blue);
            Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMin, rect.yMax, 0), Color.blue);
            Debug.DrawLine(new Vector3(rect.xMin, rect.yMax, 0), new Vector3(rect.xMax, rect.yMax, 0), Color.blue);
            Debug.DrawLine(new Vector3(rect.xMax, rect.yMin, 0), new Vector3(rect.xMax, rect.yMax, 0), Color.blue);
        }

        return new PositionActionState<T>[1] { new PositionActionState<T>(mousePosition, isHovering, isUsingMainAction, isReleasingMainAction, isMainActionHeldDown, objects) };
    }

}
