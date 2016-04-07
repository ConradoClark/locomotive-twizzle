using UnityEngine;
using System.Collections;
using System.Linq;

public class InventorySlot : InventoryClickable
{
    public Material IconMaterial;
    public Sprite IconSprite;
    public Sprite IconSpriteWhenSelected;
    public Color ColorWhenUnavailable;
    public Color IconColorWhenUnavailable;
    public Color ColorWhenSelected;
    public int OrderInLayer;
    public int OrderInLayerWhenSelected;
    public Vector2 IconScale;
    public bool Selected;
    public bool Available;
    public GameObject SelectionEffect;

    private GameObject icon;
    private SpriteRenderer iconRenderer;
    private RectTransform iconTransform;
    private SpriteRenderer slotRenderer;
    private RectTransform slotTransform;
    private Coroutine activeCoroutine;
    private Color color;
    private Inventory inventory;

    void Start()
    {
        this.slotRenderer = this.GetComponent<SpriteRenderer>();
        this.inventory = this.GetComponentInParent<Inventory>();

        this.color = this.slotRenderer.color;
        this.icon = new GameObject("Icon");
        this.iconRenderer = this.icon.AddComponent<SpriteRenderer>();
        this.iconTransform = this.icon.AddComponent<RectTransform>();
        this.slotTransform = this.GetComponent<RectTransform>();
        this.iconRenderer.material = this.IconMaterial;

        this.iconTransform.SetParent(this.transform, false);
        this.iconTransform.anchorMin = new Vector2(0f, 0.5f);
        this.iconTransform.anchorMax = new Vector2(0f, 0.5f);
        this.iconTransform.sizeDelta = this.slotTransform.sizeDelta;
        this.iconTransform.pivot = new Vector2(0f, 0.5f);
        this.iconTransform.localScale = this.IconScale;
        this.iconTransform.anchoredPosition = Vector2.zero;

        this.StartCoroutines();
    }

    IEnumerator SelectedStateCoroutine()
    {
        this.iconRenderer.sortingOrder = this.OrderInLayerWhenSelected;
        this.iconRenderer.sprite = this.IconSpriteWhenSelected;
        this.slotRenderer.color = this.ColorWhenSelected;
        yield return 0;

        while (this.enabled)
        {
            var inputStates = inventory.InputStatesOnInventory;
            if (inputStates.Any(i => i.IsMainActionReleasedOnObject && i.AffectedObjects.Contains(this)))
            {
                var effect = Instantiate(this.SelectionEffect);
                effect.transform.SetParent(this.transform, false);

                this.activeCoroutine = this.StartCoroutine(UnselectedStateCoroutine());
                yield break;
            }
            yield return 1;
        }
        this.activeCoroutine = this.StartCoroutine(WaitForEnableCoroutine());
        yield break;
    }

    IEnumerator UnselectedStateCoroutine()
    {
        this.iconRenderer.sortingOrder = this.OrderInLayer;
        this.iconRenderer.sprite = this.IconSprite;
        this.slotRenderer.color = this.color;        
        yield return 0;

        while (this.enabled)
        {
            var inputStates = inventory.InputStatesOnInventory;
            if (inputStates.Any(i => i.IsMainActionReleasedOnObject && i.AffectedObjects.Contains(this)))
            {
                var effect = Instantiate(this.SelectionEffect);
                effect.transform.SetParent(this.transform, false);

                this.activeCoroutine = this.StartCoroutine(SelectedStateCoroutine());
                yield break;
            }
            yield return 1;
        }
        this.activeCoroutine = this.StartCoroutine(WaitForEnableCoroutine());
        yield break;
    }

    IEnumerator UnavailableStateCoroutine()
    {
        this.iconRenderer.sortingOrder = this.OrderInLayer;
        this.iconRenderer.sprite = this.IconSprite;
        this.iconRenderer.color = this.IconColorWhenUnavailable;
        this.slotRenderer.color = this.ColorWhenUnavailable;

        while (this.enabled)
        {
            yield return 1;
        }
        this.activeCoroutine = this.StartCoroutine(WaitForEnableCoroutine());
        yield break;
    }

    IEnumerator WaitForEnableCoroutine()
    {
        while (!enabled)
        {
            yield return 0;
        }

        this.StartCoroutines();
    }

    void StartCoroutines()
    {
        if (!this.Available)
        {
            this.activeCoroutine = this.StartCoroutine(UnavailableStateCoroutine());
            return;
        }
        if (!this.Selected)
        {
            this.activeCoroutine = this.StartCoroutine(UnselectedStateCoroutine());            
            return;
        }
        this.activeCoroutine = this.StartCoroutine(SelectedStateCoroutine());
    }
}
