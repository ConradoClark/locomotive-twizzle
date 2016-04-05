using Assets.Scripts.Gameplay.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapActor : MonoBehaviour
{
    public bool Started { get; protected set; }
    public bool HasFinished { get; protected set; }
    public int LayerPriority;

    protected GameObject mainGame;
    protected GameManager gameManager;
    protected Queue<MapActions> queuedActions;
    public MapFloater Floater { get; private set; }
    public MapObjectCell Cell { get; set; }
    public RectTransform RectTransform { get; set; }
    public MapTurns Turns { get; private set; }

    public Vector2 Offset{ get; set; }

    public bool IsFloating
    {
        get
        {
            return Floater == null ? false : Floater.IsFloating;
        }
    }

    public void Restart()
    {
        Started = false;
    }

    protected virtual void Start()
    {
        this.RectTransform = this.GetComponent<RectTransform>();
        this.mainGame = GameObject.Find(Constants.GameObjects.Game);
        this.gameManager = mainGame.GetComponent<GameManager>();
        this.queuedActions = new Queue<MapActions>();
        this.Floater = this.GetComponent<MapFloater>();
        this.Cell = this.GetComponentInChildren<MapObjectCell>();
        this.Turns = this.GetComponent<MapTurns>();
        StartCoroutine(HandleActions());
    }

    private IEnumerator HandleActions()
    {
        while (true)
        {
            if (this.enabled && this.queuedActions.Count > 0)
            {
                var action = this.queuedActions.Dequeue();

                switch (action)
                {
                    case MapActions.HitByHammer:
                        WhenHitByHammer();
                        break;
                }
            }
            yield return 1;
        }
    }

    public virtual void FinishTurn()
    {
    }

    public void NotifyAction(MapActions action)
    {
        this.queuedActions.Enqueue(action);
    }

    protected virtual void WhenHitByHammer()
    {

    }
}