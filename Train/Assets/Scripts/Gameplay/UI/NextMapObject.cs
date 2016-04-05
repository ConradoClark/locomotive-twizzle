using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class NextMapObject : MonoBehaviour
{
    public bool Enabled;
    public int QueueSize;
    public float IconDistance;

    private GameManager gameManager;
    private Coroutine nextMapObjectCoroutine;
    private Queue<MapTurns> objectsSequence;
    private Dictionary<MapTurns, Queue<Coroutine>> objectsMoving;

    void Start()
    {
        this.gameManager = GameManager.GetGameManager();
        this.objectsSequence = new Queue<MapTurns>();
        this.objectsMoving = new Dictionary<MapTurns, Queue<Coroutine>>();
    }

    void Update()
    {
        if (this.Enabled && nextMapObjectCoroutine == null && gameManager.GameHasStarted)
        {
            this.nextMapObjectCoroutine = this.StartCoroutine(CheckNextMapObject());
        }
    }

    IEnumerator CheckNextMapObject()
    {
        while (this.Enabled)
        {
            var turnsInOrder = this.gameManager.MapGrid.GetActorsByComponent<MapTurns>().Where(turns => turns.Value.turns > 0).OrderBy(turns => turns.Value.currentTurn);

            if (!turnsInOrder.Select(turns => turns.Value).SequenceEqual(objectsSequence))
            {
                Queue<MapTurns> newSequence = new Queue<MapTurns>();
                int i = 0;
                foreach (var obj in turnsInOrder)
                {
                    if (i == this.QueueSize) break;

                    newSequence.Enqueue(obj.Value);

                    if (!objectsMoving.ContainsKey(obj.Value))
                    {
                        objectsMoving[obj.Value] = new Queue<Coroutine>();
                    }

                    if (objectsSequence.Contains(obj.Value))
                    {
                        objectsMoving[obj.Value].Enqueue(StartCoroutine(MoveActorToPosition(obj.Value, i)));
                    }
                    else
                    {
                        objectsMoving[obj.Value].Enqueue(StartCoroutine(CreateActorInPosition(obj.Value, i)));
                    }

                    i++;
                }

                foreach (var removed in objectsSequence.Where(obj => !newSequence.Contains(obj)))
                {
                    objectsMoving[removed].Enqueue(StartCoroutine(RemoveActor(removed)));
                }
                objectsSequence.Clear();
                foreach (var obj in newSequence)
                {
                    objectsSequence.Enqueue(obj);
                }
            }

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }
        this.nextMapObjectCoroutine = null;
    }

    IEnumerator MoveActorToPosition(MapTurns actor, int position)
    {
        yield return new WaitForEndOfFrame();
        Coroutine thisCoroutine = objectsMoving[actor].Last();

        while (objectsMoving[actor].Peek() != thisCoroutine)
        {
            yield return false;
        }

        Vector2 actorPosition;
        Vector2 initialActorPosition = Vector2.zero;
        Vector2 desiredPosition = new Vector3(position * this.IconDistance, 0, 0);
        float desiredScale = position == 0 ? actor.GetInitialScale() : actor.GetInitialScale() - Constants.Sizes.NextItemScaleFactor;
        float initialScale = actor.GetInitialScale();
        float currentScale = initialScale;
        float time = 0f;
        while (actor.TryGetPosition(out actorPosition) && actor.TryGetScale(out currentScale) && (
            Vector2.Distance(actorPosition, desiredPosition) > Constants.Distances.MinimumDesiredDistanceToStop
            || Mathf.Abs(currentScale-desiredScale) > Constants.Distances.MinimumDesiredDistanceToStop))
        {
            if (time == 0f)
            {
                initialActorPosition = actorPosition;
                actor.TryGetScale(out initialScale);
            }
            var posX = Mathf.SmoothStep(initialActorPosition.x, desiredPosition.x, time*2);
            var posY = Mathf.SmoothStep(initialActorPosition.y, desiredPosition.y, time*2);
            actor.MoveObjectIcon(new Vector2(posX, posY));

            var scaleStep = Mathf.SmoothStep(initialScale, desiredScale, time*2);
            actor.ScaleObjectIcon(scaleStep);

            time += Time.deltaTime;
            yield return true;
        }

        objectsMoving[actor].Dequeue();
    }

    IEnumerator CreateActorInPosition(MapTurns actor, int position)
    {
        yield return new WaitForEndOfFrame();
        Coroutine thisCoroutine = objectsMoving[actor].Last();

        while (objectsMoving[actor].Peek() != thisCoroutine)
        {
            yield return false;
        }

        actor.CreateObjectIcon(this.transform);
        Vector2 desiredPosition = new Vector3(position * this.IconDistance, 0, 0);
        float desiredScale = position == 0 ? actor.GetInitialScale() : actor.GetInitialScale() - Constants.Sizes.NextItemScaleFactor;
        actor.MoveObjectIcon(desiredPosition);
        actor.ScaleObjectIcon(desiredScale);

        objectsMoving[actor].Dequeue();
    }

    IEnumerator RemoveActor(MapTurns actor)
    {
        yield return new WaitForEndOfFrame();
        Coroutine thisCoroutine = objectsMoving[actor].Last();

        while (objectsMoving[actor].Peek() != thisCoroutine)
        {
            yield return false;
        }

        actor.DestroyIcon();
        objectsMoving[actor].Dequeue();
    }
}