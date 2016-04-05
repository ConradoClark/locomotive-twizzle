using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Gameplay.Helper;

[RequireComponent(typeof(RectTransform))]
public class TiledSprite : MonoBehaviour
{
    public Sprite sprite;
    public Color color;
    public Material material;
    public int orderInLayer;

    private RectTransform rectTransform;
    private Vector2 spriteSize;

    private List<SpriteRenderer> allSpriteRenderers;
    private List<SpriteRenderer> spriteRenderersInner;
    private List<SpriteRenderer> spriteRenderersHorizontal;
    private List<SpriteRenderer> spriteRenderersVertical;

    public Vector2 scrollSpeed;
    int columns, rows;

    void Start()
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
                // spriteRenderer.material.mainTextureOffset = GetTileTextureOffset(c, r);

                //var normalizedSpeedX = (int)scrollSpeed.normalized.x;
                //int column = c, row = r;

                //if (c == columns - 2)
                //{
                //    var offset = VectorHelper.Absolute(VectorHelper.XPart(spriteRenderer.material.mainTextureOffset));
                //    var offsetMinusOne = VectorHelper.XPart(VectorHelper.VectorMinusOne(VectorHelper.Absolute(offset)));

                //    onScroll += () => this.StartCoroutine(WrapAround(spriteRenderer, column, row, offsetMinusOne * this.sprite.bounds.size.x * normalizedSpeedX, offset, false));
                //    //Debug.Log(spriteRenderer.name + " - Moving: " + offsetMinusOne.x * this.sprite.bounds.size.x * normalizedSpeedX + " | Offset: " + offset.x);
                //}
                //else if (c == 1)
                //{
                //    var offset = VectorHelper.XPart(spriteRenderer.material.mainTextureOffset);
                //    var moving = VectorHelper.XPart(VectorHelper.OneMinusVector(offset)) * this.sprite.bounds.size.x * normalizedSpeedX;
                //    onScroll += () => this.StartCoroutine(WrapAround(spriteRenderer, column, row, moving, offset, false));
                //    //Debug.Log(spriteRenderer.name + " - Moving: " + moving.x + " | Offset: " + offset.x);
                //}
                //else if (c == 0)
                //{
                //    var offset = VectorHelper.XPart(GetTileTextureOffset(c + 1, r));
                //    var moving = (VectorHelper.Absolute(offset) * this.sprite.bounds.size.x * normalizedSpeedX);
                //    onScroll += () => this.StartCoroutine(WrapAround(spriteRenderer, column, row, moving, VectorHelper.XPart(VectorHelper.OneMinusVector(offset))));
                //    //Debug.Log(spriteRenderer.name + " - Moving: " + moving.x + " | Offset: " + VectorHelper.XPart(VectorHelper.OneMinusVector(offset)).x);
                //}
                //else if (c == columns - 1)
                //{
                //    var offset = VectorHelper.Absolute(VectorHelper.XPart(GetTileTextureOffset(c - 1, r)));
                //    var offsetMinusOne = VectorHelper.XPart(VectorHelper.VectorMinusOne(VectorHelper.Absolute(offset)));
                //    onScroll += () => this.StartCoroutine(WrapAround(spriteRenderer, column, row, offset * this.sprite.bounds.size.x, -offsetMinusOne));
                //    //Debug.Log(spriteRenderer.name + " - Moving: " + offset.x * this.sprite.bounds.size.x + " | Offset: " + -offsetMinusOne.x);
                //}
                //else if (c > 1 && c < columns - 2)
                //{
                //    var offset = VectorHelper.XPart(GetTileTextureOffset(c + normalizedSpeedX, r));
                //    var moving = VectorHelper.Absolute(new Vector2(this.sprite.bounds.size.x, 0) + (offset * this.sprite.bounds.size.x * normalizedSpeedX));
                //    onScroll += () => this.StartCoroutine(WrapAround(spriteRenderer, column, row, moving, -offset * normalizedSpeedX));
                //    //Debug.Log(spriteRenderer.name + " - Moving: " + moving.x + " | Offset: " + -offset.x * normalizedSpeedX);
                //}
                this.allSpriteRenderers.Add(spriteRenderer);
                //this.remainingScrollFunctions++;
            }
        }
        //onScroll();
    }

    private Vector3 GetTilePosition(int column, int row)
    {
        var xOffset = 0f;//GetTileTextureOffset(column, row).x * this.sprite.bounds.size.x;
        var yOffset = 0f;//GetTileTextureOffset(column, row).y * this.sprite.bounds.size.y;

        var xpos = column * this.sprite.bounds.size.x - columns * this.sprite.bounds.size.x / 2 + this.sprite.bounds.size.x / 2 + xOffset;
        return new Vector3(xpos, row * this.sprite.bounds.size.y - rows * this.sprite.bounds.size.y / 2 + this.sprite.bounds.size.y / 2 + yOffset);
    }

    //private Vector2 GetTileTextureOffset(int column, int row)
    //{
    //    var panelFullWidth = (columns - 2) * this.sprite.bounds.size.x;
    //    var visibleWidth = this.rectTransform.rect.width;

    //    var pixelsLeft_x = (panelFullWidth - visibleWidth) / 2;
    //    var remaining_x = (this.sprite.bounds.size.x - pixelsLeft_x) * (1f / this.sprite.bounds.size.x);

    //    var nextRemaining_x = 1f - remaining_x;
    //    var columnOffset = (column == 0) ? 1f : (column >= columns - 1 ? -1f : (column <= 1 ? nextRemaining_x : (column == columns - 2 ? -nextRemaining_x : 0)));

    //    var panelFullHeight = (rows - 2) * this.sprite.bounds.size.y;
    //    var visibleHeight = this.rectTransform.rect.height;

    //    var pixelsLeft_y = (panelFullHeight - visibleHeight) / 2;
    //    var remaining_y = (this.sprite.bounds.size.y - pixelsLeft_y) * (1 / this.sprite.bounds.size.y);

    //    var nextRemaining_y = 1 - remaining_y;
    //    var rowOffset = (row == 0) ? 1f : (row >= rows - 1 ? -1f : (row <= 1 ? nextRemaining_y : (row == rows - 2 ? -nextRemaining_y : 0)));

    //    return new Vector2(columnOffset, rowOffset);
    //}

    //private Coroutine StartWrapAround(SpriteRenderer spriteRenderer, int column, int row, Vector2 amountToMove, Vector2 amountToOffset, bool moveFirst)
    //{
    //    return this.StartCoroutine(WrapAround(spriteRenderer, column, row, amountToMove, amountToOffset, moveFirst));
    //}

    //public IEnumerator WrapAround(SpriteRenderer spriteRenderer, int column, int row, Vector2 amountToMove, Vector2 amountToOffset, bool moveFirst = true)
    //{
    //    //if (spriteRenderer.name.Contains("(3,2") || spriteRenderer.name.Contains("(3,3"))
    //    //{
    //    //    Debug.Log(spriteRenderer.name + " AmountToMove: " + amountToMove.x + " | AmountToOffset: " + amountToOffset.x);
    //    //}
    //    spriteRenderer.transform.localPosition = GetTilePosition(column, row);
    //    spriteRenderer.material.mainTextureOffset = GetTileTextureOffset(column, row);

    //    //while (this.enabled)
    //    //{            
    //    if (moveFirst)
    //    {
    //        yield return Move(spriteRenderer, amountToMove);
    //        yield return Offset(spriteRenderer, amountToOffset);
    //    }
    //    else
    //    {
    //        yield return Offset(spriteRenderer, amountToOffset);
    //        yield return Move(spriteRenderer, amountToMove);
    //    }

    //    //if (spriteRenderer.name.Contains("(3,2") || spriteRenderer.name.Contains("(3,3"))
    //    //{
    //    //    Debug.Log(spriteRenderer.name + " RESET (FRAME " + Time.frameCount + ") - " + Time.time);
    //    //}

    //    //}

    //    remainingScrollFunctions--;
    //    yield break;
    //}

    //public IEnumerator Move(SpriteRenderer spriteRenderer, Vector2 amountToMove)
    //{
    //    Vector2 moved = Vector2.zero;
    //    Vector2 max = amountToMove + new Vector2(Mathf.Abs(spriteRenderer.transform.localPosition.x), Mathf.Abs(spriteRenderer.transform.localPosition.y));

    //    // while (Mathf.Abs(Mathf.Abs(moved.magnitude) - Mathf.Abs(amountToMove.magnitude)) > 1f )
    //    while (moved.magnitude < amountToMove.magnitude)
    //    {
    //        spriteRenderer.transform.localPosition = new Vector3(Mathf.Clamp(spriteRenderer.transform.localPosition.x + this.scrollSpeed.x * Time.deltaTime * 60, -Mathf.Abs(max.x), Mathf.Abs(max.x)),
    //            Mathf.Clamp(spriteRenderer.transform.localPosition.y + this.scrollSpeed.y, -Mathf.Abs(max.y), Mathf.Abs(max.y)));

    //        moved = new Vector2(Mathf.Clamp(moved.x + scrollSpeed.x * Time.deltaTime * 60, -Mathf.Abs(amountToMove.x), Mathf.Abs(amountToMove.x)), Mathf.Clamp(moved.y + scrollSpeed.y, -Mathf.Abs(amountToMove.y), Mathf.Abs(amountToMove.y)));
    //        yield return moved;
    //    }
    //}

    //public IEnumerator Offset(SpriteRenderer spriteRenderer, Vector2 amountToOffset)
    //{
    //    int totalFrames = (int)(this.sprite.bounds.size.x / this.scrollSpeed.x);

    //    var spriteFactor = new Vector2((-1f / this.sprite.bounds.size.x) * this.scrollSpeed.x, (-1f / this.sprite.bounds.size.y) * this.scrollSpeed.y);
    //    Vector2 offset = Vector2.zero;
    //    Vector2 max = amountToOffset + new Vector2(Mathf.Abs(spriteRenderer.material.mainTextureOffset.x), Mathf.Abs(spriteRenderer.material.mainTextureOffset.y));

    //    while (offset.magnitude < amountToOffset.magnitude)
    //    {
    //        totalFrames--;
    //        spriteRenderer.material.mainTextureOffset = new Vector2(Mathf.Clamp(spriteRenderer.material.mainTextureOffset.x + spriteFactor.x * Time.deltaTime * 60, -max.x, max.x),
    //            Mathf.Clamp(spriteRenderer.material.mainTextureOffset.y + spriteFactor.y, -Mathf.Abs(max.y), Mathf.Abs(max.y)));

    //        offset = new Vector2(Mathf.Clamp(offset.x + spriteFactor.x * Time.deltaTime * 60, -Mathf.Abs(amountToOffset.x), Mathf.Abs(amountToOffset.x)), Mathf.Clamp(offset.y + spriteFactor.y, -Mathf.Abs(amountToOffset.y), Mathf.Abs(amountToOffset.y)));
    //        yield return offset;
    //    }
    //}

    //private bool IsCorner(int column, int row)
    //{
    //    return column == 0 || column == columns - 1 || row == 0 || row == rows - 1;
    //}

    float ix = 0f, iy = 0f;
    void LateUpdate()
    {
        Vector2 speed = new Vector2(scrollSpeed.x * Time.deltaTime * 60f, scrollSpeed.y * Time.deltaTime * 60f);

        foreach (var sprRenderer in allSpriteRenderers)
        {
            sprRenderer.transform.localPosition += new Vector3(speed.x, speed.y);
        }
        ix += Mathf.Abs(speed.x);
        iy += Mathf.Abs(speed.y);

        if (Mathf.Abs(scrollSpeed.x) > 0 && ix >= (int)(sprite.bounds.size.x))
        {
            ix = 0;
            for (int c = 0; c < columns; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    var sprRenderer = allSpriteRenderers[c * rows + r];
                    sprRenderer.transform.localPosition = new Vector3(GetTilePosition(c, r).x, sprRenderer.transform.localPosition.y);
                }
            }
        }

        if (Mathf.Abs(scrollSpeed.y) > 0 && iy >= (int)(sprite.bounds.size.y))
        {
            iy = 0;
            for (int c = 0; c < columns; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    var sprRenderer = allSpriteRenderers[c * rows + r];
                    sprRenderer.transform.localPosition = new Vector3(sprRenderer.transform.localPosition.x, GetTilePosition(c, r).y);
                }
            }
        }

        //foreach (var sprRenderer in allSpriteRenderers)
        //{
        //    if (sprRenderer.name.Contains("(3,2") || sprRenderer.name.Contains("(3,3"))
        //    {
        //        Debug.Log(sprRenderer.name + " - Position: " + sprRenderer.transform.localPosition.x + " | Offset: " + sprRenderer.material.mainTextureOffset.x + " - " + Time.time);
        //    }
        //}

        //if (remainingScrollFunctions == 0)
        //{
        //    remainingScrollFunctions = allSpriteRenderers.Count;
        //    onScroll();
        //}
    }
}
