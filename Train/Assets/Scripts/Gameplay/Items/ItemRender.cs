using UnityEngine;
using System.Collections;
using System.Linq;

public class ItemRender : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite SpriteNormal, SpriteHover, SpriteActive, SpriteInactive;
    private GameInventory inventory;

    public string Name;
    public Rect Rect { get; private set; }

    void Awake()
    {
        this.inventory = GameManager.GetMainGame().GetComponent<GameInventory>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.spriteRenderer.sprite = SpriteNormal;
        this.Rect = this.spriteRenderer.sprite.rect;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetToHover()
    {
        this.spriteRenderer.sprite = SpriteHover;
    }

    public void SetToNormal()
    {
        this.spriteRenderer.sprite = SpriteNormal;
    }

    public void SetToInactive()
    {
        this.spriteRenderer.sprite = SpriteInactive;
    }

    public void SetToActive()
    {
        this.spriteRenderer.sprite = SpriteActive;
    }
}
