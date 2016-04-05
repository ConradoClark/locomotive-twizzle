using UnityEngine;
using System.Collections;

public class LevelGoal : MonoBehaviour
{
    public Sprite GlyphIcon;
    public TextComponent TextComponent;
    public GameObject Frame;
    public Color InactiveColor;
    public Material GlyphMaterial;
    [Range(0, 1)]
    public float InactiveSaturation;
    public Sprite CompletedMarkSprite;
    public BaseGoal baseGoal;

    public bool IsDone { get { return this.done; } }

    protected bool done;
    protected SpriteRenderer spriteRenderer;
    protected SpriteRenderer glyphRenderer;
    protected GameManager gameManager;
    protected GameObject tick;

    protected void Start()
    {
        this.gameManager = GameManager.GetGameManager();
        this.spriteRenderer = this.Frame.GetComponent<SpriteRenderer>();
        if (GlyphIcon != null)
        {
            GameObject empty = new GameObject("Glyph");
            empty.transform.SetParent(this.transform, false);
            empty.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            glyphRenderer = empty.AddComponent<SpriteRenderer>();
            glyphRenderer.sprite = GlyphIcon;
            glyphRenderer.sortingOrder = this.spriteRenderer.sortingOrder - 1;
            glyphRenderer.material = new Material(this.GlyphMaterial);

            if (!this.IsDone)
            {
                glyphRenderer.material.SetFloat("_Saturation", InactiveSaturation);
                glyphRenderer.material.SetColor("_Color", InactiveColor);
            }
        }
    }

    protected void Update()
    {
        if (baseGoal!=null && baseGoal.GoalIsMet)
        {
            this.Complete();
        }
    }

    protected void Complete()
    {
        glyphRenderer.material.SetFloat("_Saturation", 1f);
        glyphRenderer.material.SetColor("_Color", new Color(1, 1, 1, 1f));
        this.done = true;

        if (tick == null && this.CompletedMarkSprite != null)
        {
            tick = new GameObject("Tick");
            tick.transform.SetParent(this.Frame.transform, false);
            tick.transform.localPosition += Constants.Positions.CompletionMarkOffset;
            var sprRenderer = tick.AddComponent<SpriteRenderer>();
            sprRenderer.sprite = this.CompletedMarkSprite;
            sprRenderer.sortingOrder = glyphRenderer.sortingOrder + 1;
        }
    }

    public virtual void ImportSettings(string import)
    {

    }
}
