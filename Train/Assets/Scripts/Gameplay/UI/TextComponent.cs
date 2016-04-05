using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextComponent : MonoBehaviour
{
    private string innerText;
    public string Text;
    public int Layer;
    public Color TextureColor;
    public bool Inverted;
    public bool RightToLeft;
    public bool InvertText;

    private bool _initialized;

    public Vector2 Offset;
    public Vector2 Scale;

    private float _space = Constants.Distances.TextSpaceDistance;
    private GameManager gameManager;

    private Sprite[] fontSpritesArray;
    private Dictionary<char, Sprite> fontSprites;
    public float CharDistance;

    public bool Translate;

    private void Awake()
    {
        this.fontSpritesArray = this.Inverted ? Resources.LoadAll<Sprite>("Sprites/Font-Inverted") : Resources.LoadAll<Sprite>("Sprites/Font");
        fontSprites = this.fontSpritesArray.ToDictionary(spr => GetSymbolTranslation(spr.name), spr => spr);
    }

    private void Start()
    {
        this.gameManager = GameManager.GetGameManager();
        if (!_initialized)
        {
            if (this.Translate)
            {
                this.Text = this.gameManager.GetTranslation(this.Text);
            }
            SetText();
        }
        this.innerText = Text;
    }

    // Update is called once per frame
    private void Update()
    {
        if (innerText != Text)
        {
            innerText = Text;
            SetText();
        }
    }

    private void OnDisable()
    {
        this.Text = "";
        SetText();
    }

    public void SetColor(Color color)
    {
        this.TextureColor = color;
        SetText();
    }

    private void SetText()
    {
        _initialized = true;
        foreach (Transform glyph in this.transform)
        {
            Destroy(glyph.gameObject);
        }
        float offset = 0;
        var charList = (InvertText ? this.Text.ToUpper().Reverse() : this.Text.ToUpper());
        foreach (char c in charList)
        {
            if (c == ' ')
            {
                offset += _space * (this.Scale.x > 0 ? this.Scale.x : 1) * (RightToLeft ? -1 : 1);
                continue;
            }

            GameObject glyph = new GameObject("Glyph_" + c.ToString());
            glyph.transform.parent = this.transform;
            glyph.transform.localScale = this.Scale == Vector2.zero ? Vector3.one : new Vector3(this.Scale.x, this.Scale.y);

            SpriteRenderer renderer = glyph.AddComponent<SpriteRenderer>();
            renderer.sprite = fontSprites[c];
            renderer.transform.parent = glyph.transform;
            renderer.transform.localPosition = Vector3.zero + new Vector3(this.Offset.x + offset, this.Offset.y, 0);
            renderer.sortingOrder = this.Layer;
            renderer.material.SetColor("_Color", this.TextureColor);

            offset += (renderer.sprite.rect.size.x + this.CharDistance) * (this.Scale.x > 0 ? this.Scale.x : 1) * (RightToLeft ? -1 : 1);
        }
    }

    private char GetSymbolTranslation(string symbol)
    {
        if (symbol == null) return '0';
        if (symbol.Length == 1)
        {
            return symbol.ToUpper().Single();
        }
        if (symbol.StartsWith("Num"))
        {
            return symbol.SkipWhile(c => c != '_').Skip(1).Take(1).FirstOrDefault();
        }
        if (symbol.StartsWith("Sym"))
        {
            return TransformSymbolToSign(symbol.SkipWhile(c => c != '_').Skip(1));
        }
        return '0';
    }

    private char TransformSymbolToSign(IEnumerable<char> symbol)
    {
        switch (new string(symbol.ToArray()))
        {
            case "Colon":
                return ':';

            case "Exclamation":
                return '!';

            case "Fullstop":
                return '.';

            case "Minus":
                return '-';

            case "Plus":
                return '+';

            case "Question":
                return '?';

            case "Comma":
                return ',';

            case "Apostrophe":
                return '\'';

            case "Infinity":
                return '∞';

            default:
                return '.';
        }
    }
}