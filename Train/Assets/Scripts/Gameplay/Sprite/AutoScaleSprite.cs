using UnityEngine;
using System.Collections;

public class AutoScaleSprite : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private RectTransform rectTransform;

    void Start()
    {
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.rectTransform = this.GetComponent<RectTransform>();
        if (this.spriteRenderer != null && this.rectTransform != null)
        {
            Vector2 spriteSize = this.spriteRenderer.sprite.bounds.size;
            Vector2 rectSize = this.rectTransform.rect.size;

            Vector2 scale = new Vector2(rectSize.x / spriteSize.x, rectSize.y / spriteSize.y);
            this.transform.localScale = scale;
        }
    }
}
