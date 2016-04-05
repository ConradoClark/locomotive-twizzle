using Assets.Scripts.Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Levels : MonoBehaviour
{
    private Dictionary<string, string> definitions;
    public Dictionary<int, GameObject[]> AllLevels;

    private GameObject mainGame;
    private GameManager gameManager;
    private MapManager mapManager;

    bool started;
    public bool HasFinished { get; private set; }

    private void Awake()
    {
        this.definitions = new Dictionary<string, string>();
        this.AllLevels = new Dictionary<int, GameObject[]>();
    }

    private void Start()
    {
        mainGame = GameObject.Find(Constants.GameObjects.Game);
        gameManager = mainGame.GetComponent<GameManager>();
        mapManager = mainGame.GetComponent<MapManager>();
    }

    private void Update()
    {
        if (gameManager.CurrentGameState == GameStates.LoadingMap && !started)
        {
            started = true;
            TextAsset txt = Resources.Load<TextAsset>(Constants.Paths.LevelsPath + mapManager.CurrentLevel);

            StartCoroutine(ReadLevelFromText(txt.text));
        }
    }

    private void Restart()
    {
        started = false;
        definitions.Clear();
    }

    private IEnumerator ReadLevelFromText(string text)
    {
        List<GameObject> cells = new List<GameObject>();
        foreach (string txt in (text ?? "").Split(new char[] { '\n' }))
        {
            switch (txt.FirstOrDefault())
            {
                case 'd':
                    AddDefinition(txt);
                    break;
                case 'g':
                    LoadMapGoal(txt);
                    break;
                case 'n':
                    LoadMapName(txt);
                    break;
                case 'c':
                    LoadMapColor(txt);
                    break;
                case 'i':
                    LoadMapItem(txt);
                    break;
                case 'm':
                    cells.Add(ReadMapTile(txt));
                    break;
                case 'o':
                    cells.Add(ReadObjectTile(txt));
                    break;
            }
        }
        AllLevels.Add(mapManager.CurrentLevel, cells.ToArray());
        this.HasFinished = true;
        yield return true;
    }

    private Vector2 ReadTilePosition(string tileInfo)
    {
        string[] data = tileInfo.Substring(1).Split(new char[] { '|' });

        return new Vector2(
            int.Parse(new string(data[0].TakeWhile(c => c != ',').ToArray())),
            int.Parse(new string(data[0].SkipWhile(c => c != ',').Skip(1).ToArray())));
    }

    private GameObject ReadMapTile(string tileInfo)
    {
        GameObject cell = new GameObject("CellInfo");
        MapObjectCell mapObject = cell.AddComponent<MapObjectCell>();
        string[] data = tileInfo.Substring(1).Split(new char[] { '|' });
        if (data.Length != 2) throw new System.Exception("Invalid map");
        var position = ReadTilePosition(tileInfo);
        mapObject.Row = (int)position.x;
        mapObject.Column = (int)position.y;
        MapBGCell mapCell = cell.AddComponent<MapBGCell>();
        mapCell.Background = definitions[data[1].Trim()];

        return cell;
    }

    private GameObject ReadObjectTile(string tileInfo)
    {
        GameObject cell = new GameObject("CellInfo");
        MapObjectCell mapObject = cell.AddComponent<MapObjectCell>();
        string[] data = tileInfo.Substring(1).Split(new char[] { '|' });
        if (data.Length != 2) throw new System.Exception("Invalid object");
        var position = ReadTilePosition(tileInfo);
        mapObject.Row = (int)position.x;
        mapObject.Column = (int)position.y;
        mapObject.Prefab = definitions[data[1].Trim()];
        return cell;
    }

    private void AddDefinition(string text)
    {
        string[] data = text.Substring(1).Split(new char[] { '|' });
        if (data.Length != 3) throw new System.Exception("Invalid definitions");
        definitions[data[1].Trim()] = data[2].Trim();
    }

    private void LoadMapName(string text)
    {
        string[] data = text.Substring(1).Split(new char[] { '|' });
        mapManager.CurrentLevelName[data[1]] = data[2];
        mapManager.CurrentLevelDescription[data[1]] = data[3];
    }

    private void LoadMapItem(string text)
    {
        string[] data = text.Substring(1).Split(new char[] { '|' });
        if (data.Length != 3) throw new System.Exception("Invalid item definitions");

        GameObject item = Resources.Load<GameObject>(Constants.Paths.ItemsPath + data[1].Trim());
        item.GetComponent<ItemCount>().quantity = uint.Parse(data[2].Trim());

        mapManager.Inventory.Add(GameObject.Instantiate(item));
    }

    private void LoadMapColor(string text)
    {
        string[] data = text.Substring(1).Split(new char[] { '|' });
        if (data.Length != 4) throw new System.Exception("Invalid color definitions");

        Color color = new Color(float.Parse(data[1].Trim())/255f, float.Parse(data[2].Trim()) / 255f, float.Parse(data[3].Trim())/255f);
        mapManager.CurrentLevelBackgroundColor = color;
    }

    private void LoadMapGoal(string text)
    {
        string[] data = text.Substring(1).Split(new char[] { '|' });
        if (data.Length != 2) throw new System.Exception("Invalid goal definitions");
        GameObject goalResource = Resources.Load<GameObject>(Constants.Paths.MapGoalsPath + data[1].Trim());
        GameObject goal = Instantiate(goalResource);

        goal.transform.SetParent(gameManager.ScorePartObject.transform,false);
        goal.transform.localPosition += new Vector3(0, Constants.Positions.UIGoalYOffset * mapManager.CurrentLevelGoals.Count);

        mapManager.CurrentLevelGoals.Add(goal.GetComponent<BaseGoal>());
    }
}