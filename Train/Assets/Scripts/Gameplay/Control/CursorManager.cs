using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Gameplay.Helper;

public class CursorManager : MonoBehaviour
{
    private GameObject cursorObject;
    private MouseControl mouseControl;

    public GameObject HandCursorPrefab;
    public GameObject[] CursorPrefabs;
    private List<ItemCursor> cursors;

    private GameManager gameManager;
    private ItemCursor activeCursor;
    private ItemCursor handCursor;

    void Awake()
    {
        Cursor.visible = false;
        this.cursorObject = new GameObject("CursorObject");
        this.cursors = new List<ItemCursor>();
    }

    void Start()
    {
        this.mouseControl = this.GetComponent<MouseControl>();
        this.gameManager = this.GetComponent<GameManager>();

        if (this.HandCursorPrefab != null)
        {
            GameObject handCursorObject = Instantiate(this.HandCursorPrefab);
            handCursorObject.transform.SetParent(this.cursorObject.transform, false);
            this.handCursor = handCursorObject.GetComponent<ItemCursor>();
            this.cursors.Add(this.handCursor);
        }
        foreach(var cursorPrefab in CursorPrefabs)
        {
            GameObject cursorObject = Instantiate(cursorPrefab);
            cursorObject.transform.SetParent(this.cursorObject.transform, false);
            this.cursors.Add(cursorObject.GetComponent<ItemCursor>());
        }
        activeCursor = cursors.FirstOrDefault();
    }

    void Update()
    {
        if (mouseControl == null || !mouseControl.IsMouseEnabled) return;

        var mousePosition = this.mouseControl.GetWorldMousePosition();
        for (int i = 0; i < cursors.Count; i++)
        {
            cursors[i].transform.position = mousePosition + (cursors[i].IsUsingHand ? cursors[i].OffsetWithHand : cursors[i].Offset);
        }

        Rect rect = new Rect(new Vector2(-Screen.currentResolution.width / 2 + 1, -Screen.currentResolution.height / 2 + 1),
            new Vector2(Screen.currentResolution.width - 2, Screen.currentResolution.height - 2));

        Rect gameMapRect = RectTransformHelper.GetRectInWorldPosition(this.gameManager.MapGrid.MapPartTransform);

        var lastActivatedItem = gameManager.Inventory.GetLastActivatedItem();

        activeCursor = gameMapRect.Contains(mousePosition) && lastActivatedItem != null ? cursors.FirstOrDefault(cur => cur.ItemName == lastActivatedItem.ReferenceName)
                            : this.handCursor;

        EnableActiveCursor();

        //Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMax, rect.yMin, 0), Color.green);
        //Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMin, rect.yMax, 0), Color.green);
        //Debug.DrawLine(new Vector3(rect.xMin, rect.yMax, 0), new Vector3(rect.xMax, rect.yMax, 0), Color.green);
        //Debug.DrawLine(new Vector3(rect.xMax, rect.yMin, 0), new Vector3(rect.xMax, rect.yMax, 0), Color.green);

        Cursor.visible = !rect.Contains(mousePosition) || activeCursor == null;
    }

    private void EnableActiveCursor()
    {
        if (activeCursor == null) return;
        bool showHand = this.gameManager.MapGrid.InputStatesOnMap.Any(i => i.IsHovering && i.AffectedObjects.Any(a => a.IsFloating)) ||
           this.gameManager.MapGrid.IsAnyFloatingObjectBeingDragged();

        DisableAllCursors();
        activeCursor.Enable();
        activeCursor.Animate(showHand);
    }

    private void DisableAllCursors()
    {
        cursors.ForEach(c => c.Disable());
    }
}