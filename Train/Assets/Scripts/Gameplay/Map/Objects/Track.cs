using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Gameplay.Helper;
using Assets.Scripts.Gameplay.Map;

public class Track : MapActor
{
    private ControlsManager controls;
    public DirectionChanges DirectionChange;

    public GameObject HitByHammerFloatEffect;
    public GameObject HitByHammerUnfloatEffect;

    private Vector2 initialSize;
    public Directions[] AllowedDirections;

    protected override void Start()
    {
        base.Start();
        var renderer = this.GetComponent<SpriteRenderer>();
        this.HasFinished = true;
        this.Started = true;
        this.controls = gameManager.GetComponent<ControlsManager>();
        this.initialSize = this.RectTransform.sizeDelta;
    }

    void Update()
    {
        //var rect = RectTransformHelper.GetRectInWorldPosition(this.RectTransform);
        //Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMax, rect.yMin, 0), Color.green);
        //Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMin, rect.yMax, 0), Color.green);
        //Debug.DrawLine(new Vector3(rect.xMin, rect.yMax, 0), new Vector3(rect.xMax, rect.yMax, 0), Color.green);
        //Debug.DrawLine(new Vector3(rect.xMax, rect.yMin, 0), new Vector3(rect.xMax, rect.yMax, 0), Color.green);
    }

    private bool CheckTrackIsOccupied()
    {
        var objects = gameManager.MapGrid.GetActorsByGridPosition(this.Cell.Row, this.Cell.Column).Except(new[] { this });
        return objects.Any(actor => actor.LayerPriority > this.LayerPriority);
    }

    private bool CheckGridCellIsAvailable(int row, int column)
    {
        return !gameManager.MapGrid.GetActorsByGridPosition(row, column).Except(new[] { this }).Any();
    }

    protected override void WhenHitByHammer()
    {
        if (Floater.IsDragging) return;
        if (Floater.IsFloating)
        {
            int newRow, newColumn;
            var rect = RectTransformHelper.GetRectInWorldPosition(this.RectTransform);
            if (this.gameManager.MapGrid.RelocateInGrid(rect, out newRow, out newColumn))
            {
                if (CheckGridCellIsAvailable(newRow, newColumn))
                {
                    if (HitByHammerUnfloatEffect != null)
                    {
                        var effect = Instantiate(HitByHammerUnfloatEffect);
                        effect.transform.SetParent(this.transform, false);
                    }

                    if (controls.AdaptToTouch)
                    {
                        this.RectTransform.sizeDelta = initialSize;
                    }

                    this.gameManager.ScoreManager.IncreaseScore(Constants.Score.HitTrackWithHammer);

                    Floater.Unfloat(newRow, newColumn);
                }
                else
                {
                    Floater.InvalidBlink();
                    return;
                }
            }
            else
            {
                Floater.InvalidBlink();
                return;
            }
        }
        else
        {
            if (this.CheckTrackIsOccupied())
            {
                Floater.InvalidBlink();
                return;
            }
            if (HitByHammerFloatEffect != null)
            {
                var effect = Instantiate(HitByHammerFloatEffect);
                effect.transform.SetParent(this.transform, false);
            }

            if (controls.AdaptToTouch)
            {
                this.RectTransform.sizeDelta = this.RectTransform.sizeDelta + Constants.Sizes.TrackExtraSizeForTouch;
            }

            this.gameManager.ScoreManager.IncreaseScore(Constants.Score.HitTrackWithHammer);
            Floater.Float();
        }
        gameManager.TickTimers();
    }
}
