using UnityEngine;
using System.Collections;

public class ItemCount : MonoBehaviour {

    public uint quantity;
    private uint currentQuantity;
    GameObject itemCirclePrefab;
    GameObject itemCountCircle;
    TextComponent itemCountText;

    Color currentTextColor;
    // Use this for initialization
    void Start() {
        this.currentQuantity = quantity;
        this.itemCirclePrefab = Resources.Load<GameObject>(Constants.Paths.UIPath+"ItemCountCircle");
        this.itemCountCircle = UnityEngine.Object.Instantiate(itemCirclePrefab);
        this.itemCountCircle.name = "ItemCount";

        this.itemCountCircle.transform.SetParent(this.transform, false);
        this.itemCountCircle.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        itemCountText = this.itemCountCircle.GetComponent<TextComponent>();
        itemCountText.Text = currentQuantity.ToString();
        this.currentTextColor = itemCountText.TextureColor;

    }

    // Update is called once per frame
    void Update() {
        itemCountCircle.GetComponent<Renderer>().enabled = quantity > 0;
        itemCountText.GetComponent<Renderer>().enabled = quantity > 0;

        itemCountText.Text = quantity > 0 ? currentQuantity.ToString() : "";

        if (quantity > 0)
        {
            itemCountText.TextureColor = currentQuantity == 0 ? Color.red : currentTextColor;
        }
    }

    public bool CanUse()
    {
        return quantity == 0 || currentQuantity > 0;
    }
}
