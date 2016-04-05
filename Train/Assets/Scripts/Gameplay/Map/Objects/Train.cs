using Assets.Scripts.Gameplay;
using Assets.Scripts.Gameplay.Helper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Train : MapActor
{
    private MapTurns turns;
    private Directions currentDirection;
    private RectTransform rectTransform;
    private bool IsMoving;
    private Queue<Coroutine> moveCoroutines;
    private Queue<Coroutine> blinkCoroutines;
    private Queue<Coroutine> hammerCdCoroutines;
    private SpriteRenderer spriteRenderer;
    private MapHealth health;

    public GameObject HitByHammerEffect;
    public Color HitByHammerBlinkingColor;
    private List<Directions> possibleDirectionsInOrder;

    protected override void Start()
    {
        base.Start();
        turns = this.GetComponent<MapTurns>();
        this.rectTransform = this.GetComponent<RectTransform>();
        this.currentDirection = Directions.Right;
        this.moveCoroutines = new Queue<Coroutine>();
        this.blinkCoroutines = new Queue<Coroutine>();
        this.hammerCdCoroutines = new Queue<Coroutine>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.health = this.GetComponent<MapHealth>();

        this.possibleDirectionsInOrder = System.Enum.GetValues(typeof(Directions)).OfType<Directions>().ToList();
    }

    private void Update()
    {
        if (gameManager.CurrentGameState == GameStates.WorldTurn && !Started)
        {
            if (turns.currentTurn == 0 && this.health.GetCurrentHealth() > 0)
            {
                Started = true;
                this.moveCoroutines.Enqueue(StartCoroutine(Move()));
                return;
            }
            this.HasFinished = true;
        }
    }

    private IEnumerator Move()
    {
        while (this.IsMoving)
        {
            yield return 0;
        }
        this.IsMoving = true;
        float time = Time.time;
        float desiredYPosition = this.rectTransform.anchoredPosition.y + (this.currentDirection == Directions.Up ? Constants.Sizes.CellSize.y :
            (this.currentDirection == Directions.Down ? -Constants.Sizes.CellSize.y : 0f));

        float desiredXPosition = this.rectTransform.anchoredPosition.x + (this.currentDirection == Directions.Right ? Constants.Sizes.CellSize.x :
            (this.currentDirection == Directions.Left ? -Constants.Sizes.CellSize.x : 0f));

        this.Cell.Column = this.Cell.Column + (this.currentDirection == Directions.Right ? 1 : (this.currentDirection == Directions.Left ? -1 : 0));
        this.Cell.Row = this.Cell.Row + (this.currentDirection == Directions.Up ? -1 : (this.currentDirection == Directions.Down ? 1 : 0));

        var currentTrack = this.GetCurrentTrack();
        this.HandleDerailment(currentTrack);
        this.AdjustMovementDirection(currentTrack);

        while (Mathf.Abs(this.rectTransform.anchoredPosition.y - desiredYPosition) > 0.01f ||
            Mathf.Abs(this.rectTransform.anchoredPosition.x - desiredXPosition) > 0.01f)
        {
            if (Time.time - time > 0.2f)
            {
                this.HasFinished = true;
            }

            float yVariation = Mathf.SmoothStep(this.rectTransform.anchoredPosition.y, desiredYPosition, (Time.time - time) * Constants.Velocity.TrainSpeed);
            float xVariation = Mathf.SmoothStep(this.rectTransform.anchoredPosition.x, desiredXPosition, (Time.time - time) * Constants.Velocity.TrainSpeed);
            yield return this.rectTransform.anchoredPosition += new Vector2(xVariation - this.rectTransform.anchoredPosition.x, yVariation - this.rectTransform.anchoredPosition.y);
        }

        this.moveCoroutines.Dequeue();
        if (moveCoroutines.Count == 0)
        {
            this.HasFinished = true;
        }

        this.IsMoving = false;
        yield return true;
    }

    public override void FinishTurn()
    {
        if (this.turns.currentTurn != 0) return;
        this.turns.Restart();
        this.HasFinished = this.Started = false;
    }

    protected override void WhenHitByHammer()
    {
        if (blinkCoroutines.Count > 0) return;

        this.health.Damage(Constants.Stats.HammerDamageToTrain);

        this.gameManager.ScoreManager.IncreaseScore(Constants.Score.HitTrainWithHammer);

        if (HitByHammerEffect != null)
        {
            var effect = Instantiate(HitByHammerEffect);
            effect.transform.SetParent(this.transform, false);
        }
        this.blinkCoroutines.Enqueue(StartCoroutine(Blink()));
        this.turns.CanTick = false;
        gameManager.TickTimers();
        this.turns.CanTick = true;
    }

    private IEnumerator Blink()
    {
        int i = 0;
        while (i < 3)
        {
            this.spriteRenderer.color = this.HitByHammerBlinkingColor;
            yield return new WaitForSeconds(Constants.Intervals.BlinkingInterval);
            this.spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(Constants.Intervals.BlinkingInterval);
            i++;
        }
        this.blinkCoroutines.Dequeue();
    }

    private IEnumerator Derail()
    {
        yield break;
    }

    private Track GetCurrentTrack()
    {
        return gameManager.MapGrid.GetActorsByGridPosition(this.Cell.Row, this.Cell.Column).FirstOrDefault(actor => actor is Track) as Track;
    }

    private void HandleDerailment(Track track)
    {   
        bool isDerailing = track == null || !track.AllowedDirections.Contains(this.currentDirection);

        if (isDerailing)
        {
            this.health.Damage((uint) this.health.Health);
            this.StartCoroutine(Derail());
        }
    }

    private void AdjustMovementDirection(Track track)
    {
        if (track == null) return;

        switch (track.DirectionChange)
        {
            case DirectionChanges.Clockwise:
                var newDirectionIndex = (this.possibleDirectionsInOrder.IndexOf(this.currentDirection) + 1) % this.possibleDirectionsInOrder.Count;
                this.currentDirection = this.possibleDirectionsInOrder[newDirectionIndex];
                break;
            case DirectionChanges.Counterclockwise:
                var newDirectionIndexCc = (this.possibleDirectionsInOrder.IndexOf(this.currentDirection) - 1) % this.possibleDirectionsInOrder.Count;
                newDirectionIndexCc = newDirectionIndexCc < 0 ? this.possibleDirectionsInOrder.Count - 1 : newDirectionIndexCc;
                this.currentDirection = this.possibleDirectionsInOrder[newDirectionIndexCc];
                break;
            default:
                break;
        }
    }
}