using UnityEngine;
using System.Linq;
using Assets.Scripts.Gameplay.Control;

public class Inventory : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject inventorySlots;

    private PositionActionState<InventoryClickable>[] inputStatesOnInventory;
    public PositionActionState<InventoryClickable>[] InputStatesOnInventory
    {
        get
        {
            if (inputStatesOnInventory == null)
            {
                inputStatesOnInventory = gameManager.Controls.GetInputStatesOnPosition<InventoryClickable>(this.gameObject).Concat(
                    gameManager.Controls.GetInputStatesOnPosition<InventoryClickable>(this.inventorySlots)).ToArray();
            }
            return inputStatesOnInventory;
        }
        set
        {
            this.inputStatesOnInventory = value;
        }
    }

    void Start()
    {
        this.gameManager = GameManager.GetGameManager();
        this.inventorySlots = this.transform.FindChild("InventorySlots").gameObject;
    }

    void Update()
    {
        this.InputStatesOnInventory = null;
    }
}