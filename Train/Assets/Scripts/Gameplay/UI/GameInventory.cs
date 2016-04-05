using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Gameplay;
using System.Linq;

public class GameInventory : MonoBehaviour
{
    Queue<GameObject> itemsToLoad;
    List<ItemState> items;

    bool started;
    public bool HasFinished { get; private set; }

    Levels levels;
    RectTransform inventoryPart_Transform;
    GameManager gameManager;

    string lastActivatedItem;

    float itemOffset = 0;

    void Awake()
    {
        this.itemsToLoad = new Queue<GameObject>();
        this.items = new List<ItemState>();
    }

    void Start()
    {
        var mainGame = GameManager.GetMainGame();
        this.gameManager = mainGame.GetComponent<GameManager>();
        levels = mainGame.GetComponent<Levels>();
        inventoryPart_Transform = (RectTransform)gameManager.MapLayoutObject.GetComponentsInChildren<Transform>().First(c => c.name == Constants.GameObjects.InventoryPart).gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.CurrentGameState == GameStates.LoadingMap && !started)
        {
            started = true;
            StartCoroutine(AddItems());
        }
    }

    public IEnumerator AddItems()
    {
        while (itemsToLoad.Count > 0 || !levels.HasFinished)
        {
            if (itemsToLoad.Count > 0)
            {
                GameObject itemToAdd = itemsToLoad.Dequeue();

                RectTransform rect = (RectTransform)itemToAdd.transform;
                ItemRender render = itemToAdd.GetComponent<ItemRender>();
                if (render != null && render.enabled)
                {
                    var anchoredPosition = rect.anchoredPosition;
                    rect.SetParent(inventoryPart_Transform, false);
                    var newY = anchoredPosition.y - itemOffset * (render.Rect.size.y + Constants.Positions.ItemExtraOffset) - Constants.Positions.ItemInitialYOffset;
                    rect.anchoredPosition = new Vector2(anchoredPosition.x, newY);
                    itemOffset++;
                }
                else
                {
                    rect.SetParent(inventoryPart_Transform, false);
                }
                items.Add(itemToAdd.GetComponent<ItemState>());
            }
            yield return 0;
        }
        this.HasFinished = true;
        yield return true;
    }

    public void SetAllToInactive()
    {
        foreach (var item in this.items)
        {
            ItemState state= item.GetComponent<ItemState>();
            state.CurrentState = ItemState.State.Inactive;
        }
    }

    public void SetAllToNormal(bool useLastActivated = false)
    {
        foreach (var item in this.items)
        {
            ItemState state = item.GetComponent<ItemState>();
            state.CurrentState = ItemState.State.Normal;
        }

        if (useLastActivated)
        {
            this.ActivateItem(lastActivatedItem);
        }
    }

    public void Clear()
    {
        this.itemsToLoad.Clear();
    }

    public void Add(GameObject obj)
    {
        if (obj != null)
        {
            this.itemsToLoad.Enqueue(obj);
        }
    }

    public ItemRender[] GetItems()
    {
        return this.items.Select(i => i.GetComponent<ItemRender>()).ToArray();
    }

    public void ActivateItem(string name)
    {
        this.items.Where(i => i.ReferenceName == name).ToList().ForEach(ActivateItem);
    }

    public void ActivateItem(ItemState item)
    {
        this.lastActivatedItem = item.ReferenceName;
        SetAllToNormal();
        item.CurrentState = ItemState.State.Active;
    }

    public bool IsItemActive(string name)
    {
        return this.items.Where(i => i.ReferenceName == name && i.CurrentState == ItemState.State.Active).Any();
    }

    public void ActivateFirstItem()
    {
        var item = this.items.FirstOrDefault(i=>i.ReferenceName != Constants.Items.Hand);
        if (item == null) return;
        ActivateItem(item);
    }

    public ItemState GetLastActivatedItem()
    {
        return this.items.FirstOrDefault(item => item.ReferenceName == this.lastActivatedItem);
    }
}