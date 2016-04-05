using Assets.Scripts.Gameplay;
using Assets.Scripts.Gameplay.Control;
using Assets.Scripts.Gameplay.Helper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    public int Rows;
    public int Columns;

    public int XOffset;
    public int YOffset;

    private GameObject evenCell;
    private GameObject oddCell;

    private GameObject mainGame;
    private GameManager gameManager;
    private MapManager mapManager;
    private Levels levels;

    private GameObject mapPartBackgroundColorObject;

    private Dictionary<string, GameObject> loadedPrefabs;

    private List<RectTransform> bgCells;
    private List<MapActor> actorCells;

    private bool started = false;
    public bool HasFinished { get; private set; }
    public RectTransform MapPartTransform { get; private set; }

    private PositionActionState[] inputStatesOnMap;
    public PositionActionState[] InputStatesOnMap
    {
        get
        {
            if (MapPartTransform == null) return new PositionActionState[0];
            if (inputStatesOnMap == null)
            {
                inputStatesOnMap = gameManager.Controls.GetInputStatesOnPosition(MapPartTransform.gameObject);
            }
            return inputStatesOnMap;
        }
        set
        {
            this.inputStatesOnMap = value;
        }
    }

    private void Awake()
    {
        evenCell = Resources.Load<GameObject>(Constants.Paths.PrefabsPath + "MapTile-Even");
        oddCell = Resources.Load<GameObject>(Constants.Paths.PrefabsPath + "MapTile-Odd");
        loadedPrefabs = new Dictionary<string, GameObject>();
        this.bgCells = new List<RectTransform>();
        this.actorCells = new List<MapActor>();
    }

    private void Start()
    {
        mainGame = GameObject.Find(Constants.GameObjects.Game);
        gameManager = mainGame.GetComponent<GameManager>();
        mapManager = mainGame.GetComponent<MapManager>();
        levels = mainGame.GetComponent<Levels>();
        this.MapPartTransform = (RectTransform)gameManager.MapPartObject.transform;
    }

    private void Update()
    {
        this.InputStatesOnMap = null;
        if (gameManager.CurrentGameState == GameStates.CreatingLevel && !started)
        {
            started = true;
            MapPartTransform = (RectTransform)gameManager.MapPartObject.transform;
            mapPartBackgroundColorObject = MapPartTransform.gameObject.GetComponentsInChildren<Transform>().First(c => c.name == Constants.GameObjects.MapBackgroundColor).gameObject;

            var currentLevel = levels.AllLevels[mapManager.CurrentLevel];

            StartCoroutine(CreateMapCells(currentLevel));
        }
    }

    public void Restart()
    {
        started = false;
    }

    private IEnumerator CreateMapCells(GameObject[] map)
    {
        mapPartBackgroundColorObject.GetComponent<SpriteRenderer>().color = mapManager.CurrentLevelBackgroundColor;
        foreach (GameObject cell in map)
        {
            var mapObject = cell.GetComponent<MapObjectCell>();
            var cellObject = cell.GetComponent<MapBGCell>();
            GameObject createdCell = null;

            if (mapObject.Prefab != null && !loadedPrefabs.ContainsKey(mapObject.Prefab))
            {
                loadedPrefabs[mapObject.Prefab] = Resources.Load<GameObject>(Constants.Paths.PrefabsPath + mapObject.Prefab);
            }

            if (cellObject == null)
            {
                createdCell = UnityEngine.Object.Instantiate(loadedPrefabs[mapObject.Prefab]);
            }
            else
            {
                createdCell = (mapObject.Row + mapObject.Column) % 2 == 0 ? UnityEngine.Object.Instantiate(evenCell) : UnityEngine.Object.Instantiate(oddCell);
            }

            cell.transform.SetParent(createdCell.transform, false);
            RectTransform rect = (RectTransform)createdCell.transform;
            Vector2 originalLocalPosition = rect.transform.localPosition;
            rect.SetParent(MapPartTransform, false);

            rect.anchoredPosition = new Vector3((Constants.Sizes.CellSize.x / 2) + mapObject.Column * Constants.Sizes.CellSize.x + originalLocalPosition.x,
                (Constants.Sizes.CellSize.y / 2) - (mapObject.Row + 1) * (Constants.Sizes.CellSize.y) + originalLocalPosition.y, 0);
            createdCell.name = string.Format("{0} ({1},{2})", mapObject.Prefab ?? "Cell", mapObject.Row, mapObject.Column);

            MapActor actor;
            if (cellObject != null)
            {
                this.bgCells.Add(rect);
            }
            else if ((actor = createdCell.GetComponent<MapActor>()) != null)
            {
                actor.Offset = new Vector2(originalLocalPosition.x, originalLocalPosition.y);
                this.actorCells.Add(actor);
            }
        }
        this.HasFinished = true;
        yield return true;
    }

    public bool RelocateInGrid(Rect rect, out int row, out int column)
    {
        //Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMax, rect.yMin, 0), Color.green, 10f);
        //Debug.DrawLine(new Vector3(rect.xMin, rect.yMin, 0), new Vector3(rect.xMin, rect.yMax, 0), Color.green, 10f);
        //Debug.DrawLine(new Vector3(rect.xMin, rect.yMax, 0), new Vector3(rect.xMax, rect.yMax, 0), Color.green, 10f);
        //Debug.DrawLine(new Vector3(rect.xMax, rect.yMin, 0), new Vector3(rect.xMax, rect.yMax, 0), Color.green, 10f);

        var chosenCell = this.bgCells.Where(c => rect.Overlaps(RectTransformHelper.GetRectInWorldPosition(c)))
                                   .Where(c => Vector2.Distance(RectTransformHelper.GetRectInWorldPosition(c).center, rect.center) < Constants.Distances.MaximumDistanceToUnfloat)
                                   .OrderBy(c =>
                                   {
                                       //Debug.DrawLine(RectTransformHelper.GetRectInWorldPosition(c).center, rect.center, Color.red, 10f);
                                       return Vector2.Distance(RectTransformHelper.GetRectInWorldPosition(c).center, rect.center);
                                   })
                                   .FirstOrDefault();

        if (chosenCell == null)
        {
            row = 0;
            column = 0;
            return false;
        }

        var objectCell = chosenCell.GetComponentInChildren<MapObjectCell>();
        row = objectCell.Row;
        column = objectCell.Column;
        return true;
    }

    public MapActor[] GetActorsByGridPosition(int row, int column)
    {
        return this.actorCells.Where(a => !a.IsFloating && a.Cell.Row == row && a.Cell.Column == column).ToArray();
    }

    public Dictionary<MapActor, T> GetActorsByComponent<T>()
    {
        return this.actorCells.Select(actor => new KeyValuePair<MapActor, T>(actor, actor.GetComponent<T>())).Where(kvp => kvp.Value != null).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public MapFloater[] GetOverlappingFloatingObjects(MapFloater floatingObject)
    {
        return this.actorCells
            .Select(actor => actor.GetComponent<MapFloater>())
            .Where(floater => floater != null)
            .Where(floater => floater.IsFloating && RectTransformHelper.GetRectInWorldPosition(floatingObject.RectTransform)
                                                                       .Overlaps(RectTransformHelper.GetRectInWorldPosition(floater.RectTransform))).ToArray();
    }

    public bool IsAnyFloatingObjectBeingDragged()
    {
        return this.actorCells
            .Select(actor => actor.GetComponent<MapFloater>())
            .Where(floater => floater != null && floater.IsFloating)
            .Any(floater => floater.IsInDragMode);
    }
}