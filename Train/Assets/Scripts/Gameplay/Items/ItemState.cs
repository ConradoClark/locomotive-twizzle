using UnityEngine;
using System.Collections;
using System.Linq;

public class ItemState : MonoBehaviour
{
    public enum State
    {
        Normal,
        Hover,
        Active,
        Inactive
    }

    public string ReferenceName { get; set; }
    public State CurrentState { get; set; }    
    private ItemRender render;
    private ItemCount itemCountManager;
    private ControlsManager controls;
    private GameInventory inventory;
    private GameObject game;

    void Start()
    {
        this.game = GameManager.GetMainGame();
        this.controls = game.GetComponent<ControlsManager>();
        this.inventory = game.GetComponent<GameInventory>();

        this.render = this.GetComponent<ItemRender>();
        this.itemCountManager = this.GetComponent<ItemCount>();
    }

    void Update()
    {
        if (render == null) return;

        UpdateState();
        UpdateRender();
    }

    void UpdateState()
    {
        if (!this.render.enabled) return;
        var inputState = controls.GetInputStateOnItem(this.render);
        bool canUse = itemCountManager.CanUse();

        if (!canUse)
        {
            this.CurrentState = State.Inactive;
            return;
        }

        if (canUse && new[] { State.Normal, State.Hover }.Contains(this.CurrentState))
        {
            if (inputState.IsUsingMainAction)
            {
                this.inventory.ActivateItem(this.ReferenceName);
                return;
            }

            if (inputState.IsHovering)
            {
                this.CurrentState = State.Hover;
            }
            else
            {
                this.CurrentState = State.Normal;
            }
        }
    }

    void UpdateRender()
    {
        switch (this.CurrentState)
        {
            case State.Normal:
                this.render.SetToNormal();
                break;
            case State.Hover:
                this.render.SetToHover();
                break;
            case State.Active:
                this.render.SetToActive();
                break;
            case State.Inactive:
                this.render.SetToInactive();
                break;
        }
    }
}
