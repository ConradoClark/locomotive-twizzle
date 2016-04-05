using Assets.Scripts.Gameplay;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private GameObject mapLayout;
    public string CurrentLanguage { get; set; }
    public GameStates CurrentGameState { get; private set; }

    private Levels levels;
    private LevelFanfare fanfare;

    private Dictionary<string, Dictionary<string, string>> gameDictionary;
    public GameObject MapLayoutObject { get; private set; }
    public GameObject MapPartObject { get; private set; }
    public GameObject ScorePartObject { get; private set; }
    public GameObject InventoryPartObject { get; private set; }
    public GameObject ItemSelectionObject { get; private set; }
    public GameInventory Inventory { get; private set; }
    public MapGrid MapGrid { get; private set; }
    public ControlsManager Controls { get; private set; }
    public CursorManager Cursor { get; private set; }
    public bool GameHasStarted { get; private set; }
    public ScoreManager ScoreManager { get; private set; }

    private TextComponent turns;
    int numberOfTurns;

    private void Awake()
    {
        mapLayout = Resources.Load<GameObject>(Constants.Paths.PrefabsPath + "Map");
        this.SetGameState(GameStates.Initializing);
        this.CurrentLanguage = "pt-br";
        this.MapLayoutObject = UnityEngine.Object.Instantiate(mapLayout);
        this.gameDictionary = new Dictionary<string, Dictionary<string, string>>();
    }

    private void Start()
    {
        this.Controls = this.GetComponent<ControlsManager>();
        this.Cursor = this.GetComponent<CursorManager>();
        this.ScoreManager = this.GetComponent<ScoreManager>();

        LoadDictionary();

        levels = this.GetComponent<Levels>();
        Inventory = this.GetComponent<GameInventory>();

        GameObject canvas = GameObject.Find(Constants.GameObjects.Canvas);
        MapGrid = this.MapLayoutObject.GetComponent<MapGrid>();
        this.MapLayoutObject.transform.SetParent(canvas.transform, false);

        this.MapPartObject = this.MapLayoutObject.GetComponentsInChildren<Transform>().First(c => c.name == Constants.GameObjects.MapPart).gameObject;
        this.ScorePartObject = this.MapLayoutObject.GetComponentsInChildren<Transform>().First(c => c.name == Constants.GameObjects.ScorePart).gameObject;
        this.InventoryPartObject = this.MapLayoutObject.GetComponentsInChildren<Transform>().First(c => c.name == Constants.GameObjects.InventoryPart).gameObject;
        this.ItemSelectionObject = canvas.GetComponentsInChildren<RectTransform>().First(c => c.name == Constants.GameObjects.ItemSelectionBackgroundUI).gameObject;
        

        ScaleScreen(canvas);

        fanfare = this.MapLayoutObject.GetComponentInChildren<LevelFanfare>();

        var textComponents = this.MapLayoutObject.transform.GetComponentsInChildren<TextComponent>();
        turns = textComponents.FirstOrDefault(c => c.name == Constants.TextComponents.Turns);

        this.SetGameState(GameStates.Initialized);
    }

    public string GetTranslation(string key)
    {
        return this.gameDictionary[this.CurrentLanguage][key];
    }

    private void ScaleScreen(GameObject canvas)
    {
        var factor = (1920f / 1080f) - ((float)Screen.width / Screen.height);
        var scaling = Mathf.Max(0, factor > 0 ? factor / 4 : 0);
        ScorePartObject.transform.localPosition += new Vector3(factor * 175, 0, 0);
        InventoryPartObject.transform.localPosition -= new Vector3(factor * 175, 0, 0);
        canvas.transform.localScale -= new Vector3(scaling, scaling, 0);

        RectTransform inventoryPartTransform = (RectTransform)InventoryPartObject.transform;

        float sizeX = (inventoryPartTransform.anchoredPosition.x + inventoryPartTransform.rect.size.x / 2+6) * 2;
        float sizeY = (inventoryPartTransform.anchoredPosition.y + inventoryPartTransform.rect.size.y / 2) * 2;

        var itemSelectionSpriteRectTransform = ItemSelectionObject.GetComponentInChildren<SpriteRenderer>().GetComponent<RectTransform>();
        var itemSelectionRectTransform = ItemSelectionObject.GetComponent<RectTransform>();

        itemSelectionRectTransform.sizeDelta = new Vector2(sizeX, sizeY);

        itemSelectionSpriteRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,sizeX);
        itemSelectionSpriteRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeY);
    }

    private void SetGameState(GameStates state)
    {
        this.CurrentGameState = state;
    }

    private void Update()
    {
        switch (this.CurrentGameState)
        {
            case GameStates.Initialized:
                this.SetGameState(GameStates.LoadingMap);
                break;
            case GameStates.LoadingMap:
                if (levels.HasFinished && Inventory.HasFinished)
                {
                    this.SetGameState(GameStates.MapLoaded);
                }
                break;
            case GameStates.MapLoaded:
                this.SetGameState(GameStates.CreatingLevel);
                break;
            case GameStates.CreatingLevel:
                if (MapGrid.HasFinished)
                {
                    this.SetGameState(GameStates.LevelCreated);
                }
                break;
            case GameStates.LevelCreated:
                this.SetGameState(GameStates.LevelOpening);
                break;
            case GameStates.LevelOpening:
                if (fanfare.HasFinished)
                {
                    this.SetGameState(GameStates.LevelFinishedOpening);
                }
                break;
            case GameStates.LevelFinishedOpening:
                this.SetGameState(GameStates.InventorySelection);
                break;
            case GameStates.InventorySelection:

                break;
            case GameStates.InventorySelectionFinished:
                this.SetGameState(GameStates.PlayerTurn);
                this.Inventory.ActivateFirstItem();
                StartPlayerTurn();
                this.GameHasStarted = true;
                break;
            case GameStates.PlayerTurn:
                var mapTurns = this.MapLayoutObject.GetComponentsInChildren<MapTurns>();
                if (mapTurns.Any(b => b.currentTurn == 0))
                {
                    this.SetGameState(GameStates.WorldTurn);
                }
                break;
            case GameStates.WorldTurn:
                this.Inventory.SetAllToInactive();
                var mapActors = this.MapLayoutObject.GetComponentsInChildren<MapActor>();
                if (mapActors.Where(actor => actor.Turns != null).All(actor => actor.HasFinished))
                {
                    StartCoroutine(ResetAndStartWorldTurn(mapActors));
                }
                break;
            case GameStates.WorldHappening:
                break;
        }
    }

    public void TickTimers()
    {
        numberOfTurns++;
        this.turns.Text = numberOfTurns.ToString();
        var mapActors = this.MapLayoutObject.GetComponentsInChildren<MapTurns>();
        mapActors.ToList().ForEach(c => c.Tick());
    }

    private IEnumerator ResetAndStartWorldTurn(MapActor[] actors)
    {
        this.SetGameState(GameStates.WorldHappening);
        yield return new WaitForSeconds(0.3f);
        actors.ToList().ForEach(b => b.FinishTurn());
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        this.SetGameState(GameStates.PlayerTurn);
        this.Inventory.SetAllToNormal(true);
    }

    private void LoadDictionary()
    {
        this.gameDictionary[this.CurrentLanguage] = new Dictionary<string, string>();

        TextAsset txt = Resources.Load<TextAsset>("Dictionary/" + this.CurrentLanguage);
        foreach (string line in (txt.text ?? "").Split(new char[] { '\n' }))
        {
            string[] data = line.Split(new char[] { '|' });
            if (data.Length != 2) continue;
            this.gameDictionary[this.CurrentLanguage][data[0].Trim()] = data[1].Trim();
        }
    }

    public static GameObject GetMainGame()
    {
        return GameObject.Find(Constants.GameObjects.Game);
    }

    public static GameManager GetGameManager()
    {
        var game = GetMainGame();
        return game == null ? null : game.GetComponent<GameManager>();
    }
}