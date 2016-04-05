using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Gameplay.Helper;
using Assets.Scripts.Gameplay.Map;

public class Hammer : MonoBehaviour
{
    private ItemState state;
    private GameManager gameManager;
    private ControlsManager controls;

    // Use this for initialization
    void Start()
    {
        this.state = this.GetComponent<ItemState>();
        this.gameManager = GameManager.GetMainGame().GetComponent<GameManager>();
        this.controls = gameManager.GetComponent<ControlsManager>();
        this.state.ReferenceName = Constants.Items.Hammer;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.state.CurrentState == ItemState.State.Active)
        {
            CheckHits();
        }
    }

    void CheckHits()
    {
        var inputStates = gameManager.MapGrid.InputStatesOnMap;
        foreach (var validState in inputStates.Where(i => i.IsMainActionReleasedOnObject))
        {
            var affectedObject = validState.AffectedObjects.Select(obj => obj.GetComponent<MapActor>())
                                                          .Where(comp => comp != null)
                                                          .OrderByDescending(actor => actor.IsFloating)
                                                          .ThenByDescending(actor => actor.LayerPriority)
                                                          .FirstOrDefault();

            if (affectedObject == null) return;
            if (affectedObject.IsFloating)
            {
                var overlappingObjects = gameManager.MapGrid.GetOverlappingFloatingObjects(affectedObject.Floater);
                if (overlappingObjects.Except(new[] { affectedObject.Floater }).Any(o => o.IsInDragMode)) return;
            }

            affectedObject.NotifyAction(MapActions.HitByHammer);
        }
    }
}
