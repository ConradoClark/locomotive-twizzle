using UnityEngine;
using System.Linq;

public class MapObjectCell : MonoBehaviour
{
    public int Row;
    public int Column;
    public string Prefab;

    // Use this for initialization
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
    }

    public bool IsInSamePlace(MapObjectCell cell2)
    {
        return this.Row == cell2.Row && this.Column == cell2.Column;
    }

    public bool AnyIsInSamePlace(MapObjectCell[] cells)
    {
        return cells.Any(cell => this.IsInSamePlace(cell));
    }
}