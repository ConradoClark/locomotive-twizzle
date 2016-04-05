using UnityEngine;

public class MapBGCell : MonoBehaviour
{
    public string Background;
    GameObject backgroundObjectPrefab;
    GameObject backgroundObject;

    // Use this for initialization
    private void Start()
    {
        if (!string.IsNullOrEmpty(this.Background))
        {
            backgroundObjectPrefab = Resources.Load<GameObject>(Constants.Paths.PrefabsPath + this.Background);
        }

        if (backgroundObjectPrefab != null)
        {
            backgroundObject = GameObject.Instantiate(backgroundObjectPrefab);
            backgroundObject.transform.SetParent(this.transform, false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}