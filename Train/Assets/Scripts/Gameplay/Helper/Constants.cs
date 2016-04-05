using UnityEngine;

public static class Constants
{
    public static class GameObjects
    {
        public static string Canvas = "Canvas";

        public static string Game = "Game";

        public static string FanfareCanvas = "FanfareCanvas";

        public static string ScreenTransition = "ScreenTransition";

        public static string MapPart = "Map_Part";

        public static string InventoryPart = "Inventory_Part";

        public static string ScorePart = "Score_Part";

        public static string ScoreTextComponent = "Score";

        public static string MapBackgroundColor = "Map_BgColor";

        public static string ItemSelectionBackgroundUI = "SelectItems_BackgroundUI";
    }

    public static class TextComponents
    {
        public static string ScoreCaption = "Score_Caption";

        public static string ItemsCaption = "Items_Caption";

        public static string TurnsCaption = "Turns_Caption";

        public static string Turns = "Turns";
    }

    public static class Dictionary
    {
        public static string Score = "Score";
        public static string Items = "Items";
    }

    public static class Positions
    {
        public static float ItemExtraOffset = 18.0f;
        public static float ItemInitialYOffset = 120.0f;
        public static float InitialFloatYOffset = 20.0f;
        public static float InitialFloatXOffset = 10.0f;
        public static float FloatingDragInputResetCoefficient = 2f;
        public static Vector3 MouseCursorOffset = new Vector2(18f, -12f);
        public static Vector3 CompletionMarkOffset = new Vector3(10, -10);
        public static float UIGoalYOffset = -48f;
    }

    public static class Distances
    {
        public static float MaximumDistanceToUnfloat = 40f;
        public static float MinimumDistanceToDrag = 10f;
        public static float MinimumDesiredDistanceToStop = 0.01f;
        public static float MinimumFloatingDistanceToPushaway = 40f;
        public static float TextSpaceDistance = 110f;
    }

    public static class Intervals
    {
        public static float MinimumTimeToDrag = 0.2f;
        public static float BlinkingInterval = 0.1f;
    }

    public static class Velocity
    {
        public static float FloatingSpeed = 5.0f;
        public static float FloatingPushSpeed = 0.5f;
        public static float FloatingDragSpeed = 1f;
        public static float TrainSpeed = 0.5f;
        public static float FloatingAnimationSpeed = 200f;
        public static float FloatingWaveLength = 2f;
    }

    public static class Sizes
    {
        public static Vector2 CellSize = new Vector2(120f, 120f);
        public static int GridRowCount = 8;
        public static int GridColumnCount = 8;
        public static Vector2 TrackExtraSizeForTouch = new Vector2(15f, 30f);
        public static float NextItemScaleFactor = 0.1225f;
    }

    public static class Input
    {
        public static int LeftMouseButton = 0;
        public static int RightMouseButton = 1;
    }

    public static class Items
    {
        public static string Hand = "Hand";
        public static string Hammer = "Hammer";
        public static string Clock = "Clock";
    }

    public static class Ordering
    {
        public static int FloatingObjectOrder = 15;
    }

    public static class Stats
    {
        public static uint HammerDamageToTrain = 1;
    }

    public static class Paths
    {
        public static string PrefabsPath = "Prefabs/";
        public static string UIPath = "Prefabs/UI/";
        public static string MapGoalsPath = "Prefabs/Map Goals/";
        public static string ItemsPath = "Prefabs/Items/";
        public static string LevelsPath = "Levels/";
    }

    public static class Score
    {
        public static uint HitTrackWithHammer = 100;
        public static uint HitTrainWithHammer = 200;
        public static uint HitExitPostWithHammer = 1;
    }
}