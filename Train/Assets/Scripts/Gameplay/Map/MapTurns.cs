using UnityEngine;
using System.Collections;

public class MapTurns : MonoBehaviour
{
    public int turns;
    public int currentTurn;
    public GameObject objectIconPrefab;
    private GameObject objectIcon;
    GameObject turnCloudPrefab;
    GameObject turnCloud;
    TextComponent turnCloudText;

    public bool CanTick { get; set; }

    void Awake()
    {
        this.currentTurn = turns;
        this.turnCloudPrefab = Resources.Load<GameObject>(Constants.Paths.UIPath + "TurnCloud");
        this.turnCloud = UnityEngine.Object.Instantiate(turnCloudPrefab);
        this.turnCloud.name = "TurnCloud";

        this.turnCloud.transform.SetParent(this.transform, false);
        this.turnCloud.transform.localScale = new Vector3(0.8f, 0.8f);

        turnCloudText = this.turnCloud.GetComponent<TextComponent>();
        turnCloudText.Text = currentTurn.ToString();
    }

    void Start()
    {
        this.CanTick = true;
    }

    void Update()
    {
        turnCloudText.Text = currentTurn.ToString();
    }

    public void Restart()
    {
        this.currentTurn = this.turns;
    }

    public void Tick()
    {
        if (this.CanTick)
        {
            currentTurn--;
        }
    }

    public void CreateObjectIcon(Transform parent)
    {
        if (objectIcon != null || objectIconPrefab == null) return;
        objectIcon = Instantiate(objectIconPrefab);
        objectIcon.transform.SetParent(parent,false);
    }

    public void MoveObjectIcon(Vector2 position)
    {
        if (objectIcon == null || objectIconPrefab == null) return;
        objectIcon.transform.localPosition = new Vector3(position.x, position.y);
    }

    public void ScaleObjectIcon(float scale)
    {
        if (objectIcon == null || objectIconPrefab == null) return;
        objectIcon.transform.localScale = new Vector3(scale, scale);
    }

    public bool TryGetPosition(out Vector2 position)
    {
        position = Vector2.zero;
        if (objectIcon == null || objectIconPrefab == null) return false;

        position = objectIcon.transform.localPosition;
        return true;
    }

    public bool TryGetScale(out float scale)
    {
        scale = 1f;
        if (objectIcon == null || objectIconPrefab == null) return false;

        scale = objectIcon.transform.localScale.x;
        return true;
    }

    public float GetInitialScale()
    {
        if (objectIcon == null || objectIconPrefab == null) return 1.0f;

        return objectIconPrefab.transform.localScale.x;
    }

    public void DestroyIcon()
    {
        if (objectIcon == null || objectIconPrefab == null) return;

        DestroyObject(objectIcon);
        //objectIcon = null;
    }
}