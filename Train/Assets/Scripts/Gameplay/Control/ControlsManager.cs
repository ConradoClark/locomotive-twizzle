using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Gameplay.Control;
using System.Linq;
using System;
using Assets.Scripts.Gameplay.Items;

public class ControlsManager : MonoBehaviour, IInputControl
{
    private GameManager gameManager;
    private List<IInputControl> inputControls;

    public bool AdaptToTouch;

    void Start()
    {
        this.gameManager = GameManager.GetMainGame().GetComponent<GameManager>();
        this.inputControls = new List<IInputControl>();
        this.inputControls.Add(this.GetComponent<MouseControl>());
    }

    void Update()
    {

    }

    public ItemActionState GetInputStateOnItem(ItemRender item)
    {
        if (this.gameManager.CurrentGameState == Assets.Scripts.Gameplay.GameStates.PlayerTurn)
        {
            var inputStates = inputControls.Select(ic => ic.GetInputStateOnItem(item));

            return new ItemActionState(inputStates.Any(i => i.IsHovering), inputStates.Any(i => i.IsUsingMainAction));
        }
        return new ItemActionState(false, false);
    }

    public PositionActionState[] GetInputStatesOnPosition(GameObject boundaries)
    {
        return inputControls.SelectMany(ic => ic.GetInputStatesOnPosition(boundaries)).ToArray();
    }
}
