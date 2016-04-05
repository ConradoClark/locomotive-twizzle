namespace Assets.Scripts.Gameplay
{
    public enum GameStates
    {
        Initializing,
        Initialized,
        LoadingMap,
        MapLoaded,
        CreatingLevel,
        LevelCreated,
        LevelOpening,
        LevelFinishedOpening,
        InventorySelection,
        InventorySelectionFinished,
        PlayerTurn,
        WorldTurn,
        WorldHappening,
        VictoryConditionsMet,
        LevelEnding,
        LevelEnded
    }
}