using UnityEngine;
using System.Collections;

public class OpacityFader : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public float AmountPerFrame;

    void Start()
    {
    }

    void Update()
    {
        if (this.SpriteRenderer == null) return;

        var currentColor = new Color(this.SpriteRenderer.color.r, this.SpriteRenderer.color.g, this.SpriteRenderer.color.b, this.SpriteRenderer.color.a);
        this.SpriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentColor.a - AmountPerFrame);
    }
}
