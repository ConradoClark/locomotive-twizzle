using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Assets.Scripts.Gameplay.Helper;
using Assets.Scripts.Gameplay.Control;

public class MapFloater : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite floatingSprite;
    public Sprite invalidBlinkSprite;
    private Sprite originalSprite;
    private int originalLayeringOrder;
    private GameManager gameManager;
    private MapObjectCell mapObjectCell;
    private float fCount = 0f;
    private bool holdingWhenReady;
    private Queue<Coroutine> pushawayCoroutines;
    private Queue<Coroutine> blinkCoroutines;
    private PositionActionState<MapActor>[] inputStates;
    private MapActor mapActor;

    public bool IsDraggable;
    public Vector2 FloatingSpriteOffset;

    public bool IsFloating { get; protected set; }
    public bool IsReadyToPickUp { get; protected set; }
    public bool IsInDragMode { get; protected set; }
    public bool IsDragging { get; protected set; }
    public RectTransform RectTransform { get; protected set; }

    // Use this for initialization
    void Start()
    {
        this.mapActor = this.GetComponent<MapActor>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        if (this.spriteRenderer == null)
        {
            this.spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
        }

        if (this.spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on MapFloater gameObject: " + gameObject.name);
            this.enabled = false;
            return;
        }

        if (this.floatingSprite == null)
        {
            Debug.LogError("No Floating Sprite found on MapFloater gameObject: " + gameObject.name);
            this.enabled = false;
            return;
        }
        this.originalSprite = this.spriteRenderer.sprite;
        this.originalLayeringOrder = this.spriteRenderer.sortingOrder;
        this.mapObjectCell = this.GetComponentInChildren<MapObjectCell>();
        this.RectTransform = this.GetComponent<RectTransform>();
        this.gameManager = GameManager.GetGameManager();
        this.pushawayCoroutines = new Queue<Coroutine>();
        this.blinkCoroutines = new Queue<Coroutine>();
    }

    // Update is called once per frame
    void Update()
    {
        this.inputStates = gameManager.Controls.GetInputStatesOnPosition<MapActor>(this.gameObject);
        if (this.IsReadyToPickUp && this.IsFloating && !this.IsInDragMode && this.IsDraggable)
        {
            if (CheckDragMode()) return;
        }

        if (this.IsFloating && !this.IsInDragMode && pushawayCoroutines.Count == 0)
        {
            var firstOverlappingFloater = this.gameManager.MapGrid.GetOverlappingFloatingObjects(this).Except(new[] { this }).FirstOrDefault();

            if (firstOverlappingFloater == null || firstOverlappingFloater.IsInDragMode) return;

            var thisRect = RectTransformHelper.GetRectInWorldPosition(this.RectTransform);
            var overlappingRect = RectTransformHelper.GetRectInWorldPosition(firstOverlappingFloater.RectTransform);

            var distance = (thisRect.center - overlappingRect.center);
            if (distance.magnitude > Constants.Distances.MinimumFloatingDistanceToPushaway) return;

            distance = new Vector2(distance.x + (distance.x >= 0 ? -(thisRect.size.x / 2) : (thisRect.size.x / 2)),
                                    distance.y + (distance.y >= 0 ? (thisRect.size.y / 2) : -(thisRect.size.y / 2)));
            pushawayCoroutines.Enqueue(StartCoroutine(Pushaway(distance)));
        }
    }

    void LateUpdate()
    {
        Rect gameMapRect = RectTransformHelper.GetRectInWorldPosition(gameManager.MapGrid.MapPartTransform);
        var halfSizeX = this.RectTransform.rect.size.x / 2;
        var halfSizeY = this.RectTransform.rect.size.y / 2;
        this.RectTransform.anchoredPosition = new Vector2(Mathf.Clamp(this.RectTransform.anchoredPosition.x, 0 + halfSizeX, gameMapRect.size.x - halfSizeX), Mathf.Clamp(this.RectTransform.anchoredPosition.y, -gameMapRect.size.y + halfSizeY, -halfSizeY));
    }

    bool CheckDragMode()
    {
        var inputState = inputStates.Where(state => (holdingWhenReady ? state.IsMainActionHeldDown : (state.IsMainActionPressedOnObject && state.IsHovering)))
                                    .OrderBy(state => state.Position - new Vector2(this.RectTransform.position.x, this.RectTransform.position.y))
                                    .FirstOrDefault();
        if (inputState == null)
        {
            this.IsDragging = false;
            return false;
        }

        this.holdingWhenReady = false;
        this.IsInDragMode = true;
        StartCoroutine(Drag());

        return true;
    }

    IEnumerator Drag()
    {
        float time = Time.time;
        float realTime = Time.time;
        var initialPosition = this.RectTransform.position;
        var desiredPosition = initialPosition;

        while (this.IsInDragMode)
        {
            var inputState = inputStates.Where(state => state.IsMainActionHeldDown)
                                        .OrderBy(state => state.Position - new Vector2(this.RectTransform.position.x, this.RectTransform.position.y))
                                        .FirstOrDefault();
            if (inputState == null)
            {
                time = Time.time;
                while (this.IsFloating && Vector2.Distance(this.RectTransform.position, desiredPosition) > Constants.Distances.MinimumDistanceToDrag && Time.time - time < 0.3f)
                {
                    float xPosAfterStep = Mathf.Lerp(this.RectTransform.position.x, desiredPosition.x, (Time.time - time) * Constants.Velocity.FloatingDragSpeed);
                    float yPosAfterStep = Mathf.Lerp(this.RectTransform.position.y, desiredPosition.y, (Time.time - time) * Constants.Velocity.FloatingDragSpeed);
                    yield return this.RectTransform.anchoredPosition += new Vector2(xPosAfterStep - this.RectTransform.position.x, yPosAfterStep - this.RectTransform.position.y);
                }

                this.IsInDragMode = false;
                yield break;
            }

            desiredPosition = inputState.Position;

            if (Vector2.Distance(this.RectTransform.position, inputState.Position) < Constants.Distances.MinimumDistanceToDrag)
            {
                time = Time.time;
            }

            if (Vector2.Distance(this.RectTransform.position, initialPosition) > Constants.Distances.MinimumDistanceToDrag || Time.time - realTime > Constants.Intervals.MinimumTimeToDrag)
            {
                this.IsDragging = true;
            }

            if (this.IsFloating)
            {
                float xPosAfterStep = Mathf.Lerp(this.RectTransform.position.x, inputState.Position.x, (Time.time - time) * Constants.Velocity.FloatingDragSpeed);
                float yPosAfterStep = Mathf.Lerp(this.RectTransform.position.y, inputState.Position.y, (Time.time - time) * Constants.Velocity.FloatingDragSpeed);
                yield return this.RectTransform.anchoredPosition += new Vector2(xPosAfterStep - this.RectTransform.position.x, yPosAfterStep - this.RectTransform.position.y);
            }
            else
            {
                this.IsInDragMode = false;
                yield break;
            }
        }
    }

    public void InvalidBlink()
    {
        if (this.blinkCoroutines.Count == 0)
        {
            this.blinkCoroutines.Enqueue(this.StartCoroutine(Blink()));
        }

    }

    IEnumerator Blink()
    {
        int i = 0;
        while (i < 3)
        {
            this.spriteRenderer.sprite = this.invalidBlinkSprite;
            yield return new WaitForSeconds(Constants.Intervals.BlinkingInterval);
            this.spriteRenderer.sprite = this.IsFloating ? this.floatingSprite : this.originalSprite;
            yield return new WaitForSeconds(Constants.Intervals.BlinkingInterval);
            i++;
        }
        this.blinkCoroutines.Dequeue();
    }

    IEnumerator Pushaway(Vector2 distance)
    {
        var desiredPosition = this.RectTransform.anchoredPosition + distance;
        float time = Time.time;
        while (this.IsFloating &&
           (Mathf.Abs(this.RectTransform.anchoredPosition.y - desiredPosition.y) > Constants.Distances.MinimumDesiredDistanceToStop ||
           Mathf.Abs(this.RectTransform.anchoredPosition.x - desiredPosition.x) > Constants.Distances.MinimumDesiredDistanceToStop))
        {
            var elapsedTime = (Time.time - time);
            if (elapsedTime > 0.8f)
            {
                pushawayCoroutines.Dequeue();
                yield break;
            }
            float yVariation = Mathf.SmoothStep(this.RectTransform.anchoredPosition.y, desiredPosition.y, elapsedTime * Constants.Velocity.FloatingPushSpeed);
            float xVariation = Mathf.SmoothStep(this.RectTransform.anchoredPosition.x, desiredPosition.x, elapsedTime * Constants.Velocity.FloatingPushSpeed);
            yield return this.RectTransform.anchoredPosition += new Vector2(xVariation - this.RectTransform.anchoredPosition.x, yVariation - this.RectTransform.anchoredPosition.y);
        }
        pushawayCoroutines.Dequeue();
    }

    private IEnumerator FloatRoutine()
    {
        this.holdingWhenReady = false;
        float time = Time.time;
        float desiredYPosition = this.RectTransform.anchoredPosition.y + Constants.Positions.InitialFloatYOffset;
        float desiredXPosition = this.RectTransform.anchoredPosition.x + ((this.mapObjectCell.Column != Constants.Sizes.GridColumnCount - 1) ? Constants.Positions.InitialFloatXOffset : 0);
        yield return 1;

        while (this.IsFloating && Time.time - time < 0.3f &&
            (Mathf.Abs(this.RectTransform.anchoredPosition.y - desiredYPosition) > Constants.Distances.MinimumDesiredDistanceToStop ||
            Mathf.Abs(this.RectTransform.anchoredPosition.x - desiredXPosition) > Constants.Distances.MinimumDesiredDistanceToStop))
        {
            float yVariation = Mathf.Lerp(this.RectTransform.anchoredPosition.y, desiredYPosition, (Time.time - time) * Constants.Velocity.FloatingSpeed);
            float xVariation = Mathf.Lerp(this.RectTransform.anchoredPosition.x, desiredXPosition, (Time.time - time) * Constants.Velocity.FloatingSpeed);

            yield return this.RectTransform.anchoredPosition += new Vector2(xVariation - this.RectTransform.anchoredPosition.x, yVariation - this.RectTransform.anchoredPosition.y);
        }

        this.IsReadyToPickUp = true;
        //this.holdingWhenReady = inputStates.Any(state => state.IsMainActionHeldDown);

        while (this.IsFloating)
        {
            if (this.IsInDragMode)
            {
                yield return false;
                continue;
            }
            this.RectTransform.anchoredPosition += new Vector2(0, Mathf.Cos(fCount) / Constants.Velocity.FloatingWaveLength);
            fCount += (Constants.Velocity.FloatingAnimationSpeed * Time.deltaTime * Mathf.Deg2Rad) % (2 * Mathf.PI);
            yield return fCount;
        }
    }

    public void Float()
    {
        this.spriteRenderer.sprite = floatingSprite;
        this.IsFloating = true;
        this.fCount = 0f;
        this.spriteRenderer.sortingOrder = Constants.Ordering.FloatingObjectOrder;
        StartCoroutine(FloatRoutine());
    }

    public void Unfloat()
    {
        this.spriteRenderer.sprite = originalSprite;
        this.IsFloating = this.IsReadyToPickUp = false;
        this.spriteRenderer.sortingOrder = this.originalLayeringOrder;
        this.RectTransform.anchoredPosition = new Vector3((Constants.Sizes.CellSize.x / 2) + this.mapObjectCell.Column * Constants.Sizes.CellSize.x + (mapActor == null ? 0f : mapActor.Offset.x),
                (Constants.Sizes.CellSize.y / 2) - (this.mapObjectCell.Row + 1) * (Constants.Sizes.CellSize.y) + (mapActor == null ? 0f : mapActor.Offset.y), 0);
    }

    public void Unfloat(int row, int column)
    {
        this.mapObjectCell.Row = row;
        this.mapObjectCell.Column = column;
        this.Unfloat();
    }
}