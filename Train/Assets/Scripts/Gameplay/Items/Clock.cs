using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Gameplay.Control;

public class Clock : MonoBehaviour
{
    private ItemState state;
    private GameManager gameManager;
    private ControlsManager controls;
    bool burningTime = false;

    void Start()
    {
        this.state = this.GetComponent<ItemState>();
        this.state.ReferenceName = Constants.Items.Clock;
        this.gameManager = GameManager.GetMainGame().GetComponent<GameManager>();
        this.controls = gameManager.GetComponent<ControlsManager>();
    }

    void Update()
    {
        if (this.state.CurrentState == ItemState.State.Active && !burningTime)
        {
            if (gameManager.MapGrid.InputStatesOnMap.Any(i => i.IsMainActionPressedOnObject))
            {
                gameManager.TickTimers();
                //StartCoroutine(BurnTime());
            }
        }
    }

    IEnumerator BurnTime()
    {
        burningTime = true;  
        while (gameManager.MapGrid.InputStatesOnMap.Any(i => i.IsMainActionHeldDown))
        {
            if (gameManager.CurrentGameState != Assets.Scripts.Gameplay.GameStates.PlayerTurn)
            {
                yield return false;
                continue;
            }

            //Debug.Log("Waiting 0.5f");
            yield return new WaitForSeconds(0.5f);

            if (this.state.CurrentState == ItemState.State.Active &&
                gameManager.MapGrid.InputStatesOnMap.Any(i => i.IsMainActionHeldDown))
            {
                //Debug.Log("Tick timers because it's still holding");
                gameManager.TickTimers();
                yield return true;
            }
            else
            {
                //Debug.Log("Stopped! cancel action");
                burningTime = false;
                yield break;
            }
        }

        //Debug.Log("Stopped right after action");
        var states = gameManager.MapGrid.InputStatesOnMap;
        this.burningTime = false;
    }
}
