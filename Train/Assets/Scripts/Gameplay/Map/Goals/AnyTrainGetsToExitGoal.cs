using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AnyTrainGetsToExitGoal : BaseGoal
{
    private GameManager gameManager;

    private Dictionary<MapActor, Train> trains;
    private Dictionary<MapActor, ExitPost> goalPosts;

    void Start()
    {
        this.gameManager = GameManager.GetGameManager();        
    }

    void Update()
    {
        if (!gameManager.GameHasStarted) return;

        if (this.trains == null || this.goalPosts == null)
        {
            this.trains = gameManager.MapGrid.GetActorsByComponent<Train>();
            this.goalPosts = gameManager.MapGrid.GetActorsByComponent<ExitPost>();
        }

        if (!GoalIsMet && trains.Any(train=>train.Key.Cell.AnyIsInSamePlace(goalPosts.Select(gp=>gp.Key.Cell).ToArray())))
        {
            this.GoalIsMet = true;
        }
    }
}
