using Assets.Scripts.Gameplay.Helper;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TiledSprite : MonoBehaviour
{
    public Sprite sprite;
    public Color color;
    public Material material;
    public int orderInLayer;

    private RectTransform rectTransform;
    private Vector2 spriteSize;
    private Vector2 positionDelta;
    private int columns, rows;

    private List<SpriteRenderer> allSpriteRenderers;
    private List<SpriteRenderer> spriteRenderersInner;
    private List<SpriteRenderer> spriteRenderersHorizontal;
    private List<SpriteRenderer> spriteRenderersVertical;
    public Vector2 scrollSpeed;

    private void Start()
    {
        this.rectTransform = this.GetComponent<RectTransform>();
        this.spriteRenderersInner = new List<SpriteRenderer>();
        this.spriteRenderersHorizontal = new List<SpriteRenderer>();
        this.spriteRenderersVertical = new List<SpriteRenderer>();
        this.allSpriteRenderers = new List<SpriteRenderer>();

        if (this.sprite != null)
        {
            this.spriteSize = this.sprite.bounds.size;
        }

        columns = Mathf.CeilToInt(rectTransform.rect.size.x / this.spriteSize.x) + 3;
        rows = Mathf.CeilToInt(rectTransform.rect.size.y / this.spriteSize.y) + 3;

        for (int c = 0; c < columns; c++)
        {
            for (int r = 0; r < rows; r++)
            {
                GameObject renderer = new GameObject(string.Format("Renderer({0},{1})", r, c));
                renderer.transform.SetParent(this.transform, false);
                renderer.transform.localPosition = GetTilePosition(c, r);
                var spriteRenderer = renderer.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = this.sprite;
                spriteRenderer.color = this.color;
                spriteRenderer.material = new Material(this.material);
                spriteRenderer.sortingOrder = this.orderInLayer;
                this.allSpriteRenderers.Add(spriteRenderer);
            }
        }
    }

    private Vector3 GetTilePosition(int column, int row)
    {
        var xpos = column * this.sprite.bounds.size.x - columns * this.sprite.bounds.size.x / 2 + this.sprite.bounds.size.x / 2;
        return new Vector3(xpos, row * this.sprite.bounds.size.y - rows * this.sprite.bounds.size.y / 2 + this.sprite.bounds.size.y / 2);
    }

    private void LateUpdate()
    {
        Vector2 speed = new Vector2(scrollSpeed.x * Time.deltaTime * 60f, scrollSpeed.y * Time.deltaTime * 60f);

        foreach (var sprRenderer in allSpriteRenderers)
        {
            sprRenderer.transform.localPosition += new Vector3(speed.x, speed.y);
        }

        positionDelta += VectorHelper.Absolute(speed);

        if (Mathf.Abs(scrollSpeed.x) > 0 && positionDelta.x >= (int)(sprite.bounds.size.x))
        {
            positionDelta.x = 0;
            for (int c = 0; c < columns; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    var sprRenderer = allSpriteRenderers[c * rows + r];
                    sprRenderer.transform.localPosition = new Vector3(GetTilePosition(c, r).x, sprRenderer.transform.localPosition.y);
                }
            }
        }

        if (Mathf.Abs(scrollSpeed.y) > 0 && positionDelta.y >= (int)(sprite.bounds.size.y))
        {
            positionDelta.y = 0;
            for (int c = 0; c < columns; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    var sprRenderer = allSpriteRenderers[c * rows + r];
                    sprRenderer.transform.localPosition = new Vector3(sprRenderer.transform.localPosition.x, GetTilePosition(c, r).y);
                }
            }
        }
    }
}