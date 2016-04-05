using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public int CurrentLevel;
    public Dictionary<string,string> CurrentLevelName;
    public Dictionary<string, string> CurrentLevelDescription;
    public GameInventory Inventory;
    public Color CurrentLevelBackgroundColor;
    public List<BaseGoal> CurrentLevelGoals;

    private void Awake()
    {
        this.CurrentLevelName = new Dictionary<string, string>();
        this.CurrentLevelDescription = new Dictionary<string, string>();
        this.CurrentLevelBackgroundColor = Color.white;
        this.CurrentLevelGoals = new List<BaseGoal>();
    }

    private void Start()
    {
        this.Inventory = GetComponent<GameInventory>();
    }

    private void Update()
    {

    }
}