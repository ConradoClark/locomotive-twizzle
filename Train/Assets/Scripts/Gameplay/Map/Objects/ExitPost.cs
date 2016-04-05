using UnityEngine;
using System.Collections;
using Assets.Scripts.Gameplay.Helper;

public class ExitPost : MapActor
{
    public GameObject HitByHammerEffect;

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        //var rect = RectTransformHelper.GetRectInWorldPosition(this.RectTransform);
        //Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMax, rect.yMin, 0), Color.green);
        //Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMin, rect.yMax, 0), Color.green);
        //Debug.DrawLine(new Vector3(rect.xMin, rect.yMax, 0), new Vector3(rect.xMax, rect.yMax, 0), Color.green);
        //Debug.DrawLine(new Vector3(rect.xMax, rect.yMin, 0), new Vector3(rect.xMax, rect.yMax, 0), Color.green);
    }

    protected override void WhenHitByHammer()
    {
        if (HitByHammerEffect != null)
        {
            var effect = Instantiate(HitByHammerEffect);
            effect.transform.SetParent(this.transform, false);
        }

        gameManager.ScoreManager.IncreaseScore(Constants.Score.HitExitPostWithHammer);
        gameManager.TickTimers();
    }
}